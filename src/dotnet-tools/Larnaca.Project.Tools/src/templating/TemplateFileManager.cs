using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Larnaca.Project.Tools.Templating
{
    public static class TemplateFileManager
    {
        internal static OperationResult InstallUpdateTemplates(string csproj, string targetDir)
        {
            var templatesRootPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(TemplateFileManager).Assembly.Location), "templates"));
            var csprojFolder = Path.GetDirectoryName(Path.GetFullPath(csproj));
            var targetFolderFullPath = Path.GetFullPath(Path.Combine(csprojFolder, targetDir));
            var rellativeTargetPath = Path.GetRelativePath(csprojFolder, targetFolderFullPath);

            TemplateFile[] templates = Directory.EnumerateFiles(templatesRootPath, "*.lca.tt", SearchOption.AllDirectories)
                .Select(p =>
                {
                    var fullPath = Path.GetFullPath(p);
                    string subPath;
                    if (fullPath.StartsWith(templatesRootPath, StringComparison.OrdinalIgnoreCase))
                    {
                        subPath = fullPath.Substring(templatesRootPath.Length + 1);
                    }
                    else
                    {
                        subPath = Path.GetFileName(p);
                    }
                    string targetFullPath = Path.Combine(targetFolderFullPath, subPath);
                    string targetRelativePath = Path.GetRelativePath(csprojFolder, targetFullPath);
                    return new TemplateFile(fullPath, subPath, targetRelativePath);
                })
                .ToArray();
            var updateCsprojOp = WriteTemplateFilesToCsproj(csproj, templates);
            if (updateCsprojOp.Fail())
            {
                return updateCsprojOp;
            }

            foreach (var currentTemplate in templates)
            {
                // todo: check if newer
                if (currentTemplate.TemplateUpdateTemplateMode != ETemplateUpdateTemplateMode.Newer)
                {
                    var targetPath = Path.GetFullPath(Path.Combine(csprojFolder, currentTemplate.TargetRelativePath));
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                    File.Copy(currentTemplate.SourceFullPath, targetPath, true);
                }
            }
            return new OperationResult();
        }

        private static OperationResult WriteTemplateFilesToCsproj(string csprojFile, TemplateFile[] templates)
        {
            var proj = XDocument.Parse(File.ReadAllText(csprojFile));
            var elements = proj.XPathSelectElements(@"//ItemGroup/LCAT4Template")
                .ToArray();
            Dictionary<string, TemplateFile> templatesByName = templates.ToDictionary(t => Path.GetFileName(t.SourceFullPath), t => t);
            Dictionary<string, TemplateFile> templatesInCsproj = new Dictionary<string, TemplateFile>();
            Dictionary<string, TemplateFile> templatesNotInCsproj = new Dictionary<string, TemplateFile>();

            foreach (var currentElement in elements)
            {
                var updateElement = currentElement.Attribute("Update");
                var inludeElement = currentElement.Attribute("Include");
                var generateSourceElement = currentElement.Attribute("GenerateSource");
                var updateTemplateElement = currentElement.Attribute("UpdateTemplate");

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
                if (templatesByName.TryGetValue(templateName, out var template))
                {
                    template.TemplateGenerateSourceMode = generateSourceMode;
                    template.TemplateUpdateTemplateMode = updateTemplateMode;
                    template.TargetRelativePath = path;
                    templatesInCsproj[templateName] = template;
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
                foreach (var currentTemplate in templatesNotInCsproj.Values)
                {
                    var elementTtemGroup = new XElement("ItemGroup",
                        new XElement("LCAT4Template",
                            new XAttribute("Update", currentTemplate.TargetRelativePath)
                            , new XAttribute("GenerateSource", ETemplateGenerateSourceMode.Build.ToString())
                            , new XAttribute("UpdateTemplate", ETemplateUpdateTemplateMode.Build.ToString())
                            )
                        );
                    projElement.Add(elementTtemGroup);
                }
                File.WriteAllText(csprojFile, proj.ToString());
            }

            return new OperationResult();
        }

        private class TemplateFile
        {
            public string SourceFullPath;
            public string SubPath;
            public string TargetRelativePath;
            public ETemplateGenerateSourceMode TemplateGenerateSourceMode;
            public ETemplateUpdateTemplateMode TemplateUpdateTemplateMode;
            public TemplateFile() { }
            public TemplateFile(string sourceFullPath, string subPath, string targetRelativePath)
            {
                SourceFullPath = sourceFullPath;
                SubPath = subPath;
                TargetRelativePath = targetRelativePath;
                TemplateGenerateSourceMode = default;
                TemplateUpdateTemplateMode = default;
            }
        }

    }
}
