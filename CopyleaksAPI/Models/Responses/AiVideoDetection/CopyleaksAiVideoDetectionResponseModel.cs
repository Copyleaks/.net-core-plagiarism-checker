using System.Collections.Generic;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.AiVideoDetection
{
    /// <summary>
    /// The AI video detection result that Copyleaks posts to your webhook.
    /// Positions and lengths are in milliseconds.
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

        /// <summary>
        /// Whether the video was determined to contain AI-generated content.
        /// </summary>
        [JsonProperty("isAiDetected")]
        public bool? IsAiDetected { get; set; }

        [JsonProperty("videoInfo")]
        public CopyleaksAiVideoInfo VideoInfo { get; set; }

        [JsonProperty("scannedVideo")]
        public CopyleaksAiVideoScannedVideo ScannedVideo { get; set; }
    }

    public class CopyleaksAiVideoTrackResult
    {
        [JsonProperty("starts")]
        public List<long> Starts { get; set; }

        [JsonProperty("lengths")]
        public List<long> Lengths { get; set; }

        /// <summary>
        /// Ranges that were not scored and are not part of the AI ratios.
        /// </summary>
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
        /// <summary>
        /// The duration of the video in seconds.
        /// </summary>
        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("metadata")]
        public CopyleaksAiVideoMetadata Metadata { get; set; }
    }

    public class CopyleaksAiVideoMetadata
    {
        [JsonProperty("issuedTime")]
        public string IssuedTime { get; set; }

        [JsonProperty("issuedBy")]
        public string IssuedBy { get; set; }

        [JsonProperty("appOrDeviceUsed")]
        public string AppOrDeviceUsed { get; set; }

        [JsonProperty("contentSummary")]
        public string ContentSummary { get; set; }
    }

    public class CopyleaksAiVideoScannedVideo
    {
        [JsonProperty("scanId")]
        public string ScanId { get; set; }

        [JsonProperty("actualCredits")]
        public int ActualCredits { get; set; }

        [JsonProperty("expectedCredits")]
        public int ExpectedCredits { get; set; }

        [JsonProperty("creationTime")]
        public string CreationTime { get; set; }
    }
}
