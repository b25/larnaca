using envoy.contracts;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace envoy.service
{
    public class GatewayClient: IHostedService
    {
        private readonly uint _port;

        private readonly IGatewayService _gateway;

        private Timer _timer;

        public GatewayClient(IConfiguration configuration)
        {
            _port = configuration.GetValue<uint>("port_http", 50050);

            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            var http = GrpcChannel.ForAddress("http://localhost:5000");
            _gateway = http.CreateGrpcService<IGatewayService>();
        }

        private static bool IsIPv4(IPAddress ipa) => ipa.AddressFamily == AddressFamily.InterNetwork;

        private static IPAddress GetMainIPv4() => NetworkInterface.GetAllNetworkInterfaces()
            .Select((ni) => ni.GetIPProperties())
            .Where((ip) => ip.GatewayAddresses.Where((ga) => IsIPv4(ga.Address)).Count() > 0)
            .FirstOrDefault()?.UnicastAddresses?
            .Where((ua) => IsIPv4(ua.Address))?.FirstOrDefault()?.Address;

        public async void RegisterToGateway(object state)
        {
            try
            {
                var result = await _gateway.Register(new RegisterRequest
                {
                    PodAddress = GetMainIPv4()?.ToString() ?? "localhost",
                    PodPort = _port,
                    Routes =
                    {
                        "/envoy.service.TestService/Echo"
                    }
                });

                Console.WriteLine($"Registered to Gateway {(result.Registered ? "successfully" : "unsuccessfully")}!");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Encountered exception when registering to gateway: {ex.Message}");
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"Gateway registration canceled: {ex.Message}");
            }
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                RegisterToGateway,
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(1)
            );

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            try
            {
                var result = await _gateway.Unregister(new UnregisterRequest
                {
                    PodAddress = GetMainIPv4()?.ToString() ?? "localhost",
                    PodPort = _port
                });

                Console.WriteLine($"Unregistered from Gateway {(result.Unregistered ? "successfully" : "unsuccessfully")}!");
            }
            catch (RpcException ex)
            {
                Console.WriteLine($"Encountered exception when registering to gateway: {ex.Message}");
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"Gateway registration canceled: {ex.Message}");
            }
        }
    }
}
