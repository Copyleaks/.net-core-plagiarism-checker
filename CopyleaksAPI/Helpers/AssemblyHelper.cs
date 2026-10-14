/********************************************************************************
 The MIT License(MIT)
 
 Copyright(c) 2016 Copyleaks LTD (https://copyleaks.com)
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
********************************************************************************/

using System.Reflection;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>
    /// Get Copyleaks SDK version from assembly
    /// </summary>
    internal static class AssemblyHelper
    {
        /// <summary>
        /// The SDK version, read once from the assembly metadata.
        /// It does not touch the file system, so it also works in single-file and trimmed deployments.
        /// </summary>
        public static readonly string Version = ReadVersion();

        /// <summary>
        /// The User-Agent sent with every request.
        /// </summary>
        public static readonly string UserAgent = $"copyleaks-core-sdk/{Version}";

        /// <summary>
        /// Return the version of Copyleaks SDK from assembly
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            return Version;
        }

        private static string ReadVersion()
        {
            Assembly assembly = typeof(AssemblyHelper).Assembly;
            string version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            if (string.IsNullOrEmpty(version))
                version = assembly.GetName().Version?.ToString();
            return string.IsNullOrEmpty(version) ? "unknown" : version;
        }
    }
}
