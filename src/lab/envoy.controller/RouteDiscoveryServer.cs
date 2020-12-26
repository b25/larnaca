using envoy.controller.Cache;
using Envoy.Service.Discovery.V3;
using Envoy.Service.Route.V3;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace envoy.controller
{
    public class RouteDiscoveryServer : RouteDiscoveryService.RouteDiscoveryServiceBase
    {
        private readonly GatewayController _service;

        public RouteDiscoveryServer(GatewayController service)
        {
            _service = service;
        }

        public override Task StreamRoutes(IAsyncStreamReader<DiscoveryRequest> requestStream, IServerStreamWriter<DiscoveryResponse> responseStream, ServerCallContext context)
        {
            return _service.HandleStream(requestStream, responseStream, context, TypeStrings.RouteType);
        }
    }
}
