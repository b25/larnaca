using Mono.TextTemplating;
using mssql.collector.types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Larnaca.Project.Tools
{
    public static class TemplatingHelper
    {
        public static int GenerateSources(string folder, string[] templates)
        {
            int theReturn = 0;
            var collectorTypesAsm = typeof(mssql.collector.types.DatabaseMeta).Assembly.Location;
            var genUtilsAsm = typeof(gen.utils.DalUtils).Assembly.Location;
            var dbMeta = new DatabaseMeta()
            {
                Name = "TestDb",
                Procedures = new ProcedureMeta[]
                {
                    new ProcedureMeta()
                    {
                        SpName = "TestSP",
                        Request = new List<ParamMeta>()
                        {
                            new ParamMeta()
                            {
                                Name = "TestParam",
                                Order = 1,
                                SqlType = "int"
                            }
                        },
                        Responses = new List<ResponseItem>()
                        {
                            new ResponseItem()
                            {
                                Name = "TestResponse",
                                Order = 1,
                                Params = new List<ParamMeta>()
                                {
                                    new ParamMeta()
                                    {
                                        Name = "TestParam",
                                        Order = 1,
                                        SqlType = "int"
                                    }
                                }
                            }
                        }
                    }
                }
            };


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

                var outputFile = inputFile;
                if (Path.HasExtension(outputFile))
                {
                    var dir = Path.GetDirectoryName(outputFile);
                    var fn = Path.GetFileNameWithoutExtension(outputFile);
                    outputFile = Path.Combine(dir, fn + ".cs");
                }
                else
                {
                    outputFile = outputFile + ".txt";
                }

                var pt = ParsedTemplate.FromText(inputContent, generator);
                var settings = TemplatingEngine.GetSettings(generator, pt);
                settings.Log = Console.Out;

                if (pt.Errors.Count > 0)
                {
                    generator.Errors.AddRange(pt.Errors);
                }
                //fix template assemblies path
                foreach (var x in settings.Assemblies)
                {
                    settings.Assemblies.Add(FixPath(x, folder));
                }
                settings.Assemblies.Add(collectorTypesAsm);
                settings.Assemblies.Add(genUtilsAsm);

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
