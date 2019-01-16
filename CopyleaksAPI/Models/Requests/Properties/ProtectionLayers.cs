using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// Defines which mediums to scan.
    /// </summary>
    public class ProtectionLayers
    {
        /// <summary>
        /// Scan against the internet?
        /// </summary>
        [JsonProperty("internet")]
        public bool Internet { get; set; } = true;

        /// <summary>
        /// Scan against copyleaks internal database?
        /// If specified true, it will also index the scanned document into the database.
        /// </summary>
        [JsonProperty("copyleaksDB")]
        public bool CopyleaksInternalDB { get; set; } = true;
    }
}
