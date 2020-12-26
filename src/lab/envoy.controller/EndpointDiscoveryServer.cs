using envoy.controller.Cache;
using Envoy.Service.Discovery.V3;
using Envoy.Service.Endpoint.V3;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace envoy.controller
{
    public class EndpointDiscoveryServer : EndpointDiscoveryService.EndpointDiscoveryServiceBase
    {
        private readonly GatewayController _service;

        public EndpointDiscoveryServer(GatewayController service)
        {
            _service = service;
        }

        public override Task StreamEndpoints(IAsyncStreamReader<DiscoveryRequest> requestStream, IServerStreamWriter<DiscoveryResponse> responseStream, ServerCallContext context)
        {
            return _service.HandleStream(requestStream, responseStream, context, TypeStrings.RouteType);
        }
    }
}
