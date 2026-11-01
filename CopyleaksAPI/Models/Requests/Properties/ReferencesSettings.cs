using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// Citation Checker settings. Results: a referencesValidation summary in the completed webhook,
    /// full per-reference results in the crawled version export.
    /// </summary>
    public class ReferencesSettings
    {
        /// <summary>Detect, parse and validate every reference in the text. Null = server default (false).</summary>
        [JsonProperty("validate", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Validate { get; set; }
    }
}
