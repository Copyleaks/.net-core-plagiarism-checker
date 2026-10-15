using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
    /// <summary>Copyleaks API response for `Start` request</summary>
	public class StartResponse
    {
        /// <summary>Scans that started successfully.</summary>
        [JsonProperty("success")]
        public StartRow[] Success { get; set; }

        /// <summary>Scans that failed to start.</summary>
        [JsonProperty("failed")]
        public StartRow[] Failed { get; set; }
    }
}
