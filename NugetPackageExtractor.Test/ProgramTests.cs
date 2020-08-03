// <copyright file="ProgramTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// </copyright>

using NupkgPackageExtractor.Test.Utility;
using System.IO;
using Xunit;

namespace NugetPackageExtractor.Test
{
    public class ProgramTests
    {
        private readonly string installDirectory;
        private readonly TemporaryDirectoryManager temporaryDirectoryManager;
        private const string TestPackagePath = @"Data\test.2.4.2.nupkg";
        private const string PackageId = "test";
        private const string PackageVersion = "2.4.2";

        public ProgramTests()
        {
            temporaryDirectoryManager = new TemporaryDirectoryManager();
            installDirectory = temporaryDirectoryManager.GetTemporaryDirectory();
        }

        [Fact]
        public void ExtractNupkg()
        {
            Program.ExtractNupkg(TestPackagePath, installDirectory, PackageId, PackageVersion);

            var packageDir = Path.Combine(installDirectory, "test", "2.4.2");

            Assert.True(File.Exists(Path.Combine(packageDir, @"test.2.4.2.nupkg")));
            Assert.True(File.Exists(Path.Combine(packageDir, @".nupkg.metadata")));
            Assert.True(File.Exists(Path.Combine(packageDir, @".signature.p7s")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"License.txt")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"logo-512-transparent.png")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"test.2.4.2.nupkg.sha512")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"test.nuspec")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\net452\xunit.abstractions.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\net452\xunit.runner.reporters.net452.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\net452\xunit.runner.utility.net452.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\net452\xunit.runner.visualstudio.props")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\net452\xunit.runner.visualstudio.testadapter.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\netcoreapp2.1\xunit.abstractions.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\netcoreapp2.1\xunit.runner.reporters.netcoreapp10.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\netcoreapp2.1\xunit.runner.utility.netcoreapp10.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\netcoreapp2.1\xunit.runner.visualstudio.dotnetcore.testadapter.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\netcoreapp2.1\xunit.runner.visualstudio.props")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\uap10.0.16299\xunit.runner.reporters.netstandard15.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\uap10.0.16299\xunit.runner.utility.netstandard15.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\uap10.0.16299\xunit.runner.utility.uwp10.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\uap10.0.16299\xunit.runner.utility.uwp10.pri")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\uap10.0.16299\xunit.runner.visualstudio.props")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\uap10.0.16299\xunit.runner.visualstudio.targets")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\uap10.0.16299\xunit.runner.visualstudio.uwp.testadapter.dll")));
            Assert.True(File.Exists(Path.Combine(packageDir, @"build\uap10.0.16299\xunit.runner.visualstudio.uwp.testadapter.pri")));
        }
    
        [Theory]
        [InlineData("test", "test\\")]
        [InlineData("test\\", "test\\")]
        [InlineData("", "")]
        public void AddBackslashIfNotPresent(string path, string expected)
        {
            var updatedPath = Program.AddBackslashIfNotPresent(path);

            Assert.Equal(expected, updatedPath);
        }
    }
}
