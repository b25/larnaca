using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace mssql.collector
{
    internal class Program
    {
        private static Task<int> Main(string[] args)
        {
            var toolDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var workingDirectory = Directory.GetCurrentDirectory();
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            Console.WriteLine($@"
{asciiart}
tool directory: { toolDirectory }
work directory: { workingDirectory }
tool version  : {version.Major}.{version.Minor}.{version.Build}
            ");

            var service = GetService(workingDirectory);
            var rootCommand = new RootCommand {
                new Option("--config-file")
                {
                    Argument = new Argument<string>()
                }
            };

            rootCommand.Description = "ananke mssql collector";

            rootCommand.Handler = CommandHandler.Create<string>(async (configFile) =>
            {
                var resp = await service.WriteDatabaseMetaToFile(workingDirectory);

                Console.WriteLine(resp.StatusMessage);
                if (resp.Fail())
                {
                    Environment.Exit(resp.StatusCode);
                }
            });

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args);
        }

        private static SqlCollectorService GetService(string workingDirectory)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(workingDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<SqlCollectorServiceOptions>();
            var configuration = builder.Build();

            var services = new ServiceCollection()
            .Configure<SqlCollectorServiceOptions>(configuration.GetSection(nameof(SqlCollectorServiceOptions)))
            .AddOptions()
            .AddSingleton<SqlCollectorService>()
            .BuildServiceProvider(true);

            return services.GetRequiredService<SqlCollectorService>();
        }

        private static string asciiart = @"
 __  __ ____ ____   ___  _           ____      _ _           _
|  \/  / ___/ ___| / _ \| |         / ___|___ | | | ___  ___| |_ ___  _ __
| |\/| \___ \___ \| | | | |   _____| |   / _ \| | |/ _ \/ __| __/ _ \| '__|
| |  | |___) |__) | |_| | |__|_____| |__| (_) | | |  __/ (__| || (_) | |
|_|  |_|____/____/ \__\_\_____|     \____\___/|_|_|\___|\___|\__\___/|_|
";
    }
}