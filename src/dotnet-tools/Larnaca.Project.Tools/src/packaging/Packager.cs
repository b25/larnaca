using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Larnaca.Project.Tools.Packaging
{
    public static class Packager
    {
        internal static async Task<OperationResult> GenerateNugetPackage(
            string resultFile,
            string packageId,
            string version,
            string authors,
            string owners,
            string description,
            string outputFolder)
        {
            Directory.CreateDirectory(outputFolder);
            var tempDirectoryPath = Path.GetTempPath();
            Directory.CreateDirectory(tempDirectoryPath);
            var resultFileName = Path.GetFileName(resultFile);
            string targetsFile = Path.Combine(tempDirectoryPath, $"{packageId}.targets");
            var targetsContents = $@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <LarnacaFiles Include=""$(MSBuildThisFileDirectory)..\contentFiles\**\{resultFileName}"">
        <Version>{version}</Version>
        <PackageIdentity>{packageId}</PackageIdentity>
    </LarnacaFiles>
  </ItemGroup>
</Project>
";
            File.WriteAllText(targetsFile, targetsContents);
            ManifestMetadata metadata = new ManifestMetadata()
            {
                Authors = new string[] { authors },
                Owners = new string[] { owners },
                Version = new NuGetVersion(version),
                Id = packageId,
                Description = description,
                ContentFiles = new List<ManifestContentFiles>() { new ManifestContentFiles()
                {
                    Include = "**/obj/*.*",
                    BuildAction = "none"
                } },
                DependencyGroups = new PackageDependencyGroup[] { new PackageDependencyGroup(NuGet.Frameworks.NuGetFramework.AnyFramework, new PackageDependency[] { new PackageDependency("protobuf-net", new VersionRange(new NuGetVersion("2.4.2"))) }) }
            };

            PackageBuilder builder = new PackageBuilder();
            builder.PopulateFiles("", new ManifestFile[]
            {
                new ManifestFile()
                {
                    Source = resultFile,
                    Target = Path.Combine("contentFiles", "any", "any", "obj", resultFileName)
                },
                new ManifestFile()
                {
                    Source = targetsFile,
                    Target = Path.Combine("build", $"{packageId}.targets")
                }
            });
            builder.Populate(metadata);

            var nugetFileName = $"{packageId}.{version}.nupkg";
            string nugetPackageFile = Path.Combine(tempDirectoryPath, nugetFileName);

            // not deleting the file and overwriting it results in a corrupted archive
            if (File.Exists(nugetPackageFile))
            {
                File.Delete(nugetPackageFile);
            }

            using (FileStream stream = File.Open(nugetPackageFile, FileMode.CreateNew))
            {
                builder.Save(stream);
                stream.Flush();
            }

            File.Delete(targetsFile);
            File.Move(nugetPackageFile, Path.Combine(outputFolder, nugetFileName));
            return new OperationResult();
        }
    }
}
