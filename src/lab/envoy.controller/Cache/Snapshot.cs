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
        private const string GatewayClusterName = "ads_cluster";
        private static uint _peerMaxConcurrentStreams;
        private static uint _initialStreamWindowSize;
        private static uint _initialConnectionWindowSize;
        private static Duration _routeTimeout;
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
                                    Rds = new Rds
                                    {
                                        RouteConfigName = DefaultListener,
                                        ConfigSource = new ConfigSource
                                        {
                                            Ads = new AggregatedConfigSource(),
                                            ResourceApiVersion = ApiVersion.V3
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

            var routeConfig = new Envoy.Config.Route.V3.RouteConfiguration
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
            };

            Routes.Items.Add(routeConfig.Name, routeConfig);
        }

        public Snapshot(Resources endpoints, Resources clusters, Resources routes, Resources listeners)
        {
            Endpoints = endpoints;
            Clusters = clusters;
            Routes = routes;
            Listeners = listeners;
        }

        public Snapshot With(string podAddress, uint podPort, List<string> addedRoutes, List<string> removedRoutes)
        {
            var version = Interlocked.Increment(ref _version).ToString();
            var clusters = new Dictionary<string, IMessage>();
            var endpoints = new Dictionary<string, IMessage>();

            // keep only valid endpoints
            foreach (var pair in Endpoints.Items)
            {
                if (pair.Value is ClusterLoadAssignment clusterLoadAssignment)
                {
                    if (removedRoutes.Contains(clusterLoadAssignment.ClusterName))
                    {
                        // remove endpoint for this route
                        var updatedClusterLoadAssignment = new ClusterLoadAssignment(clusterLoadAssignment);
                        var localityLbEndpointsToRemove = new List<LocalityLbEndpoints>();

                        foreach (var localityLbEndpoints in updatedClusterLoadAssignment.Endpoints)
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

                        if (updatedClusterLoadAssignment.Endpoints.Count > localityLbEndpointsToRemove.Count)
                        {
                            foreach (var localityLbEndpoints in localityLbEndpointsToRemove)
                            {
                                // remove empty localityLbEndpoints
                                updatedClusterLoadAssignment.Endpoints.Remove(localityLbEndpoints);
                            }

                            endpoints[updatedClusterLoadAssignment.ClusterName] = updatedClusterLoadAssignment;
                        }

                        continue;
                    }

                    endpoints[clusterLoadAssignment.ClusterName] = clusterLoadAssignment;
                }
            }

            // keep only valid clusters
            foreach (var pair in Clusters.Items)
            {
                if (pair.Value is Cluster cluster)
                {
                    if (endpoints.ContainsKey(cluster.Name))
                    {
                        // keep only clusters that have endpoints
                        clusters[cluster.Name] = cluster;
                    }
                }
            }


            // add new clusters and endpoints
            foreach (var route in addedRoutes)
            {
                if (!clusters.ContainsKey(route))
                {
                    // create a new cluster
                    var cluster = new Cluster
                    {
                        Name = route,
                        ConnectTimeout = new Duration
                        {
                            Seconds = 1,
                            Nanos = 0
                        },
                        PerConnectionBufferLimitBytes = 32768, // 32 KiB
                        Type = Cluster.Types.DiscoveryType.Eds,
                        EdsClusterConfig = new Cluster.Types.EdsClusterConfig
                        {
                            EdsConfig = new ConfigSource
                            {
                                Ads = new AggregatedConfigSource(),
                                ResourceApiVersion = ApiVersion.V3
                            }
                        },
                        LbPolicy = Cluster.Types.LbPolicy.LeastRequest,
                        TypedExtensionProtocolOptions = {
                            {
                                "envoy.extensions.upstreams.http.v3.HttpProtocolOptions",
                                Any.Pack(new Envoy.Extensions.Upstreams.Http.V3.HttpProtocolOptions {
                                    ExplicitHttpConfig = new Envoy.Extensions.Upstreams.Http.V3.HttpProtocolOptions.Types.ExplicitHttpConfig
                                    {
                                        Http2ProtocolOptions = new Http2ProtocolOptions
                                        {
                                            InitialStreamWindowSize = _initialStreamWindowSize,
                                            InitialConnectionWindowSize = _initialConnectionWindowSize
                                        }
                                    }
                                })
                            }
                        }
                    };

                    clusters[cluster.Name] = cluster;
                }

                ClusterLoadAssignment clusterLoadAssignment;

                if (endpoints.TryGetValue(route, out var message))
                {
                    // a cluster load assignment already exists
                    clusterLoadAssignment = new ClusterLoadAssignment(message as ClusterLoadAssignment);
                }
                else
                {
                    // create a new cluster load assignment
                    clusterLoadAssignment = new ClusterLoadAssignment
                    {
                        ClusterName = route
                    };
                }

                // add the new endpoint to the cluster load assignment
                clusterLoadAssignment.Endpoints.Add(new LocalityLbEndpoints
                {
                    LbEndpoints =
                    {
                        new LbEndpoint
                        {
                            Endpoint = new Endpoint()
                            {
                                Address = new Address()
                                {
                                    SocketAddress = new SocketAddress()
                                    {
                                        Address = podAddress,
                                        PortValue = podPort
                                    }
                                }
                            }
                        }
                    }
                });

                endpoints[route] = clusterLoadAssignment;
            }

            var routes = GetRoutes(addedRoutes, clusters.Keys);

            return new Snapshot(
                new Resources(version, endpoints),
                new Resources(version, clusters),
                new Resources(version, routes),
                Listeners
            );
        }

        public Snapshot WithGatewayRoutes(List<string> gatewayRoutes)
        {
            var version = Interlocked.Increment(ref _version).ToString();
            var routes = GetRoutes(gatewayRoutes, Clusters.Items.Keys, true);

            return new Snapshot(
                Endpoints,
                Clusters,
                new Resources(version, routes),
                Listeners
            );
        }

        private Dictionary<string, IMessage> GetRoutes(List<string> addedRoutes, ICollection<string> validClusterNames, bool useGatewayClusterForRoutes = false)
        {
            var routes = new Dictionary<string, IMessage>();

            // add new routes
            foreach (var pair in Routes.Items)
            {
                if (pair.Value is Envoy.Config.Route.V3.RouteConfiguration routeConfiguration)
                {
                    if (routeConfiguration.Name != DefaultListener)
                    {
                        // do not modify other routes
                        routes[routeConfiguration.Name] = routeConfiguration;

                        continue;
                    }

                    var updatedRouteConfiguration = new Envoy.Config.Route.V3.RouteConfiguration(routeConfiguration);
                    var virtualHost = updatedRouteConfiguration.VirtualHosts.FirstOrDefault(virtualHost => virtualHost.Name == DefaultListener);

                    if (virtualHost != null)
                    {
                        // remove routes that point to removed clusters
                        virtualHost.Routes.RemoveAll(route => route.Route_.Cluster != GatewayClusterName && !validClusterNames.Contains(route.Route_.Cluster));

                        // add new routes
                        foreach (var route in addedRoutes)
                        {
                            if (virtualHost.Routes.FindIndex(configRoute => configRoute.Match.Path == route) != -1)
                            {
                                // skip routes that already exist
                                continue;
                            }

                            virtualHost.Routes.Add(new Envoy.Config.Route.V3.Route
                            {
                                Match = new Envoy.Config.Route.V3.RouteMatch
                                {
                                    Path = route,
                                    Grpc = new Envoy.Config.Route.V3.RouteMatch.Types.GrpcRouteMatchOptions()
                                },
                                Route_ = new Envoy.Config.Route.V3.RouteAction
                                {
                                    Cluster = useGatewayClusterForRoutes ? GatewayClusterName : route,
                                    Timeout = _routeTimeout
                                }
                            });
                        }
                    }

                    routes[updatedRouteConfiguration.Name] = updatedRouteConfiguration;
                }
            }

            return routes;
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
