using Copyleaks.SDK.V3.API.Models.Responses.TextModeration.Submodules;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.TextModeration
{
    public class CopyleaksTextModerationResponseModel
    {
        /// <summary>Moderated text segments detected in the input text.</summary>
        [JsonProperty("moderations")]
        public ModerationsModel Moderations { get; set; }

        /// <summary>Lookup for the label indices in text.chars.labels: one entry per label used in the scan.</summary>
        [JsonProperty("legend")]
        public TextModerationsLegend[] Legend { get; set; }

        /// <summary>General information about the scanned document.</summary>
        [JsonProperty("scannedDocument")]
        public TextModerationScannedDocument ScannedDocument {  get; set; }
    }
}
