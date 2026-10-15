using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
    public class MetadataSection
    {
        [JsonProperty("words")]
        public ushort Words { get; set; }

        [JsonProperty("excluded")]
        public ushort Excluded { get; set; }
    }
}