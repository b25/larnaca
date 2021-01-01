using System;
using System.IO;
using System.Linq;
using envoy.contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace envoy.gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("--proto"))
            {
                var generator = new ProtoBuf.Grpc.Reflection.SchemaGenerator();
                var schema = generator.GetSchema<IGatewayService>();
                var path = Path.Join(Directory.GetCurrentDirectory(), "protos", "service.proto");

                File.WriteAllText(path, schema);

                Console.WriteLine($"Proto definitions dumped to {path}");
            }
            else
            {
                CreateHostBuilder(args).Build().Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        options.ConfigureEndpointDefaults(listenOptions =>
                        {
                            // support grpc on http protocol
                            listenOptions.Protocols = HttpProtocols.Http2;
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                }
                )
                // Register the GatewayServer AFTER ConfigureWebHostDefaults to have access to all registered endpoints
                .ConfigureServices(services => services.AddHostedService<GatewayServer>());
    }
}
