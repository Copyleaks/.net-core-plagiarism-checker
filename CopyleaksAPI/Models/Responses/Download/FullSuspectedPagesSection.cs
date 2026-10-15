using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
    public class FullSuspectedPagesSection
    {
        [JsonProperty("startPosition")]
        public int[] StartPosition { get; set; }
    }
}