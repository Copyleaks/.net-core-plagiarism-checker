using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection
{
    /// <summary>An object to locate and segment in an image.</summary>
    public class CopyleaksAiImageObjectDetectionItem
    {
        public CopyleaksAiImageObjectDetectionItem()
        {
        }

        public CopyleaksAiImageObjectDetectionItem(string @object)
        {
            Object = @object;
        }

        /// <summary>Free-text object name, e.g. "face" or "hand"; non-empty, at most 32 characters.</summary>
        [JsonProperty("object")]
        public string Object { get; set; }
    }
}
