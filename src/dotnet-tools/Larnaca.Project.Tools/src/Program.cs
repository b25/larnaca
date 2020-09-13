using Larnaca.Project.Tools.Packaging;
using Larnaca.Project.Tools.Templating;
using mssql.collector.types;
using System;
using System.Collections.Generic;
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
args: { string.Join(" ", args)}
            ");

            var rootCommand = new RootCommand("Generate code from templates");
            rootCommand.Add(new Option<string>("--templates", "Semicolon (;) separated list of T4 Template files for which to generate code") { IsRequired = true });
            rootCommand.Add(new Option<string>("--templatePackageIds", "Semicolon (;) separated list of pacakge identities of template files") { IsRequired = true });
            rootCommand.Add(new Option<string>("--larnacaFiles", "Semicolon (;) separated list of larnaca source files") { IsRequired = true });
            rootCommand.Add(new Option<string>("--larnacaFilesPackageIdentities", "Semicolon (;) separated list of pacakge identities of larnaca source files") { IsRequired = true });
            rootCommand.Add(new Option<string>("--dir", () => Environment.CurrentDirectory, "Working dir") { IsRequired = false });
            rootCommand.Add(new Option<string>("--projFile", "the .csproj file name") { IsRequired = true });
            rootCommand.Add(new Option<string>("--outCsSourcesToCompile", () => "outCsSourcesToCompile.txt", "Output file where the list of cs source files generated from templates that need to be compiled will be written") { IsRequired = false });
            rootCommand.Add(new Option<string>("--outAnalysisProject", () => "outAnalysisProject.txt", "Output file where the ananlysis csproj file generated from analysys template will be written") { IsRequired = false });
            var methodInfo = typeof(Program).GetMethod(nameof(GenerateCodeFromTemplatesCommand));
            rootCommand.Handler = CommandHandler.Create(methodInfo);

            var generateNugetCommand = new Command("package", "Generate nuget package");
            generateNugetCommand.Add(new Option<string>("--resultFile", "Source file for which to generate the nuget") { IsRequired = true });
            generateNugetCommand.Add(new Option<string>("--packageId", "Nuget package name/id") { IsRequired = true });
            generateNugetCommand.Add(new Option<string>("--version", "Nuget version") { IsRequired = true });
            generateNugetCommand.Add(new Option<string>("--authors", "Nuget authors") { IsRequired = false });
            generateNugetCommand.Add(new Option<string>("--owners", "Nuget owners") { IsRequired = false });
            generateNugetCommand.Add(new Option<string>("--description", "Nuget description") { IsRequired = false });
            generateNugetCommand.Add(new Option<string>("--outputFolder", () => Environment.CurrentDirectory, "Output folder"));
            generateNugetCommand.Handler = CommandHandler.Create<string, string, string, string, string, string, string>(GenerateNugetFromFile);
            rootCommand.AddCommand(generateNugetCommand);

            var installTemplatesCommand = new Command("install-templates", "Install/update templates");
            installTemplatesCommand.Add(new Option<string>(new string[] { "--csproj", "--csprojFile" }, () => Directory.EnumerateFiles(Environment.CurrentDirectory, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault(), "the .csproj file"));
            installTemplatesCommand.Add(new Option<string>("--larnacaFiles", "Semicolon (;) separated list of larnaca source files") { IsRequired = true });
            installTemplatesCommand.Add(new Option<string>("--larnacaFilesPackageIdentities", "Semicolon (;) separated list of pacakge identities of larnaca source files") { IsRequired = true });
            installTemplatesCommand.Add(new Option<string>(new string[] { "--dir", "--targetDir" }, () => "obj/templates", "the directory where the templates should be written, relative to the .csproj file"));
            installTemplatesCommand.Add(new Option<string>("--outAddedTemplates", () => "outAddedTemplates.txt", "Output file where the list of added template files will be written"));
            installTemplatesCommand.Handler = CommandHandler.Create<string, string, string, string, string>(InstallTemplates);
            rootCommand.Add(installTemplatesCommand);


            return rootCommand.InvokeAsync(args);
        }

        private static int InstallTemplates(string csproj, string larnacaFiles, string larnacaFilesPackageIdentities, string dir, string outAddedTemplates)
        {
            var larnacaFilesArray = larnacaFiles.Split(';').Select(p => Path.Combine(dir, p)).ToArray();
            var larnacaFilesPackageIdentitiesArray = larnacaFilesPackageIdentities.Split(';');
            var result = TemplateFileManager.InstallUpdateTemplates(csproj, larnacaFilesArray, larnacaFilesPackageIdentitiesArray, dir, outAddedTemplates);
            if (result.Fail())
            {
                Console.Error.WriteLine(result);
            }
            else
            {
                Console.WriteLine(result);
            }
            return result.StatusCode;
        }

        public static Task<int> GenerateCodeFromTemplatesCommand(
            string templates,
            string templatePackageIds,
            string larnacaFiles,
            string larnacaFilesPackageIdentities,
            string dir,
            string projFile,
            string outCsSourcesToCompile,
            string outAnalysisProject)
        {
            var templatesArray = templates.Split(';')
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .Select(p => Path.Combine(dir, p))
                .ToArray();
            var larnacaFilesArray = larnacaFiles.Split(';')
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .Select(p => Path.Combine(dir, p))
                .ToArray();
            var templatePackageIdsArray = templatePackageIds.Split(';')
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .ToArray();
            var larnacaFilesPackageIdentitiesArray = larnacaFilesPackageIdentities.Split(';')
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .ToArray();

            bool returnEarly = false;
            bool returnError = false;
            if (templatesArray.Length == 0)
            {
                returnEarly = true;
                Console.WriteLine("No templates to compile");
            }

            if (larnacaFilesArray.Length == 0)
            {
                returnEarly = true;
                Console.WriteLine("No Larnaca files to use for compiling");
            }

            if (templatesArray.Length != templatePackageIdsArray.Length)
            {
                returnEarly = true;
                returnError = true;
                Console.Error.WriteLine($"Mismatch count between templates ({templatesArray.Length}) and templatePackageIds ({templatePackageIdsArray.Length})");
            }

            if (larnacaFilesArray.Length != larnacaFilesPackageIdentitiesArray.Length)
            {
                returnEarly = true;
                returnError = true;
                Console.Error.WriteLine($"Mismatch count between larnacaFiles ({larnacaFilesArray.Length}) and larnacaFilesPackageIdentities ({larnacaFilesPackageIdentitiesArray.Length})");
            }

            if (returnEarly)
            {
                if (!string.IsNullOrWhiteSpace(outCsSourcesToCompile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outCsSourcesToCompile));
                    File.WriteAllText(outCsSourcesToCompile, string.Empty);
                }

                if (returnError)
                {
                    return Task.FromResult(1);
                }
                else
                {
                    return Task.FromResult(0);
                }
            }

            return TemplateGenerationHelper.GenerateSources(
                dir,
                templatesArray,
                templatePackageIdsArray,
                larnacaFilesArray,
                larnacaFilesPackageIdentitiesArray,
                projFile,
                outCsSourcesToCompile,
                outAnalysisProject);
        }

        private static async Task<int> GenerateNugetFromFile(
            string resultFile,
            string packageId,
            string version,
            string authors,
            string owners,
            string description,
            string outputFolder)
        {
            var result = await Packager.GenerateNugetPackage(
                resultFile,
                packageId,
                version,
                authors,
                owners,
                description,
                outputFolder);
            int theReturn = result.StatusCode;
            TextWriter output;
            if (result.Fail())
            {
                output = Console.Error;
            }
            else
            {
                output = Console.Out;
            }
            output.WriteLine(result.ToString());
            return theReturn;
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
