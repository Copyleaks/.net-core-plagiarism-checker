using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection
{
    /// <summary>RLE-encoded mask data for AI-detected regions.</summary>
    public class CopyleaksAiImageDetectionResultModel
    {
        /// <summary>Start positions of AI-detected segments in the flattened image array.</summary>
        [JsonProperty("starts")]
        public int[] Starts { get; set; }

        /// <summary>Lengths of AI-detected segments corresponding to each start position.</summary>
        [JsonProperty("lengths")]
        public int[] Lengths { get; set; }

        /// <summary>"heatmap" mask type only: one confidence band per segment, 0 (lowest) to 4 (highest).</summary>
        [JsonProperty("labels")]
        public int[] Labels { get; set; }
    }
}
