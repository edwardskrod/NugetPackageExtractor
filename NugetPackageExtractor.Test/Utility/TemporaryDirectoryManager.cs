// <copyright file="TemporaryDirectoryManager.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NupkgPackageExtractor.Test.Utility
{
    public class TemporaryDirectoryManager : IDisposable
    {
        private const int DefaultRetryCount = 2;
        private const int DefaultRetryDelay = 500;

        private static readonly string TestDirectoryRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        private static readonly string IllegalCharacters = "d_";
        private string tempDirectory = null;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryDirectoryManager"/> class.
        /// </summary>
        public TemporaryDirectoryManager()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TemporaryDirectoryManager"/> class.
        /// </summary>
        ~TemporaryDirectoryManager()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets temporary directory for test cases.
        /// </summary>
        /// <param name="path">Optional sub path.</param>
        /// <param name="createDirectory">Whether to create the actual directory or not.</param>
        /// <param name="className">Caller's class name.</param>
        /// <param name="methodName">Caller's method name.</param>
        /// <returns>Path string of the temporary directory.</returns>
        /// Note: please notice the call stack frame and default methodName here, specify them as needed. For Async methods, please specify the className.
        public string GetTemporaryDirectory(string path = null, bool createDirectory = true, string className = null, [CallerMemberName] string methodName = null)
        {
            MethodBase caller = null;
            MethodBase getCaller()
            {
                if (caller is null)
                {
                    var stack = new StackTrace();
                    caller = stack.GetFrame(2).GetMethod();
                }

                return caller;
            }

            if (string.IsNullOrEmpty(className))
            {
                className = getCaller().DeclaringType.Name;
            }

            if (className.Contains(IllegalCharacters))
            {
                // The user is using an async method w/o specifying the className as instructed in the Note above.
                // Fall back to a default name; it will be safe with the random guid and the directory name.
                className = "fallback";
            }

            if (string.IsNullOrEmpty(methodName))
            {
                methodName = getCaller().Name;
            }

            if (getCaller().IsConstructor)
            {
                methodName = methodName.Replace(".", string.Empty);
            }

            var tempDirPrefix = string.Join("-", TestDirectoryRoot, className);
            var tempDirectory = Path.Combine(tempDirPrefix, methodName, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            if (!string.IsNullOrEmpty(path))
            {
                tempDirectory = Path.Combine(tempDirectory, path);
            }

            if (createDirectory)
            {
                Directory.CreateDirectory(tempDirectory);
                this.tempDirectory = tempDirPrefix;
            }

            return tempDirectory;
        }

        /// <summary>
        /// Gets temporary file for test cases.
        /// </summary>
        /// <param name="path">Optional sub path.</param>
        /// <param name="createFile">Whether to create the actual file or not.</param>
        /// <param name="extension">Change to a specific extension</param>
        /// <param name="className">Caller's class name.</param>
        /// <param name="methodName">Caller's method name.</param>
        /// <returns>Path string of the temporary directory.</returns>
        /// <exception cref="ArgumentException"><paramref name="extension"/> contains one or more of the invalid characters defined in System.IO.Path.GetInvalidPathChars.</exception>
        /// Note: please notice the call stack frame and default methodName here, specify them as needed. For Async methods, please specify the className.
        public string GetTemporaryFile(string path = null, bool createFile = true, string extension = null, string className = null, [CallerMemberName] string methodName = null)
        {
            MethodBase caller = null;
            MethodBase getCaller()
            {
                if (caller is null)
                {
                    var stack = new StackTrace();
                    caller = stack.GetFrame(2).GetMethod();
                }

                return caller;
            }

            if (string.IsNullOrEmpty(className))
            {
                className = getCaller().DeclaringType.Name;
            }

            if (className.Contains(IllegalCharacters))
            {
                // The user is using an async method w/o specifying the className as instructed in the Note above.
                // Fall back to a default name; it will be safe with the random guid and the file name.
                className = "fallback";
            }

            if (string.IsNullOrEmpty(methodName))
            {
                methodName = getCaller().Name;
            }

            if (getCaller().IsConstructor)
            {
                methodName = methodName.Replace(".", string.Empty);
            }

            var tempDirPrefix = string.Join("-", TestDirectoryRoot, className);
            var tempFile = Path.Combine(tempDirPrefix, methodName, Path.GetRandomFileName());

            if (!string.IsNullOrEmpty(extension))
            {
                tempFile = Path.ChangeExtension(tempFile, extension);
            }

            if (!string.IsNullOrEmpty(path))
            {
                tempFile = Path.Combine(tempFile, path);
            }

            if (createFile)
            {
                CreateEmptyFile(tempFile);
                this.tempDirectory = tempDirPrefix;
            }

            return tempFile;
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (tempDirectory != null)
                {
                    TryDeleteDirectoryWithRetry(tempDirectory);
                }

                disposed = true;
            }
        }

        private void TryDeleteDirectoryWithRetry(string path, bool recursive = true, int retryCount = DefaultRetryCount, int retryDelay = DefaultRetryDelay)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            TryExecuteFileActionWithRetry(retryCount, retryDelay, () => Directory.Delete(path, recursive: recursive));
        }

        private void TryExecuteFileActionWithRetry(int retryCount, int retryDelay, Action fileAction)
        {
            var maxAttempts = retryCount + 1;

            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    fileAction();
                    return;
                }
                catch
                {
                    if (attempt + 1 < maxAttempts)
                    {
                        Task.Delay(retryDelay).Wait();
                    }
                }
            }
        }

        private void CreateEmptyFile(string filename)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.Create(filename).Dispose();
        }
    }
}