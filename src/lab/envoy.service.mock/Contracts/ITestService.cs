using System.ServiceModel;
using System.Threading.Tasks;

namespace envoy.service.Contracts
{
    [ServiceContract(Name = "envoy.service.TestService")]
    public interface ITestService
    {
        Task<EchoResponse> Echo(EchoRequest request);
    }
}
