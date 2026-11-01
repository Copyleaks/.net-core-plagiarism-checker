using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
    /// <summary>A scan that failed to be deleted.</summary>
    public class Failure
    {
        /// <summary>The scan id</summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>The failure description</summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
