using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// AI Logic settings for AI detection in a scan (sent as properties.aiGeneratedText.explain).
    /// </summary>
    public class AIExplainSettings
    {
        /// <summary>
        /// Enable AI Logic, which explains the AI detection result. Explanations are returned for a limited set of languages only.
        /// Leave it unset (null) to omit it from the request and get the server default (false).
        /// </summary>
        [JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enable { get; set; }
    }
}
