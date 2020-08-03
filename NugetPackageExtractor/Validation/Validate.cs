// <copyright file="Validate.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace NugetPackageExtractor
{
    /// <summary>
    /// General parameter validation helpers.
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the given object is null.
        /// </summary>
        /// <param name="o">Object to test.</param>
        /// <param name="paramName">
        /// <paramref name="paramName"/> parameter used if an exception is raised.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="o"/> is null.</exception>
        public static void IsNotNull(object o, string paramName)
        {
            if (o == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the given string is empty.
        /// </summary>
        /// <param name="s">String to test</param>
        /// <param name="paramName">
        /// <paramref name="paramName"/> parameter used if an exception is raised
        /// </param>
        /// <exception cref="ArgumentException"><paramref name="s"/> is an empty string.</exception>
        public static void IsNotEmpty(string s, string paramName)
        {
            if (s == string.Empty)
            {
                throw new ArgumentException(Resources.ValidateError_StringEmpty, paramName);
            }
        }

        /// <summary>
        /// Validates that the given string is both non-null and non-empty.
        /// </summary>
        /// <param name="s">String to test</param>
        /// <param name="paramName">
        /// <paramref name="paramName"/> parameter used if an exception is raised
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="s"/> is an empty string.</exception>
        public static void IsNotNullOrEmpty(string s, string paramName)
        {
            Validate.IsNotNull(s, paramName);
            IsNotEmpty(s, paramName);
        }
    }
}
