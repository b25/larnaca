using System.ServiceModel;
using System.Threading.Tasks;

namespace envoy.contracts
{
    [ServiceContract(Name = "envoy.gateway.GatewayService")]
    public interface IGatewayService
    {
        Task<RegisterResponse> Register(RegisterRequest request);

        Task<UnregisterResponse> Unregister(UnregisterRequest request);
    }
}
