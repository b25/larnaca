using envoy.service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace envoy.service
{
    public class TestService : ITestService
    {
        private static string Id = Guid.NewGuid().ToString();

        public TestService()
        {
            Console.WriteLine($"Starting server with Id: {Id}");
        }

        public Task<EchoResponse> Echo(EchoRequest request)
        {
            return Task.FromResult(new EchoResponse
            {
                Message = $"{request.Message} from {Id}"
            });
        }
    }
}
