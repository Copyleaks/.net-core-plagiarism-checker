using System;
using System.ComponentModel.DataAnnotations;
using Copyleaks.SDK.V3.API.Models.Requests.Shared;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection
{
    /// <summary>
    /// Async AI image detection of an image URL. The result is sent to <see cref="Webhooks"/>,
    /// shaped like the sync response (<c>CopyleaksAiImageDetectionResponseModel</c>).
    /// </summary>
    public class CopyleaksAiImageDetectionAsyncRequestModel : CopyleaksAiImageDetectionRequestBase
    {
        /// <summary>Public URL of the image. Required.</summary>
        [Required]
        [JsonProperty("url")]
        public Uri Url { get; set; }

        /// <summary>
        /// Image file name with extension, e.g. "image1.png". Required.
        /// Supported: .jpg, .jpeg, .png, .webp, .tiff, .bmp, .heic, .heif.
        /// </summary>
        [Required]
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        /// <summary>Custom headers for fetching the image from <see cref="Url"/>, as [name, value] rows.</summary>
        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public string[,] Headers { get; set; }

        /// <summary>Where to deliver the result. Required.</summary>
        [Required]
        [JsonProperty("webhooks")]
        public CopyleaksAsyncWebhooks Webhooks { get; set; }

        /// <summary>Custom string included in the webhook payload.</summary>
        [JsonProperty("developerPayload", NullValueHandling = NullValueHandling.Ignore)]
        public string DeveloperPayload { get; set; }
    }
}
