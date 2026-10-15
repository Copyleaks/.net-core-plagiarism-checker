using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
    public class FullTextSuspectedComparisonDescription
    {
        [JsonProperty("source")]
        public FullSuspectedComparisonDescriptionLayer Source { get; set; }

        [JsonProperty("suspected")]
        public FullSuspectedComparisonDescriptionLayer Suspected { get; set; }
    }

    public class FullHtmlSuspectedComparisonDescription : FullTextSuspectedComparisonDescription
    {
        [JsonProperty("groupId")]
        public int[] GroupId { get; set; }
    }
}