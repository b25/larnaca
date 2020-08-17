using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Larnaca.Project.Tools
{
    internal class Program
    {
        private static Task<int> Main(string[] args)
        {
            var toolDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var workingDirectory = Environment.CurrentDirectory;
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            Console.WriteLine($@"
{asciiart}
tool directory: { toolDirectory }
work directory: { workingDirectory }
tool version  : { version.ToString(3) }
args: { string.Join(",", args)}
            ");

            var rootCommand = new RootCommand("Generate code from templates");
            rootCommand.Add(new Option<string>("--templates", "Semicolon (;) separated list of T4 Template files for which to generate code") { IsRequired = true });
            rootCommand.Handler = CommandHandler.Create<string>(GenerateCodeFromTemplatesCommand);
            return rootCommand.InvokeAsync(args);
        }

        private static int GenerateCodeFromTemplatesCommand(string templates)
        {
            var dir = @"D:\Dev\lca\src\lab\sandbox.larnaca.project";
            var templatesArray = templates.Split(';').Select(p => Path.Combine(dir, p)).ToArray();
            return TemplatingHelper.GenerateSources(dir, templatesArray);
        }

        // to update use: http://patorjk.com/software/taag/#p=display&f=Rectangles&t=Larnaca%20%0AProject%20Tools
        private const string asciiart = @"                                                      
 __                                                   
|  |   ___ ___ ___ ___ ___ ___                        
|  |__| .'|  _|   | .'|  _| .'|                       
|_____|__,|_| |_|_|__,|___|__,|                       
                                                      
                                                      
 _____           _         _      _____         _     
|  _  |___ ___  |_|___ ___| |_   |_   _|___ ___| |___ 
|   __|  _| . | | | -_|  _|  _|    | | | . | . | |_ -|
|__|  |_| |___|_| |___|___|_|      |_| |___|___|_|___|
              |___|                                   
";
    }
}
