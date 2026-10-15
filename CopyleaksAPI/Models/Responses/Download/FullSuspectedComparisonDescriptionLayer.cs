using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
    public class FullSuspectedComparisonDescriptionLayer
    {
        [JsonProperty("chars")]
        public FullSuspectedComparisonLayerValues Chars { get; set; }

        [JsonProperty("words")]
        public FullSuspectedComparisonLayerValues Words { get; set; }
    }
}