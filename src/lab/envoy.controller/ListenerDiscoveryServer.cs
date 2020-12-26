using envoy.controller.Cache;
using Envoy.Service.Discovery.V3;
using Envoy.Service.Listener.V3;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace envoy.controller
{
    public class ListenerDiscoveryServer : ListenerDiscoveryService.ListenerDiscoveryServiceBase
    {
        private readonly GatewayController _service;

        public ListenerDiscoveryServer(GatewayController service)
        {
            _service = service;
        }

        public override Task StreamListeners(IAsyncStreamReader<DiscoveryRequest> requestStream, IServerStreamWriter<DiscoveryResponse> responseStream, ServerCallContext context)
        {
            return _service.HandleStream(requestStream, responseStream, context, TypeStrings.ListenerType);
        }
    }
}
