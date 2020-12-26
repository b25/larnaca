using collection.extensions;
using Envoy.Config.Cluster.V3;
using Envoy.Config.Core.V3;
using Envoy.Config.Endpoint.V3;
using Envoy.Config.Listener.V3;
using Envoy.Extensions.Filters.Network.HttpConnectionManager.V3;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace envoy.controller.Cache
{
    public class Snapshot
    {
        private const string DefaultListener = "default";
        private readonly uint _peerMaxConcurrentStreams;
        private readonly uint _initialStreamWindowSize;
        private readonly uint _initialConnectionWindowSize;
        private readonly Duration _routeTimeout;
        private static int _version = 0;

        public Resources Endpoints { get; }
        public Resources Clusters { get; }
        public Resources Routes { get; }
        public Resources Listeners { get; }

        public Snapshot(GatewayOptions options)
        {
            var version = Interlocked.Increment(ref _version).ToString();

            _routeTimeout = Duration.FromTimeSpan(TimeSpan.FromSeconds(options.RouteTimeoutSeconds));
            _peerMaxConcurrentStreams = options.PeerMaxConcurrentStreams;
            _initialStreamWindowSize = options.InitialStreamWindowSize;
            _initialConnectionWindowSize = options.InitialConnectionWindowSize;
            Endpoints = new Resources(version, new Dictionary<string, IMessage>());
            Clusters = new Resources(version, new Dictionary<string, IMessage>());
            Routes = new Resources(version, new Dictionary<string, IMessage>());
            Listeners = new Resources(version, new Dictionary<string, IMessage>());

            var listener = new Listener
            {
                Name = DefaultListener,
                Address = new Address
                {
                    SocketAddress = new SocketAddress
                    {
                        Address = "0.0.0.0",
                        PortValue = options.ListenPort
                    }
                },
                PerConnectionBufferLimitBytes = options.PerConnectionBufferLimitBytes,
                FilterChains =
                {
                    new FilterChain 
                    { 
                        Filters =
                        {
                            new Envoy.Config.Listener.V3.Filter
                            {
                                Name = "envoy.filters.network.http_connection_manager",
                                TypedConfig = Any.Pack(new HttpConnectionManager
                                {
                                    CodecType = HttpConnectionManager.Types.CodecType.Http2,
                                    StatPrefix = "ingress_grpc",
                                    Http2ProtocolOptions = new Http2ProtocolOptions
                                    {
                                        MaxConcurrentStreams = _peerMaxConcurrentStreams,
                                        InitialStreamWindowSize = _initialStreamWindowSize,
                                        InitialConnectionWindowSize = _initialConnectionWindowSize,
                                    },
                                    RouteConfig = new Envoy.Config.Route.V3.RouteConfiguration
                                    {
                                        Name = DefaultListener,
                                        VirtualHosts =
                                        {
                                            new Envoy.Config.Route.V3.VirtualHost
                                            {
                                                Name = DefaultListener,
                                                Domains = { "*" }
                                            }
                                        }
                                    },
                                    HttpFilters =
                                    {
                                        new HttpFilter
                                        {
                                            Name = "envoy.filters.http.router",
                                            TypedConfig = Any.Pack(new Envoy.Extensions.Filters.Http.Router.V3.Router())
                                        }
                                    }
                                })
                            }
                        }
                    }
                }
            };

            Listeners.Items.Add(listener.Name, listener);
        }

        public Snapshot(Resources endpoints, Resources clusters, Resources routes, Resources listeners)
        {
            Endpoints = endpoints;
            Clusters = clusters;
            Routes = routes;
            Listeners = listeners;
        }

        public Snapshot WithClusters(Resources clusters) => new Snapshot(Endpoints, clusters, Routes, Listeners);

        public Snapshot With(string podAddress, uint podPort, List<string> addedRoutes, List<string> removedRoutes)
        {
            var version = Interlocked.Increment(ref _version).ToString();
            var endpoint = new Endpoint()
            {
                Address = new Address()
                {
                    SocketAddress = new SocketAddress()
                    {
                        Address = podAddress,
                        PortValue = podPort
                    }
                }
            };

            var clusters = new Dictionary<string, IMessage>();

            foreach (var pair in Clusters.Items)
            {
                if (pair.Value is Cluster cluster)
                {
                    if (removedRoutes.Contains(cluster.Name))
                    {
                        // remove endpoint for this route
                        var updatedCluster = new Cluster(cluster);
                        var localityLbEndpointsToRemove = new List<LocalityLbEndpoints>();

                        foreach (var localityLbEndpoints in updatedCluster.LoadAssignment.Endpoints)
                        {
                            localityLbEndpoints.LbEndpoints.RemoveFirst(lbEndpoint =>
                            {
                                return lbEndpoint.Endpoint.Address.SocketAddress.Address == podAddress && lbEndpoint.Endpoint.Address.SocketAddress.PortValue == podPort;
                            });

                            if (!localityLbEndpoints.LbEndpoints.Any())
                            {
                                // keep track of empty localityLbEndpoints
                                localityLbEndpointsToRemove.Add(localityLbEndpoints);
                            }
                        }

                        if (updatedCluster.LoadAssignment.Endpoints.Count > localityLbEndpointsToRemove.Count)
                        {
                            foreach (var localityLbEndpoints in localityLbEndpointsToRemove)
                            {
                                // remove empty localityLbEndpoints
                                updatedCluster.LoadAssignment.Endpoints.Remove(localityLbEndpoints);
                            }

                            clusters[updatedCluster.Name] = updatedCluster;
                        }

                        continue;
                    }

                    clusters[cluster.Name] = cluster;
                }
            }


            // add new clusters route
            foreach (var route in addedRoutes)
            {
                Cluster cluster;

                if (clusters.TryGetValue(route, out var message))
                {
                    // a cluster already exists
                    cluster = new Cluster(message as Cluster);
                }
                else
                {
                    // create a new cluster
                    cluster = new Cluster
                    {
                        Name = route,
                        ConnectTimeout = new Duration 
                        {
                            Seconds = 1,
                            Nanos = 0
                        },
                        PerConnectionBufferLimitBytes = 32768, // 32 KiB
                        Type = Cluster.Types.DiscoveryType.StrictDns,
                        LbPolicy = Cluster.Types.LbPolicy.LeastRequest,
                        Http2ProtocolOptions = new Http2ProtocolOptions
                        {
                            InitialStreamWindowSize = _initialStreamWindowSize,
                            InitialConnectionWindowSize = _initialConnectionWindowSize
                        },
                        LoadAssignment = new ClusterLoadAssignment
                        {
                            ClusterName = route
                        }
                    };
                }

                cluster.LoadAssignment.Endpoints.Add(new LocalityLbEndpoints
                {
                    LbEndpoints =
                    {
                        new LbEndpoint
                        {
                            Endpoint = endpoint
                        }
                    }
                });

                clusters[cluster.Name] = cluster;
            }

            var listeners = new Dictionary<string, IMessage>();

            foreach (var pair in Listeners.Items)
            {
                if (pair.Value is Listener listener)
                {
                    if (listener.Name != DefaultListener)
                    {
                        // do not modify other listeners
                        listeners[listener.Name] = listener;

                        continue;
                    }

                    var updatedListener = new Listener(listener);
                    var filters = updatedListener.FilterChains.SelectMany(filterChain => filterChain.Filters).Where(filter => filter.TypedConfig.Is(HttpConnectionManager.Descriptor));

                    foreach (var filter in filters)
                    {
                        var connectionManager = filter.TypedConfig.Unpack<HttpConnectionManager>();
                        var virtualHost = connectionManager.RouteConfig.VirtualHosts.FirstOrDefault(virtualHost => virtualHost.Name == DefaultListener);

                        if (virtualHost != null)
                        { 
                            // remove routes that point to removed clusters
                            virtualHost.Routes.RemoveAll(route => !clusters.ContainsKey(route.Route_.Cluster));

                            // add new routes
                            foreach (var route in addedRoutes)
                            {
                                virtualHost.Routes.Add(new Envoy.Config.Route.V3.Route
                                {
                                    Match = new Envoy.Config.Route.V3.RouteMatch
                                    {
                                        Path = route,
                                        Grpc = new Envoy.Config.Route.V3.RouteMatch.Types.GrpcRouteMatchOptions()
                                    },
                                    Route_ = new Envoy.Config.Route.V3.RouteAction
                                    {
                                        Cluster = route,
                                        Timeout = _routeTimeout
                                    }
                                });
                            }
                        }

                        filter.TypedConfig = Any.Pack(connectionManager);
                    }

                    listeners[updatedListener.Name] = updatedListener;
                }
            }

            return new Snapshot(
                Endpoints,
                new Resources(version, clusters),
                Routes,
                new Resources(version, listeners)
            );
        }

        public string GetVersion(string type)
        {
            return GetByType(type, e => e.Version);
        }

        public IDictionary<string, IMessage> GetResources(string type)
        {
            return GetByType(type, e => e.Items);
        }

        private T GetByType<T>(
            string type,
            Func<Resources, T> selector)
        {
            switch (type)
            {
                case TypeStrings.EndpointType:
                    return selector(Endpoints);
                case TypeStrings.ClusterType:
                    return selector(Clusters);
                case TypeStrings.RouteType:
                    return selector(Routes);
                case TypeStrings.ListenerType:
                    return selector(Listeners);
                default:
                    throw new ArgumentOutOfRangeException(type);
            }
        }

    }
}
