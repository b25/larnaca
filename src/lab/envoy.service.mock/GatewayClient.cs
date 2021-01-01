using envoy.contracts;
using envoy.service.Contracts;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
        private readonly IServerAddressesFeature _serverAddresses;
        private readonly EndpointDataSource _endpointDataSource;
        private readonly IGatewayService _gateway;
        private uint _port;
        private List<string> _routes;
        private Timer _timer;

        public GatewayClient(IServer server, EndpointDataSource endpointDataSource, IOptions<ServiceOptions> options)
        {
            _serverAddresses = server.Features.Get<IServerAddressesFeature>();
            _endpointDataSource = endpointDataSource;

            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            var http = GrpcChannel.ForAddress(options.Value.GatewayAddress);

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
                    Routes = _routes
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
            _port = (uint)(_serverAddresses?.Addresses?.Select(address => {
                if (address.Contains(':'))
                {
                    return int.Parse(address.Split(':').Last());
                }

                return address.StartsWith("https") ? 443 : 80;
            })?.FirstOrDefault() ?? 80);

            _routes = RegisterRequest.GetRoutes(_endpointDataSource, new List<Type> { typeof(ITestService) });

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
