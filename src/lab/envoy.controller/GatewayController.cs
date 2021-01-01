using envoy.controller.Cache;
using envoy.contracts;
using Envoy.Config.Cluster.V3;
using Envoy.Config.Core.V3;
using Envoy.Config.Endpoint.V3;
using Envoy.Service.Discovery.V3;
using Grpc.Core;
using Grpc.Core.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace envoy.controller
{
    public class GatewayController
    {
        private readonly ISnapshotCache _cache;
        private Snapshot _snapshot;
        private readonly HashSet<string> _nodes = new HashSet<string>();
        private readonly Dictionary<string, RegisterRequest> _registrations = new Dictionary<string, RegisterRequest>();
        private readonly object _lock = new object();
        private readonly ILogger _logger = new ConsoleLogger().ForType<GatewayController>();
        private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;

        public GatewayController(IOptions<GatewayOptions> options)
        {
            _cache = new SnapshotCache(true, _logger);
            _snapshot = new Snapshot(options.Value);
        }

        public void SetGatewayRoutes(List<string> routes)
        {
            lock (_lock)
            {
                _snapshot = _snapshot.WithGatewayRoutes(routes);
                
                _cache.SetSnapshot(_snapshot);
            }
        }

        public RegisterResponse Register(RegisterRequest request)
        {
            lock (_lock)
            {
                var registrationKey = $"{request.PodAddress}:{request.PodPort}";
                var currentRoutes = _registrations.GetValueOrDefault(registrationKey, null)?.Routes ?? new List<string>();
                var removedRoutes = currentRoutes.Except(request.Routes).ToList();
                var addedRoutes = request.Routes.Except(currentRoutes).ToList();

                if (addedRoutes.Any() || removedRoutes.Any())
                {
                    _snapshot = _snapshot.With(request.PodAddress, request.PodPort, addedRoutes, removedRoutes);

                    _cache.SetSnapshot(_snapshot);

                    _registrations[registrationKey] = request;
                }

                return new RegisterResponse { Registered = true };
            }
        }

        public UnregisterResponse Unregister(UnregisterRequest request)
        {
            lock (_lock)
            {
                var registrationKey = $"{request.PodAddress}:{request.PodPort}";
                var currentRoutes = _registrations.GetValueOrDefault(registrationKey, null)?.Routes;

                if (currentRoutes?.Any() == true)
                {
                    _snapshot = _snapshot.With(request.PodAddress, request.PodPort, new List<string>(), currentRoutes);

                    _cache.SetSnapshot(_snapshot);

                    _registrations.Remove(registrationKey);

                    return new UnregisterResponse { Unregistered = true };
                }

                return new UnregisterResponse { Unregistered = false };
            }
        }

        public async Task HandleStream(
            IAsyncStreamReader<DiscoveryRequest> requestStream,
            IAsyncStreamWriter<DiscoveryResponse> responseStream,
            ServerCallContext context,
            string defaultTypeUrl)
        {
            var streamId = Guid.NewGuid();
            _logger.InfoF($"New Stream started {streamId}");

            var clusters = new WatchAndNounce();
            var endpoints = new WatchAndNounce();
            var listeners = new WatchAndNounce();
            var routes = new WatchAndNounce();

            var watches = new Dictionary<string, WatchAndNounce>()
            {
                {TypeStrings.ClusterType, clusters},
                {TypeStrings.EndpointType, endpoints},
                {TypeStrings.ListenerType, listeners},
                {TypeStrings.RouteType, routes},
            };

            try
            {
                var streamNonce = 0;

                var requestTask = requestStream.MoveNext(_cancellationToken);

                while (!_cancellationToken.IsCancellationRequested &&
                       !context.CancellationToken.IsCancellationRequested)
                {
                    var resolved = await Task.WhenAny(
                        requestTask,
                        clusters.Watch.Response,
                        endpoints.Watch.Response,
                        listeners.Watch.Response,
                        routes.Watch.Response);

                    switch (resolved)
                    {
                        case Task<bool> r when ReferenceEquals(r, requestTask):
                            // New request from Envoy
                            if (!r.Result)
                            {
                                // MoveNext failed. The stream was finalized by Envoy or is broken
                                throw new OperationCanceledException();
                            }

                            var request = requestStream.Current;

                            if (request.Node?.Id == null)
                            {
                                _logger.Warning("Missing Node ID in envoy request. Check configuration");
                                throw new InvalidOperationException("Missing Node ID in envoy request. Check configuration");
                            }

                            if (defaultTypeUrl == TypeStrings.Any && string.IsNullOrEmpty(request.TypeUrl))
                            {
                                _logger.Warning("type URL is required for ADS");
                                throw new InvalidOperationException("type URL is required for ADS");
                            }

                            if (string.IsNullOrEmpty(request.TypeUrl))
                            {
                                request.TypeUrl = defaultTypeUrl;
                            }

                            _logger.DebugF($"<- New request on stream {streamId}, typeUrl {request.TypeUrl}, version {request.VersionInfo}, nonce: {request.ResponseNonce}");

                            var requestWatch = watches[request.TypeUrl];
                            if (requestWatch.Nonce == null || requestWatch.Nonce == request.ResponseNonce)
                            {
                                // if the nonce is not correct ignore the request.
                                requestWatch.Watch.Cancel();
                                requestWatch.Watch = _cache.CreateWatch(request);

                                if (_nodes.Add(request.Node.Id))
                                {
                                    // send to this newly connected node envoy the current snapshot
                                    _cache.SetSnapshot(request.Node.Id, _snapshot);
                                }
                            }
                            else
                            {
                                _logger.DebugF($"<- Ignoring request on stream {streamId}, expectedNonce: {requestWatch.Nonce}, recievedNonce: {request.ResponseNonce}");
                            }

                            requestTask = requestStream.MoveNext(_cancellationToken);
                            break;
                        case Task<DiscoveryResponse> responseTask:
                            // Watch was resolved. Send the update.
                            var response = responseTask.Result;
                            var responseWatch = watches[response.TypeUrl];

                            response.Nonce = (++streamNonce).ToString();
                            responseWatch.Nonce = streamNonce.ToString();

                            // the value is consumed we do not want to get it again from this watch. 
                            responseWatch.Watch.Cancel();
                            responseWatch.Watch = Watch.Empty;

                            _logger.DebugF($"-> New response on stream {streamId}, typeUrl {response.TypeUrl}, version {response.VersionInfo}, nonce: {response.Nonce}");

                            await responseStream.WriteAsync(response);
                            break;
                    }
                }
            }
            finally
            {
                // cleanup the cache before exit
                clusters.Watch.Cancel();
                endpoints.Watch.Cancel();
                listeners.Watch.Cancel();
                routes.Watch.Cancel();

                _logger.InfoF($"Stream finalized {streamId}");
            }
        }

        private class WatchAndNounce
        {
            public WatchAndNounce()
            {
                Watch = Watch.Empty;
            }

            public Watch Watch;
            public string Nonce;
        }

    }
}
