using Copyleaks.SDK.V3.API.Models.Responses.WritingAssistant;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.WritingFeedback
{
    public class WritingAssistantResult
    {
        [JsonProperty("scanType")]
        public string ScanType { get; set; }

        [JsonProperty("score")]
        public Score Score { get; set; }

        /// <summary>Corrections for the input text, such as spelling, grammar and sentence structure errors.</summary>
        [JsonProperty("corrections")]
        public Corrections Corrections { get; set; }

        /// <summary>General information about the scanned document.</summary>
        [JsonProperty("scannedDocument")]
        public ScannedDocument ScannedDocument { get; set; }
    }
}
