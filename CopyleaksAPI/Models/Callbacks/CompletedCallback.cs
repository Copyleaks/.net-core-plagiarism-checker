using Copyleaks.SDK.V3.API.Models.Responses.Result;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Callbacks
{
    /// <summary>Payload of the completed webhook.</summary>
    public class CompletedCallback : Result
	{
        [JsonProperty("downloadableReport")]
        public ReportResponse ReportResponse { get; set; }
        
    }
}
