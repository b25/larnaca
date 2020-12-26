using envoy.controller.Cache;
using Envoy.Service.Cluster.V3;
using Envoy.Service.Discovery.V3;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace envoy.controller
{
    public class ClusterDiscoveryServer : ClusterDiscoveryService.ClusterDiscoveryServiceBase
    {
        private readonly GatewayController _service;

        public ClusterDiscoveryServer(GatewayController service)
        {
            _service = service;
        }

        public override Task StreamClusters(IAsyncStreamReader<DiscoveryRequest> requestStream, IServerStreamWriter<DiscoveryResponse> responseStream, ServerCallContext context)
        {
            return _service.HandleStream(requestStream, responseStream, context, TypeStrings.ClusterType);
        }
    }
}
