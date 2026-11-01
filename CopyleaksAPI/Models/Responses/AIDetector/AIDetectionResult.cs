using Newtonsoft.Json;
using System.Collections.Generic;

namespace Copyleaks.SDK.V3.API.Models.Responses.AIDetector
{
    /// <summary>
    /// AI text detection result, from the writer-detector check or the completed webhook's "suspected-ai-text" alert.
    /// </summary>
    public class AIDetectionResult
    {
        [JsonProperty("scanType")]
        public string ScanType { get; set; }

        /// <summary>AI detection model version.</summary>
        [JsonProperty("modelVersion")]
        public string ModelVersion { get; set; }

        /// <summary>Classified sections, each with its positions, classification and probability.</summary>
        [JsonProperty("results")]
        public List<ClassificationResult> Results { get; set; }

        /// <summary>Aggregated detection results.</summary>
        [JsonProperty("summary")]
        public Summary Summary { get; set; }

        /// <summary>General information about the scanned document.</summary>
        [JsonProperty("scannedDocument")]
        public ScannedDocument ScannedDocument { get; set; }

        /// <summary>
        /// Provider that machine-translated the text to English before detection; 0 = not translated.
        /// Only in the completed webhook AI alert data; null when absent.
        /// </summary>
        [JsonProperty("translationProvider")]
        public int? TranslationProvider { get; set; }

        /// <summary>The scanned English machine translation, if any. Webhook AI alert data only; else null.</summary>
        [JsonProperty("translation")]
        public string Translation { get; set; }

        /// <summary>AI Logic explanation; null unless AI Logic (explain) was enabled.</summary>
        [JsonProperty("explain")]
        public AIExplain Explain { get; set; }
    }
}
