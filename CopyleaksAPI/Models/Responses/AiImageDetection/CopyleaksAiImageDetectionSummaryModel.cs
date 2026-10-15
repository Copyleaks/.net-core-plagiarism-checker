using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection
{
    /// <summary>Summary statistics of the AI detection analysis.</summary>
    public class CopyleaksAiImageDetectionSummaryModel
    {
        /// <summary>Percentage of pixels suspected to be human-created. Range: 0.0-1.0</summary>
        [JsonProperty("human")]
        public float Human { get; set; }

        /// <summary>Percentage of pixels suspected to be AI-generated. Range: 0.0-1.0</summary>
        [JsonProperty("ai")]
        public float Ai { get; set; }
    }
}
