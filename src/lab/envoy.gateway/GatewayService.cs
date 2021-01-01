using envoy.controller;
using envoy.contracts;
using System.Threading.Tasks;

namespace envoy.gateway
{
    public class GatewayService : IGatewayService
    {
        private readonly GatewayController _service;

        public GatewayService(GatewayController service)
        {
            _service = service;
        }

        public Task<RegisterResponse> Register(RegisterRequest request)
        {
            return Task.FromResult(_service.Register(request));
        }

        public Task<UnregisterResponse> Unregister(UnregisterRequest request)
        {
            return Task.FromResult(_service.Unregister(request));
        }
    }
}
