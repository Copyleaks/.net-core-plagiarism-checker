using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.TextModeration.Submodules
{
    public class TextModerationsLegend
    {
        /// <summary>The numerical index of the label.</summary>
        [JsonProperty("index")]
        public int Index { get; set; }

        /// <summary>Unique, machine-readable label id.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
