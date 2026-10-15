using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>Internal AI Source Match settings.</summary>
    public class InternalAISourceMatch
    {
        /// <summary>
        /// Find sources in the Copyleaks internal AI database that contain AI-generated text.
        /// Null = server default (false).
        /// </summary>
        [JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enable { get; set; }
    }
}
