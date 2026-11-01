using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection
{
    /// <summary>Optional metadata extracted from an image or a video.</summary>
    public class CopyleaksImageMetadataModel
    {
        /// <summary>Timestamp when the content was created (if available).</summary>
        [JsonProperty("issuedTime")]
        public string IssuedTime { get; set; }

        /// <summary>The AI service or tool that created the content (if detected).</summary>
        [JsonProperty("issuedBy")]
        public string IssuedBy { get; set; }

        /// <summary>The application or device used to create the content.</summary>
        [JsonProperty("appOrDeviceUsed")]
        public string AppOrDeviceUsed { get; set; }

        /// <summary>A summary of how the content was generated, when the metadata has one.</summary>
        [JsonProperty("contentSummary")]
        public string ContentSummary { get; set; }
    }
}
