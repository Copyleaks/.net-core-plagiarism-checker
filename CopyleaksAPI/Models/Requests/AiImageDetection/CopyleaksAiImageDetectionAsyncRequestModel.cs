using System;
using System.Collections.Generic;
using Copyleaks.SDK.V3.API.Models.Requests.Shared;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection
{
    /// <summary>
    /// Request model for asynchronous AI image detection of an image URL.
    /// The result is delivered to <see cref="Webhooks"/> with the same shape as the synchronous response
    /// (<c>CopyleaksAiImageDetectionResponseModel</c>).
    /// </summary>
    public class CopyleaksAiImageDetectionAsyncRequestModel
    {
        /// <summary>
        /// A publicly accessible URL of the image to analyze. Required.
        /// </summary>
        [JsonProperty("url")]
        public Uri Url { get; set; }

        /// <summary>
        /// The image file name including its extension, for example "image1.png". Required.
        /// Supported extensions: .jpg, .jpeg, .png, .webp, .tiff, .bmp, .heic, .heif.
        /// </summary>
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// The AI image detection model, for example <see cref="Constants.CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA"/>. Required.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// The type of detection mask in the webhook payload: "binary" (the default) or "heatmap".
        /// Leave it unset (null) to omit it from the request.
        /// </summary>
        [JsonProperty("maskType", NullValueHandling = NullValueHandling.Ignore)]
        public string MaskType { get; set; }

        /// <summary>
        /// Objects to locate and segment in the image. Up to 3 entries.
        /// Leave it unset (null) to omit it from the request. Do not send an empty list: the server rejects it.
        /// </summary>
        [JsonProperty("objectDetection", NullValueHandling = NullValueHandling.Ignore)]
        public List<CopyleaksAiImageObjectDetectionItem> ObjectDetection { get; set; }

        /// <summary>
        /// Custom headers for fetching the image from <see cref="Url"/>, as [name, value] rows.
        /// </summary>
        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public string[,] Headers { get; set; }

        /// <summary>
        /// Where to deliver the result. Required.
        /// </summary>
        [JsonProperty("webhooks")]
        public CopyleaksAsyncWebhooks Webhooks { get; set; }

        /// <summary>
        /// Use sandbox mode to test your integration without consuming credits. Returns mock results.
        /// </summary>
        [JsonProperty("sandbox")]
        public bool Sandbox { get; set; } = false;

        /// <summary>
        /// A custom string that is included in the webhook payload.
        /// </summary>
        [JsonProperty("developerPayload", NullValueHandling = NullValueHandling.Ignore)]
        public string DeveloperPayload { get; set; }
    }
}
