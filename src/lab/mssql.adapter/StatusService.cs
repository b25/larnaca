using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Health.V1;
using Grpc.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace mssql.adapter
{
    /// <summary>
    /// Background status service used to set the correct health status for the gRPC service.
    /// </summary>
    public class StatusService : BackgroundService
    {
        private readonly HealthServiceImpl _healthService;
        private readonly HealthCheckService _healthCheckService;

        /// <summary>
        /// Instanciates the service with the health service singleton used to set the current grpc health status
        /// and the asp health check service which can be used to check the status of Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
        /// instances registered in the application.
        /// </summary>
        /// <param name="healthService"></param>
        /// <param name="healthCheckService"></param>
        public StatusService(HealthServiceImpl healthService, HealthCheckService healthCheckService)
        {
            _healthService = healthService;
            _healthCheckService = healthCheckService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var health = await _healthCheckService.CheckHealthAsync(stoppingToken);

                _healthService.SetStatus(string.Empty,
                    health.Status == HealthStatus.Healthy
                        ? HealthCheckResponse.Types.ServingStatus.Serving
                        : HealthCheckResponse.Types.ServingStatus.NotServing);

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
