using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection
{
    /// <summary>
    /// One object to locate and segment in an image (an entry of objectDetection).
    /// </summary>
    public class CopyleaksAiImageObjectDetectionItem
    {
        public CopyleaksAiImageObjectDetectionItem()
        {
        }

        public CopyleaksAiImageObjectDetectionItem(string @object)
        {
            Object = @object;
        }

        /// <summary>
        /// A free-text name of the object, for example "face" or "hand". At most 32 characters.
        /// </summary>
        [JsonProperty("object")]
        public string Object { get; set; }
    }
}
