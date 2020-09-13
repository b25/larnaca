using Mono.TextTemplating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Larnaca.Project.Tools.Templating
{
    public static class TemplateFileManager
    {
        internal static OperationResult InstallUpdateTemplates(string csproj, string[] larancaFiles, string[] larnacaFilesPackageIdentities, string targetDir, string outAddedTemplates)
        {
            var templatesRootPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(TemplateFileManager).Assembly.Location), "templates"));
            var csprojFolder = Path.GetDirectoryName(Path.GetFullPath(csproj));
            var targetFolderFullPath = Path.GetFullPath(Path.Combine(csprojFolder, targetDir));
            var rellativeTargetPath = Path.GetRelativePath(csprojFolder, targetFolderFullPath);

            List<TemplateFile> templates = new List<TemplateFile>();
            foreach (var currentTemplate in Directory.EnumerateFiles(templatesRootPath, "*.lca.tt", SearchOption.AllDirectories))
            {
                ETemplateType templateType = ETemplateType.undefined;
                ETemplateReplication templateReplication = ETemplateReplication.undefined;
                bool templateError;
                var fullTemplatePath = Path.GetFullPath(currentTemplate);
                try
                {
                    var templateContent = File.ReadAllText(fullTemplatePath);
                    var generator = new ToolTemplateGenerator();
                    var pt = ParsedTemplate.FromText(templateContent, generator);
                    var extractResult = TemplateLarnacaProperties.Extract(pt);
                    if (extractResult.Fail())
                    {
                        templateError = true;
                        Console.Error.WriteLine($"Failed to extract larnaca properties from template {fullTemplatePath}. {extractResult.StatusMessage}");
                    }
                    else
                    {
                        templateError = false;
                        templateType = extractResult.Data.Type;
                        templateReplication = extractResult.Data.Replication;
                    }

                }
                catch (Exception ex)
                {
                    templateError = true;
                    Console.Error.WriteLine($"Failed to load template {currentTemplate}: {ex}");
                }

                if (templateError)
                {
                    continue;
                }

                string subPath;
                if (fullTemplatePath.StartsWith(templatesRootPath, StringComparison.OrdinalIgnoreCase))
                {
                    subPath = fullTemplatePath.Substring(templatesRootPath.Length + 1);
                }
                else
                {
                    subPath = Path.GetFileName(currentTemplate);
                }

                string targetRelativePath;
                if (templateReplication == ETemplateReplication.Single)
                {
                    // no need to larnaca package subdir
                    string targetFullPath = Path.Combine(targetFolderFullPath, subPath);
                    targetRelativePath = Path.GetRelativePath(csprojFolder, targetFullPath);
                }
                else
                {
                    targetRelativePath = null;
                }

                bool singleTemplateWritten = false;

                if (templateType == ETemplateType.Analysis)
                {
                    if (templateReplication != ETemplateReplication.Single)
                    {
                        Console.Error.WriteLine($"Invalid template {currentTemplate}, cannot have templateType={templateType} and templateReplication={templateReplication}");
                    }
                    else
                    {
                        templates.Add(new TemplateFile(fullTemplatePath, subPath, targetRelativePath, "none"));
                    }
                }
                else
                {
                    for (int i = 0; i < larancaFiles.Length; i++)
                    {
                        string currentLarnacaFile = larancaFiles[i];
                        string currentLarnacaPackageId = larnacaFilesPackageIdentities[i];

                        var loadedFile = DeserializedLarancaFile.Load(currentLarnacaFile);

                        if (loadedFile.Fail())
                        {
                            Console.Error.WriteLine($"Failed to load larnaca file ({currentLarnacaFile}): {loadedFile.StatusMessage}");
                        }
                        else
                        {
                            if (loadedFile.Data.DatabaseMeta != null && templateType == ETemplateType.DB)
                            {
                                if (templateReplication == ETemplateReplication.Project)
                                {
                                    var targetLarnacaPackageSubdir = Path.Combine(targetFolderFullPath, currentLarnacaPackageId);
                                    string targetFullPath = Path.Combine(targetLarnacaPackageSubdir, subPath);
                                    targetRelativePath = Path.GetRelativePath(csprojFolder, targetFullPath);
                                    templates.Add(new TemplateFile(fullTemplatePath, subPath, targetRelativePath, currentLarnacaPackageId));
                                }
                                else
                                {
                                    if (!singleTemplateWritten)
                                    {
                                        singleTemplateWritten = true;
                                        templates.Add(new TemplateFile(fullTemplatePath, subPath, targetRelativePath, "none"));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var updateCsprojOp = WriteTemplateFilesToCsproj(csproj, templates);
            if (updateCsprojOp.Fail())
            {
                return updateCsprojOp;
            }

            foreach (var currentTemplate in templates)
            {
                // todo: check if newer
                if (currentTemplate.TemplateUpdateTemplateMode != ETemplateUpdateTemplateMode.None)
                {
                    var targetPath = Path.GetFullPath(Path.Combine(csprojFolder, currentTemplate.TargetRelativePath));
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                    File.Copy(currentTemplate.SourceFullPath, targetPath, true);
                }
            }

            if (!string.IsNullOrWhiteSpace(outAddedTemplates))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outAddedTemplates));
                File.WriteAllText(outAddedTemplates, string.Join(Environment.NewLine, updateCsprojOp.Data.Values.Select(t => t.TargetRelativePath)));
            }

            return new OperationResult();
        }

        private static OperationResult<Dictionary<(string templateName, string packageId), TemplateFile>> WriteTemplateFilesToCsproj(string csprojFile, List<TemplateFile> templates)
        {
            var proj = XDocument.Parse(File.ReadAllText(csprojFile));
            var elements = proj.XPathSelectElements(@"//ItemGroup/LCAT4Template")
                .ToArray();
            Dictionary<(string templateName, string packageId), TemplateFile> templatesByName = templates.ToDictionary(t => (templateName: Path.GetFileName(t.SourceFullPath), t.LarnacaPackageIdentity), t => t);
            Dictionary<(string templateName, string packageId), TemplateFile> templatesInCsproj = new Dictionary<(string templateName, string packageId), TemplateFile>();
            Dictionary<(string templateName, string packageId), TemplateFile> templatesNotInCsproj = new Dictionary<(string templateName, string packageId), TemplateFile>();

            foreach (var currentElement in elements)
            {
                var updateElement = currentElement.Attribute("Update");
                var inludeElement = currentElement.Attribute("Include");
                var generateSourceElement = currentElement.Attribute("GenerateSource");
                var updateTemplateElement = currentElement.Attribute("UpdateTemplate");
                var pacakgeIdElement = currentElement.Attribute("PackageIdentity");
                if (!string.IsNullOrWhiteSpace(pacakgeIdElement?.Value))
                {
                    var pacakgeId = pacakgeIdElement.Value;
                    ETemplateGenerateSourceMode generateSourceMode;
                    ETemplateUpdateTemplateMode updateTemplateMode;
                    if (generateSourceElement == null || !Enum.TryParse(generateSourceElement.Value, out generateSourceMode))
                    {
                        generateSourceMode = default;
                    }

                    if (updateTemplateElement == null || !Enum.TryParse(updateTemplateElement.Value, out updateTemplateMode))
                    {
                        updateTemplateMode = default;
                    }


                    var path = (updateElement ?? inludeElement).Value;
                    var templateName = Path.GetFileName(path);
                    var key = (templateName, pacakgeId);
                    if (templatesByName.TryGetValue(key, out var template))
                    {
                        template.TemplateGenerateSourceMode = generateSourceMode;
                        template.TemplateUpdateTemplateMode = updateTemplateMode;
                        template.TargetRelativePath = path;
                        templatesInCsproj[key] = template;
                    }
                }
            }

            foreach (var currentTemplate in templatesByName)
            {
                if (!templatesInCsproj.ContainsKey(currentTemplate.Key))
                {
                    templatesNotInCsproj[currentTemplate.Key] = currentTemplate.Value;
                }
            }

            if (templatesNotInCsproj.Any())
            {
                var projElement = proj.XPathSelectElement("Project");
                List<XElement> templateElements = new List<XElement>();
                foreach (var currentTemplate in templatesNotInCsproj.Values)
                {
                    templateElements.Add(new XElement("LCAT4Template",
                            new XAttribute("Update", currentTemplate.TargetRelativePath)
                            , new XAttribute("GenerateSource", ETemplateGenerateSourceMode.Build.ToString())
                            , new XAttribute("UpdateTemplate", ETemplateUpdateTemplateMode.Build.ToString())
                            , new XAttribute("PackageIdentity", currentTemplate.LarnacaPackageIdentity))
                        );
                }
                var elementTtemGroup = new XElement("ItemGroup", templateElements.ToArray());
                projElement.Add(elementTtemGroup);
                File.WriteAllText(csprojFile, proj.ToString());
            }

            return templatesNotInCsproj.ToOperationResult();
        }

        private class TemplateFile
        {
            public string SourceFullPath;
            public string SubPath;
            public string TargetRelativePath;
            public ETemplateGenerateSourceMode TemplateGenerateSourceMode;
            public ETemplateUpdateTemplateMode TemplateUpdateTemplateMode;
            public string LarnacaPackageIdentity;
            public TemplateFile() { }
            public TemplateFile(string sourceFullPath, string subPath, string targetRelativePath, string larnacaPackageIdentity)
            {
                SourceFullPath = sourceFullPath;
                SubPath = subPath;
                TargetRelativePath = targetRelativePath;
                LarnacaPackageIdentity = larnacaPackageIdentity;
                TemplateGenerateSourceMode = default;
                TemplateUpdateTemplateMode = default;
            }
        }

    }
}
