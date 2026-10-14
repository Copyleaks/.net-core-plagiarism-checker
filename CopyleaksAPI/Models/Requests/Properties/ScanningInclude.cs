using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// Which internal database and repository results to include (sent as properties.scanning.include).
    /// </summary>
    public class ScanningInclude
    {
        /// <summary>
        /// Include results only when their scan id matches this pattern.
        /// "*" matches any number of characters and "." matches exactly one non-whitespace character.
        /// </summary>
        [JsonProperty("idPattern", NullValueHandling = NullValueHandling.Ignore)]
        public string IdPattern { get; set; }
    }
}
