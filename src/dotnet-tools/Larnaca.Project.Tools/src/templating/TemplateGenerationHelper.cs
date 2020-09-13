using Mono.TextTemplating;
using mssql.collector.types;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Larnaca.Project.Tools.Templating
{
    public static class TemplateGenerationHelper
    {
        public static async Task<int> GenerateSources(
            string folder,
            string[] templates,
            string[] templatePackageIds,
            string[] larnacaFiles,
            string[] larnacaFilesPackageIdentities,
            string projFile,
            string outCsSourcesToCompile,
            string outAnalysisProject
            )
        {
            string collectorTypesAsm = typeof(DatabaseMeta).Assembly.Location;
            string genUtilsAsm = typeof(gen.utils.DalUtils).Assembly.Location;
            string newtonsoftAsm = typeof(Newtonsoft.Json.JsonConvert).Assembly.Location;

            List<Task<TemplateGenerationResult>> allTasks = new List<Task<TemplateGenerationResult>>();
            Dictionary<string, DeserializedLarancaFile> larnacaFilesByPackageid = new Dictionary<string, DeserializedLarancaFile>();

            for (int i = 0; i < larnacaFiles.Length; i++)
            {
                var tryLoad = DeserializedLarancaFile.Load(larnacaFiles[i]);
                if (tryLoad.Fail())
                {
                    Console.Error.WriteLine($"Failed to deserialize laranca file {larnacaFiles[i]}: {tryLoad.StatusMessage}");
                }
                else
                {
                    larnacaFilesByPackageid[larnacaFilesPackageIdentities[i]] = tryLoad.Data;
                }
            }

            for (int i = 0; i < templates.Length; i++)
            {
                var currentTemplate = templates[i];
                var currentTemplatePackageId = templatePackageIds[i];

                DeserializedLarancaFile larnacaFile = null;
                if (currentTemplatePackageId.Equals("none", StringComparison.OrdinalIgnoreCase) || larnacaFilesByPackageid.TryGetValue(currentTemplatePackageId, out larnacaFile))
                {
                    allTasks.Add(Task.Run(() => GenerateTemplate(currentTemplate, larnacaFile, folder, collectorTypesAsm, genUtilsAsm, newtonsoftAsm, projFile)));
                }
                else
                {
                    Console.Error.WriteLine($"Failed to find larnaca file {currentTemplatePackageId} for template {currentTemplate}");
                }
            }

            var allResults = await Task.WhenAll(allTasks).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(outCsSourcesToCompile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outCsSourcesToCompile));
                File.WriteAllText(outCsSourcesToCompile, string.Join(Environment.NewLine, allResults.Where(r => !string.IsNullOrWhiteSpace(r.CSFileToCompile)).Select(r => r.CSFileToCompile)));
            }

            if (!string.IsNullOrWhiteSpace(outAnalysisProject))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outAnalysisProject));
                var analysisProject = allResults.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.AnalysisProjectFileToBuild))?.AnalysisProjectFileToBuild ?? "";
                File.WriteAllText(outAnalysisProject, analysisProject);
            }

            return allResults.Max(r => r.StatusCode);
        }
        private static TemplateGenerationResult GenerateTemplate(
            string templateFile,
            DeserializedLarancaFile larancaFile,
            string folder,
            string collectorTypesAsm,
            string genUtilsAsm,
            string newtonsoftAsm,
            string projFile)
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
                return new TemplateGenerationResult() { StatusCode = 1 };
            }

            if (inputContent.Length == 0)
            {
                Console.Error.WriteLine("Input is empty");
                return new TemplateGenerationResult() { StatusCode = 1 };
            }

            var pt = ParsedTemplate.FromText(inputContent, generator);
            var larnacaPropertiesExtractResult = TemplateLarnacaProperties.Extract(pt);
            if (larnacaPropertiesExtractResult.Fail())
            {
                Console.Error.WriteLine($"Failed to parse larnaca propertsions of template: {templateFile}. {larnacaPropertiesExtractResult.StatusMessage}");
            }
            var settings = TemplatingEngine.GetSettings(generator, pt);
            settings.Log = Console.Out;

            if (pt.Errors.Count > 0)
            {
                foreach (var currentError in pt.Errors)
                {
                    var currentCompilerError = (CompilerError)currentError;
                    if (currentCompilerError.IsWarning)
                    {
                        Console.WriteLine(currentCompilerError.ToString());
                    }
                    else
                    {
                        Console.Error.WriteLine(currentCompilerError.ToString());
                        generator.Errors.Add(currentCompilerError);
                    }
                }
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
                var larnacaDirective = pt.Directives.FirstOrDefault(d => d.Name.Equals("larnaca", StringComparison.OrdinalIgnoreCase));
                if (larancaFile?.DatabaseMeta != null)
                {
                    generator.AddParameter(null, null, "dbMeta", larancaFile.DatabaseMeta);
                }
                generator.AddParameter(null, null, "projFile", projFile);

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

                    if (outputFile.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    {
                        if (larnacaPropertiesExtractResult.Data?.DoNotCompile ?? false)
                        {
                            return new TemplateGenerationResult();
                        }
                        else
                        {
                            return new TemplateGenerationResult() { CSFileToCompile = outputFile };
                        }
                    }
                    else if (outputFile.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                    {
                        if ((larnacaPropertiesExtractResult.Data?.Type ?? ETemplateType.undefined) == ETemplateType.Analysis)
                        {
                            return new TemplateGenerationResult() { AnalysisProjectFileToBuild = outputFile };
                        }
                        else
                        {
                            return new TemplateGenerationResult();
                        }
                    }
                    else
                    {
                        return new TemplateGenerationResult();
                    }
                }
                catch (IOException ex)
                {
                    Console.Error.WriteLine("Could not write output file '" + outputFile + "':\n" + ex);
                    return new TemplateGenerationResult() { StatusCode = 1 };
                }
            }
            else
            {
                Console.Error.WriteLine(inputFile == null ? "Processing failed." : $"Processing '{inputFile}' failed.");
                return new TemplateGenerationResult() { StatusCode = 1 };
            }
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

        private class TemplateGenerationResult : IOperationResult
        {
            public int StatusCode { get; set; }
            public string StatusMessage { get; set; }
            public string CSFileToCompile { get; set; }
            public string AnalysisProjectFileToBuild { get; set; }
        }
    }
}
