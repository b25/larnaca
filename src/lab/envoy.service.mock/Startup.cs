using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProtoBuf.Grpc.Server;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace envoy.service
{
    public class Startup
    {
        public readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ServiceOptions>(_configuration.GetSection(nameof(ServiceOptions)));

            services
              .AddOptions()
              .AddSingleton<TestService>()
              .AddMvcCore()
              .AddFormatterMappings()
              .AddCors(o => o.AddPolicy("AllowAll", builder =>
              {
                  builder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader()
                         .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
              }));

            services.AddGrpcHealthChecks()
              .AddCheck("", () => HealthCheckResult.Healthy(), Array.Empty<string>());

            services.AddCodeFirstGrpc(config =>
            {
                config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
            });

            services.AddCodeFirstGrpcReflection();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            var product = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync($"{product} v{version}");
                });

                // health checks
                endpoints.MapHealthChecks("/health");
                endpoints.MapGrpcHealthChecksService();

                endpoints.MapGrpcService<TestService>();
                endpoints.MapCodeFirstGrpcReflectionService();
            });
        }
    }
}
