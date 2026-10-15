using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection
{
    /// <summary>
    /// AI image detection request. <c>SubmitAsync</c> sends JSON with the base64 image;
    /// <c>SubmitFileAsync</c> uploads the file (multipart/form-data), and <see cref="Base64"/> must then be null.
    /// </summary>
    public class CopyleaksAiImageDetectionRequestModel : CopyleaksAiImageDetectionRequestBase
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
        /// Base64 image: 512×512 to 6000×4500 px (27 megapixels), under 32MB; PNG, JPEG, BMP, WebP, HEIC/HEIF or AVIF.
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
    }
}
