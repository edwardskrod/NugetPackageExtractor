using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NugetPackageExtractor
{
    public class Configuration
    {
        // Match on any valid Nuget package Id defined in NuGet.Packaging PackageIdValidator.cs
        // https://github.com/NuGet/NuGet.Client/blob/dev/src/NuGet.Core/NuGet.Packaging/PackageCreation/Utility/PackageIdValidator.cs
        public static Regex IdRegex = new Regex(@"^\w+([_.-]\w+)*$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        private string pathToNupkg;
        private string targetPath;
        private string packageId;
        private string packageVersion;

        public string PathToNupkg
        {
            get { return pathToNupkg; }
            private set
            {
                if (!File.Exists(value))
                {
                    throw new ConfigurationException($"value {value} should be a valid path to the nupkg.");
                }
                pathToNupkg = value;
            }
        }

        public string TargetPath
        {
            get { return targetPath; }
            private set
            {
                if (!Directory.Exists(value))
                {
                    throw new ConfigurationException($"value {value} should be a valid directory.");
                }

                targetPath = value;
            }
        }

        public string PackageId
        {
            get { return packageId; }
            private set
            {
                var nupkgPackageIdMatch = IdRegex.Match(value);
                if (!nupkgPackageIdMatch.Success)
                {
                    throw new ConfigurationException($"value {value} should be a valid Nuget Package Id matching {IdRegex}.");
                }
                packageId = value;
            }
        }

        public string PackageVersion
        {
            get { return packageVersion; }
            private set
            {
                if (!Version.TryParse(value, out var v))
                {
                    throw new ConfigurationException($"value {value} should be a Version.");
                }
                packageVersion = value;
            }
        }

        public static Configuration InitializeFrom(string[] args)
        {
            if (args.Length != 4)
            {
                throw new ConfigurationException("Exactly 4 arguments required.");
            }

            return new Configuration
            {
                PathToNupkg = args[0],
                TargetPath = args[1],
                PackageId = args[2],
                PackageVersion = args[3]
            };
        }
    }

    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {
        }
    }
}
