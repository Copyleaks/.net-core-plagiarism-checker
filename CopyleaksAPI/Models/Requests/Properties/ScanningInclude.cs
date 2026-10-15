using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>Which internal database and repository results to include.</summary>
    public class ScanningInclude
    {
        /// <summary>
        /// Include results only when their scan id matches this pattern.
        /// "*" = any number of characters, "." = exactly one non-whitespace character.
        /// </summary>
        [JsonProperty("idPattern", NullValueHandling = NullValueHandling.Ignore)]
        public string IdPattern { get; set; }
    }
}
