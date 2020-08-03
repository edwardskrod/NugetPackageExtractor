using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System;
using System.Threading;
using System.Threading.Tasks;
using Protocol = NuGet.Protocol;

namespace NugetPackageExtractor
{
    public class Program
    {
        private static readonly int PathToNupkg = 0;
        private static readonly int TargetPath = 1;
        private static readonly int PackageId = 2;
        private static readonly int PackageVersion = 3;
        private static readonly char DirectorySeparatorChar = '\\';

        /// <summary>
        /// Extracts a Nuget Package to a target path.
        /// </summary>
        /// <param name="args">
        /// args[0] string pathToNupkg
        /// args[1] string targetPath
        /// args[2] string packageId
        /// args[3] string packageVersion
        /// </param>
        static void Main(string[] args)
        {
            try
            {
                var config = Configuration.InitializeFrom(args);
                ExtractNupkg(config.PathToNupkg, config.TargetPath, config.PackageId, config.PackageVersion);
            }
            catch (ConfigurationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Extracts a Nuget Package
        /// </summary>
        /// <param name="pathToNupkg">The path to the Nuget Package.</param>
        /// <param name="targetPath">The target path to extract the Nuget Package.</param>
        /// <param name="packageId">The package id.</param>
        /// <param name="packageVersion">The package version.</param>
        public static void ExtractNupkg(string pathToNupkg, string targetPath, string packageId, string packageVersion)
        {
            targetPath = AddBackslashIfNotPresent(targetPath);
            var task = Task.Run(async () =>
            {
                using (var packageStream = FileSystem.OpenFile(pathToNupkg))
                {
                    var packageExtractionContext = new PackageExtractionContext(
                        packageSaveMode: PackageSaveMode.Defaultv3,
                        xmlDocFileSaveMode: XmlDocFileSaveMode.None,
                        clientPolicyContext: null,
                        logger: NullLogger.Instance);

                    var packageIdentity = new PackageIdentity(packageId, new NuGetVersion(new Version(packageVersion)));

                    using (var packageDownloader = new Protocol.LocalPackageArchiveDownloader(
                           null,
                           pathToNupkg,
                           packageIdentity,
                           logger: NullLogger.Instance))
                    {
                        await PackageExtractor.InstallFromSourceAsync(
                            packageIdentity,
                            packageDownloader,
                            new VersionFolderPathResolver(targetPath),
                            packageExtractionContext,
                            CancellationToken.None);
                    }
                }
            });

            task.Wait();
        }

        /// <summary>
        /// Returns a path that contains the backslash at then end. If the <paramref name="path"/> already contains a backslash, just return the original <paramref name="path"/>
        /// </summary>
        /// <param name="path">Path to append a backslash to if it doesn't contain one.</param>
        /// <returns>A path that contains backslash at the end.</returns>
        public static string AddBackslashIfNotPresent(string path)
        {
            if (!string.IsNullOrEmpty(path) && path[path.Length - 1] != DirectorySeparatorChar)
            {
                path += DirectorySeparatorChar;
            }

            return path;
        }
    }
}
