using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// Citation Checker settings (sent as properties.references).
    /// The completed webhook then has a referencesValidation summary, and the crawled version export has the full per-reference results.
    /// </summary>
    public class ReferencesSettings
    {
        /// <summary>
        /// Detect, parse and validate every reference in the submitted text.
        /// Leave it unset (null) to omit it from the request and get the server default (false).
        /// </summary>
        [JsonProperty("validate", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Validate { get; set; }
    }
}
