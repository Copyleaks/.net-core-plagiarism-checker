using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.TextModeration.Submodules
{
    public class ModerationsModel
    {
        /// <summary>Moderated segments of the textual version; each inner-array position is one segment.</summary>
        [JsonProperty("text")]
        public Text Text { get; set; }
    }
}
