using Copyleaks.SDK.V3.API.Models.Responses.Result;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Callbacks
{
    /// <summary>Payload of the webhook sent when a 'check credits' scan completes.</summary>
	public class CreditsCheckCallback : BaseResponse
    {
        /// <summary>Credits the scan will consume.</summary>
        [JsonProperty("credits")]
        public uint Credits { get; set; }
    }
}
