using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
    public class FullTextSourceExcludeSection
    {
        [JsonProperty("starts")]
        public int[] Starts { get; set; }

        [JsonProperty("lengths")]
        public int[] Lengths { get; set; }

        [JsonProperty("reasons")]
        public int[] Reasons { get; set; }
    }
    public class FullHtmlSourceExcludeSection: FullTextSourceExcludeSection
    {
        [JsonProperty("groupIds")]
        public int[] GroupIds { get; set; }
    }
}