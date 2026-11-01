using System.Reflection;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>SDK version and User-Agent.</summary>
    internal static class AssemblyHelper
    {
        /// <summary>
        /// Read once from the assembly metadata, not the file system, so it works in single-file and trimmed deployments.
        /// </summary>
        public static readonly string Version = ReadVersion();

        /// <summary>The User-Agent sent with every request.</summary>
        public static readonly string UserAgent = $"copyleaks-core-sdk/{Version}";

        /// <summary>Returns <see cref="Version"/>.</summary>
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
