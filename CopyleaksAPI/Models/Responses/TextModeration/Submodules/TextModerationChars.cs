using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.TextModeration.Submodules
{
    public class TextModerationChars
    {
        /// <summary>Start character position of the labelled segment.</summary>
        [JsonProperty("starts")]
        public int[] Starts { get; set; }

        /// <summary>Predicted label index of each segment; resolve it to its ID with the legend.</summary>
        [JsonProperty("labels")]
        public int[] Labels { get; set; }

        /// <summary>Labelled segment character length.</summary>
        [JsonProperty("lengths")]
        public int[] Lengths { get; set; }


    }
}
