using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.TextModeration.Submodules
{
    public class Text
    {
        /// <summary>Arrays describing the labelled segments.</summary>
        [JsonProperty("chars")]
        public TextModerationChars Chars { get; set; }
    }
}
