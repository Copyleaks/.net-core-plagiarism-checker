using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.AIDetector
{
    public class Summary
    {
        [JsonProperty("human")]
        public double Human { get; set; }

        [JsonProperty("ai")]
        public double AI { get; set; }
    }
}
