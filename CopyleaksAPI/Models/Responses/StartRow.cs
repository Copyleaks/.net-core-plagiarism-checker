using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
    /// <summary>A started scan id</summary>
    public class StartRow
    {
        /// <summary>The scan id</summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
