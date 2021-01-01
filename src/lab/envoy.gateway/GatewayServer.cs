using envoy.contracts;
using envoy.controller;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace envoy.gateway
{
    public class GatewayServer: IHostedService
    {
        private readonly EndpointDataSource _endpointDataSource;
        private readonly GatewayController _gatewayController;

        public GatewayServer(GatewayController gatewayController, EndpointDataSource endpointDataSource)
        {
            _endpointDataSource = endpointDataSource;
            _gatewayController = gatewayController;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            var routes = RegisterRequest.GetRoutes(_endpointDataSource, new List<Type> { typeof(IGatewayService) });
            
            _gatewayController.SetGatewayRoutes(routes);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
