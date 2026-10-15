using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection
{
    /// <summary>Metadata about the AI image detection scan operation.</summary>
    public class CopyleaksAiImageDetectionScannedDocumentModel
    {
        /// <summary>The unique identifier for this scan.</summary>
        [JsonProperty("scanId")]
        public string ScanId { get; set; }

        /// <summary>The actual number of credits consumed by this scan.</summary>
        [JsonProperty("actualCredits")]
        public int ActualCredits { get; set; }

        /// <summary>The expected number of credits for this scan.</summary>
        [JsonProperty("expectedCredits")]   
        public int ExpectedCredits { get; set; }

        /// <summary>ISO 8601 timestamp of when the scan was created.</summary>
        [JsonProperty("creationTime")]
        public string CreationTime { get; set; }
    }
}
