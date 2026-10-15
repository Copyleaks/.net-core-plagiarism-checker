using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
    public class FullSourcePagesSection
    {
        [JsonProperty("startPosition")]
        public uint[] StartPosition { get; set; }
    }
}