using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// Internal AI Source Match settings (sent as properties.internalAiSourceMatch).
    /// </summary>
    public class InternalAISourceMatch
    {
        /// <summary>
        /// Activates identification of sources in the Copyleaks internal AI database that contain AI-generated text.
        /// Leave it unset (null) to omit it from the request and get the server default (false).
        /// </summary>
        [JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enable { get; set; }
    }
}
