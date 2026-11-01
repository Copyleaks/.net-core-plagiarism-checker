using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection
{
    /// <summary>Information about the analyzed image.</summary>
    public class CopyleaksAiImageDetectionImageInfoModel
    {
        /// <summary>Dimensions of the analyzed image.</summary>
        [JsonProperty("shape")]
        public CopyleaksImageShapeModel Shape { get; set; }

        /// <summary>Optional metadata extracted from the image.</summary>
        [JsonProperty("metadata")]
        public CopyleaksImageMetadataModel Metadata { get; set; }
    }
}
