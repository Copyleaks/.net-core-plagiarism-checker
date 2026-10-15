using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>AI Source Match settings.</summary>
    public class AISourceMatch
    {
        /// <summary>
        /// Find online sources suspected of containing AI-generated text. Only for documents detected as English.
        /// Null = server default (false).
        /// </summary>
        [JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enable { get; set; }
    }
}
