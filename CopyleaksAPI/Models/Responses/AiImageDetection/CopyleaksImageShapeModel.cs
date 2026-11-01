using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection
{
    /// <summary>Dimensions of the analyzed image.</summary>
    public class CopyleaksImageShapeModel
    {
        /// <summary>Height of the image in pixels.</summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>Width of the image in pixels.</summary>
        [JsonProperty("width")]
        public int Width { get; set; }
    }
}
