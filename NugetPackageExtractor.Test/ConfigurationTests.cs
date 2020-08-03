// <copyright file="ConfigurationTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// </copyright>

using NupkgPackageExtractor.Test.Utility;
using Xunit;

namespace NugetPackageExtractor.Test
{
    public class ConfigurationTests
    {
        private readonly string installDirectory;
        private readonly TemporaryDirectoryManager temporaryDirectoryManager;

        private string pathToNupkg;
        private string targetPath;
        private string packageId;
        private string packageVersion;

        private const string TestPackagePath = @"Data\test.2.4.2.nupkg";

        public ConfigurationTests()
        {
            temporaryDirectoryManager = new TemporaryDirectoryManager();
            installDirectory = temporaryDirectoryManager.GetTemporaryDirectory();
        }

        [Fact]
        public void InitializeFrom_Valid()
        {
            pathToNupkg = TestPackagePath;
            targetPath = installDirectory;
            packageId = "test";
            packageVersion = "2.4.2";

            var config = Configuration.InitializeFrom(new string[] { pathToNupkg, targetPath, packageId, packageVersion });

            Assert.Equal(pathToNupkg, config.PathToNupkg);
            Assert.Equal(targetPath, config.TargetPath);
            Assert.Equal(packageId, config.PackageId);
            Assert.Equal(packageVersion, config.PackageVersion);
        }

        [Fact]
        public void InitializeFrom_ThreeArgs_ExpectException()
        {
            var exception = Assert.Throws<ConfigurationException>(() => Configuration.InitializeFrom(new string[] { "c:\\test" }));

            Assert.Equal("Exactly 4 arguments required.", exception.Message);
        }

        [Fact]
        public void InitializeFrom_InvalidPathToNupkg_ExpectException()
        {
            pathToNupkg = "c:\\doesnotexist";
            targetPath = installDirectory;
            packageId = "test";
            packageVersion = "2.4.2";

            var exception = Assert.Throws<ConfigurationException>(() => Configuration.InitializeFrom(new string[] { pathToNupkg, targetPath, packageId, packageVersion }));

            Assert.Equal($"value {pathToNupkg} should be a valid path to the nupkg.", exception.Message);
        }

        [Fact]
        public void InitializeFrom_InvalidTargetPath_ExpectException()
        {
            pathToNupkg = TestPackagePath;
            targetPath = "c:\\doesnotexist";
            packageId = "test";
            packageVersion = "2.4.2";

            var exception = Assert.Throws<ConfigurationException>(() => Configuration.InitializeFrom(new string[] { pathToNupkg, targetPath, packageId, packageVersion }));

            Assert.Equal($"value {targetPath} should be a valid directory.", exception.Message);
        }

        [Fact]
        public void InitializeFrom_InvalidPackageId_ExpectException()
        {
            pathToNupkg = TestPackagePath;
            targetPath = installDirectory;
            packageId = "test.";
            packageVersion = "2.4.2";

            var exception = Assert.Throws<ConfigurationException>(() => Configuration.InitializeFrom(new string[] { pathToNupkg, targetPath, packageId, packageVersion }));

            Assert.Equal($"value {packageId} should be a valid Nuget Package Id matching {Configuration.IdRegex}.", exception.Message);
        }

        [Fact]
        public void InitializeFrom_InvalidPackageVersion_ExpectException()
        {
            pathToNupkg = TestPackagePath;
            targetPath = installDirectory;
            packageId = "test";
            packageVersion = "invalidVersion";

            var exception = Assert.Throws<ConfigurationException>(() => Configuration.InitializeFrom(new string[] { pathToNupkg, targetPath, packageId, packageVersion }));

            Assert.Equal($"value {packageVersion} should be a Version.", exception.Message);
        }

    }
}
