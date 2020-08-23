using Mono.TextTemplating;
using mssql.collector.types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Larnaca.Project.Tools.Templating
{
    public static class TemplateGenerationHelper
    {
        public static int GenerateSources(string folder, string[] templates, string[] larancaFiles)
        {
            int theReturn = 0;
            var collectorTypesAsm = typeof(mssql.collector.types.DatabaseMeta).Assembly.Location;
            var genUtilsAsm = typeof(gen.utils.DalUtils).Assembly.Location;
            var newtonsoftAsm = typeof(Newtonsoft.Json.JsonConvert).Assembly.Location;
            var dbMeta = Newtonsoft.Json.JsonConvert.DeserializeObject<DatabaseMeta>(File.ReadAllText(larancaFiles.First()));

            foreach (var templateFile in templates)
            {
                var generator = new ToolTemplateGenerator();
                var inputFile = templateFile;
                string inputContent;

                try
                {
                    inputContent = File.ReadAllText(inputFile);
                }
                catch (IOException ex)
                {
                    Console.Error.WriteLine("Could not read input file '" + inputFile + "':\n" + ex);
                    return 1;
                }

                if (inputContent.Length == 0)
                {
                    Console.Error.WriteLine("Input is empty");
                    return 1;
                }

                var pt = ParsedTemplate.FromText(inputContent, generator);
                var settings = TemplatingEngine.GetSettings(generator, pt);
                settings.Log = Console.Out;

                if (pt.Errors.Count > 0)
                {
                    generator.Errors.AddRange(pt.Errors);
                }

                var outputFile = inputFile;
                if (Path.HasExtension(outputFile))
                {
                    var dir = Path.GetDirectoryName(outputFile);
                    var fn = Path.GetFileNameWithoutExtension(outputFile);
                    outputFile = Path.Combine(dir, fn + (settings.Extension ?? ".txt"));
                }
                else
                {
                    outputFile = outputFile + (settings.Extension ?? ".txt");
                }

                HashSet<string> assemblyNamesToRemove = new HashSet<string>(new[]
                {
                    Path.GetFileName(collectorTypesAsm),
                    Path.GetFileName(genUtilsAsm),
                    Path.GetFileName(newtonsoftAsm),
                }, StringComparer.OrdinalIgnoreCase);
                //fix template assemblies path
                foreach (var x in settings.Assemblies.ToArray())
                {
                    if (assemblyNamesToRemove.Contains(Path.GetFileName(x)))
                    {
                        settings.Assemblies.Remove(x);
                    }
                    else
                    {
                        settings.Assemblies.Add(FixPath(x, folder));
                    }

                }
                settings.Assemblies.Add(collectorTypesAsm);
                settings.Assemblies.Add(genUtilsAsm);
                settings.Assemblies.Add(newtonsoftAsm);

                string outputContent = null;
                if (!generator.Errors.HasErrors)
                {
                    generator.AddParameter(null, null, "dbMeta", dbMeta);
                    outputContent = generator.ProcessTemplate(pt, inputFile, inputContent, ref outputFile, settings);
                }

                if (!generator.Errors.HasErrors)
                {
                    try
                    {
                        if (outputFile.EndsWith(".g.g.cs", StringComparison.OrdinalIgnoreCase))
                        {
                            outputFile = outputFile.Substring(0, outputFile.Length - ".g.g.cs".Length) + ".g.cs";
                        }
                        File.WriteAllText(outputFile, outputContent, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                    }
                    catch (IOException ex)
                    {
                        Console.Error.WriteLine("Could not write output file '" + outputFile + "':\n" + ex);
                        theReturn++;
                    }
                }
                else
                {
                    Console.Error.WriteLine(inputFile == null ? "Processing failed." : $"Processing '{inputFile}' failed.");
                    theReturn++;
                }
            }

            return theReturn;
        }
        static string FixPath(string assemblyPath, string binFolder)
        {
            if (assemblyPath.IndexOf("$(SolutionDir)", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                var lastSlash = assemblyPath.LastIndexOf("\\") + 1;
                return $"{binFolder}{assemblyPath.Substring(lastSlash, assemblyPath.Length - lastSlash)}";
            }

            return assemblyPath;
        }
    }
}
