using System.Collections.Generic;
using Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ErrorModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.AiVideoDetection
{
    /// <summary>
    /// AI video detection result posted to your webhook; positions and lengths are in milliseconds.
    /// Check <see cref="Error"/> first.
    /// </summary>
    public class CopyleaksAiVideoDetectionResponseModel
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("audioResult")]
        public CopyleaksAiVideoTrackResult AudioResult { get; set; }

        [JsonProperty("visualResult")]
        public CopyleaksAiVideoTrackResult VisualResult { get; set; }

        [JsonProperty("summary")]
        public CopyleaksAiVideoSummary Summary { get; set; }

        /// <summary>Whether the video was determined to contain AI-generated content.</summary>
        [JsonProperty("isAiDetected")]
        public bool? IsAiDetected { get; set; }

        [JsonProperty("videoInfo")]
        public CopyleaksAiVideoInfo VideoInfo { get; set; }

        /// <summary>Scan id, credits and creation time.</summary>
        [JsonProperty("scannedVideo")]
        public CopyleaksAiImageDetectionScannedDocumentModel ScannedVideo { get; set; }

        /// <summary>Set when the detection failed; then only Error and DeveloperPayload are set.</summary>
        [JsonProperty("error")]
        public Error Error { get; set; }

        /// <summary>The developer payload from the submit request, or null.</summary>
        [JsonProperty("developerPayload")]
        public string DeveloperPayload { get; set; }
    }

    public class CopyleaksAiVideoTrackResult
    {
        [JsonProperty("starts")]
        public List<long> Starts { get; set; }

        [JsonProperty("lengths")]
        public List<long> Lengths { get; set; }

        /// <summary>Ranges that were not scored and are not part of the AI ratios.</summary>
        [JsonProperty("exclude")]
        public CopyleaksAiVideoRanges Exclude { get; set; }
    }

    public class CopyleaksAiVideoRanges
    {
        [JsonProperty("starts")]
        public List<long> Starts { get; set; }

        [JsonProperty("lengths")]
        public List<long> Lengths { get; set; }
    }

    public class CopyleaksAiVideoSummary
    {
        [JsonProperty("audioAIRatio")]
        public double AudioAIRatio { get; set; }

        [JsonProperty("visualAIRatio")]
        public double VisualAIRatio { get; set; }

        [JsonProperty("overallAIRatio")]
        public double OverallAIRatio { get; set; }
    }

    public class CopyleaksAiVideoInfo
    {
        /// <summary>The duration of the video in seconds.</summary>
        [JsonProperty("duration")]
        public double Duration { get; set; }

        /// <summary>Optional metadata extracted from the video.</summary>
        [JsonProperty("metadata")]
        public CopyleaksImageMetadataModel Metadata { get; set; }
    }
}
