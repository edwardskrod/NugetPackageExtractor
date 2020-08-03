// <copyright file="FileSystem.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// </copyright>

using System.IO;

namespace NugetPackageExtractor
{
    public static class FileSystem
    {
        public static Stream OpenFile(string path)
        {
            Validate.IsNotNullOrEmpty(path, nameof(Path));

            return File.OpenRead(path);
        }
    }
}
