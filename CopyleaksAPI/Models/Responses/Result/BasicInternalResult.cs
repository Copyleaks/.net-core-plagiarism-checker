using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
    public abstract class BasicInternalResult: BasicResult
    {
        /// <summary>Original scan id of the matched resource if you submitted it; otherwise null.</summary>
        [JsonProperty("scanId")]
        public string ScanId { get; set; }
    }
}
