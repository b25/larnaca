using System;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace mssql.adapter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("--proto"))
            {
                DalHelper.CreateProto();
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
                        var httpsPort = context.Configuration.GetValue<int>("port", 5001);
                        var httpPort = context.Configuration.GetValue<int>("port_http", 5000);

                        options.Limits.MinRequestBodyDataRate = null;
                        options.Listen(new IPEndPoint(IPAddress.Any, httpsPort), o => ConfigureEndpoint(o, true));
                        if (httpPort != -1)
                        {
                            options.Listen(new IPEndPoint(IPAddress.Any, httpPort), o => ConfigureEndpoint(o, false));
                        }

                        void ConfigureEndpoint(ListenOptions listenOptions, bool useTls)
                        {
                            Console.WriteLine($"Enabling connection encryption: {useTls}");

                            if (useTls)
                            {
                                var basePath = Directory.GetCurrentDirectory();
                                var certPath = Path.Combine(basePath, "cert.pfx");

                                if (File.Exists(certPath))
                                {
                                    // use custom certificate
                                    listenOptions.UseHttps(certPath);
                                } else
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
