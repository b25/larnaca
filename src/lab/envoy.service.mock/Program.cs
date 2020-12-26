using System;
using System.IO;
using System.Linq;
using System.Net;
using envoy.service.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
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
                        // Support --port and --port_http cmdline arguments normally supported
                        // by gRPC interop servers.
                        var httpsPort = context.Configuration.GetValue<int>("port", 50051);
                        var httpPort = context.Configuration.GetValue<int>("port_http", httpsPort - 1);

                        options.Limits.MinRequestBodyDataRate = null;
                        options.Listen(new IPEndPoint(IPAddress.Any, httpsPort), o => ConfigureEndpoint(o, true));
                        if (httpPort != -1)
                        {
                            options.Listen(new IPEndPoint(IPAddress.Any, httpPort), o => ConfigureEndpoint(o, false));
                        }

                        void ConfigureEndpoint(ListenOptions listenOptions, bool useTls)
                        {
                            if (useTls)
                            {
                                var basePath = Directory.GetCurrentDirectory();
                                var certPath = Path.Combine(basePath, "cert.pfx");

                                if (File.Exists(certPath))
                                {
                                    // use custom certificate
                                    listenOptions.UseHttps(certPath);
                                }
                                else
                                {
                                    listenOptions.UseHttps();
                                }
                            }
                            listenOptions.Protocols = HttpProtocols.Http2;
                        }
                    });
                    webBuilder.UseStartup<Startup>();
                }
                );
    }
}
