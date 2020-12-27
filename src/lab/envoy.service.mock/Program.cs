using System;
using System.IO;
using System.Linq;
using envoy.service.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace envoy.service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("--proto"))
            {
                var generator = new ProtoBuf.Grpc.Reflection.SchemaGenerator();
                var schema = generator.GetSchema<ITestService>();
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
                // Register the GatewayClient AFTER ConfigureWebHostDefaults to have access to the listening addresses
                .ConfigureServices(services => services.AddHostedService<GatewayClient>());
    }
}
