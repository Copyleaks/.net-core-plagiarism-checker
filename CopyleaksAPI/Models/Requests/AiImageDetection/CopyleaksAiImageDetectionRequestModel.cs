using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection
{
    /// <summary>
    /// AI image detection request. <c>SubmitAsync</c> sends JSON with the base64 image;
    /// <c>SubmitFileAsync</c> uploads the file (multipart/form-data) and ignores <see cref="Base64"/>.
    /// </summary>
    public class CopyleaksAiImageDetectionRequestModel
    {
        /// <summary>For <c>SubmitFileAsync</c>, which uploads the image as a file instead of base64.</summary>
        public CopyleaksAiImageDetectionRequestModel(string fileName, string model, bool sandbox = false)
            : this(null, fileName, model, sandbox)
        {
        }

        public CopyleaksAiImageDetectionRequestModel(string base64, string fileName, string model, bool sandbox = false)
        {
            Base64 = base64;
            FileName = fileName;
            Model = model;
            Sandbox = sandbox;
        }
        /// <summary>
        /// Base64 image: min 512×512px, max 16 megapixels, under 32MB; PNG, JPEG, BMP, WebP or HEIC/HEIF.
        /// </summary>
        [JsonProperty("base64", NullValueHandling = NullValueHandling.Ignore)]
        public string Base64 { get; set; }

        /// <summary>
        /// Image file name with extension, e.g. "my-image.png"; at most 255 characters.
        /// Supported: .png, .bmp, .jpg, .jpeg, .webp, .heic, .heif.
        /// </summary>
        [Required]
        [JsonProperty("filename")]
        public string FileName { get; set; }

        /// <summary>
        /// Detection model, full name or alias: "ai-image-1-ultra-01-09-2025" or "ai-image-1-ultra" (AI Image 1 Ultra).
        /// </summary>
        [Required]
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>Sandbox mode for development: mock results, no credits consumed. Default false.</summary>
        [JsonProperty("sandbox")]
        public bool Sandbox { get; set; } = false;

        /// <summary>
        /// Mask type (<see cref="Constants.CopyleaksAiImageMaskTypes"/>): "binary", "heatmap" (adds Result.Labels)
        /// or "none" (omits Result). Null = server default ("binary").
        /// </summary>
        [JsonProperty("maskType", NullValueHandling = NullValueHandling.Ignore)]
        public string MaskType { get; set; }

        /// <summary>
        /// Objects to locate and segment, up to 3; the response then has DetectedObjects.
        /// Do not send an empty list: the server rejects it.
        /// </summary>
        [JsonProperty("objectDetection", NullValueHandling = NullValueHandling.Ignore)]
        public List<CopyleaksAiImageObjectDetectionItem> ObjectDetection { get; set; }
    }
}
