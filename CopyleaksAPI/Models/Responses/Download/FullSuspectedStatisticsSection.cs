using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
    public class FullSuspectedStatisticsSection
    {
        [JsonProperty("identical")]
        public int Identical { get; set; }

        [JsonProperty("minorChanges")]
        public int MinorChanges { get; set; }

        [JsonProperty("relatedMeaning")]
        public int RelatedMeaning { get; set; }
    }
}