using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>AI Logic settings for AI detection in a scan.</summary>
    public class AIExplainSettings
    {
        /// <summary>
        /// Enable AI Logic, which explains the AI detection result, for a limited set of languages only.
        /// Null = server default (false).
        /// </summary>
        [JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enable { get; set; }
    }
}
