using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
    /// <summary>Request to start a batch scan.</summary>
    public class StartBatchRequest : StartRequest
    {
        /// <summary>Previously scanned content to compare against.</summary>
        [JsonProperty("include")]
        public string[] Include { get; set; }
    }
}
