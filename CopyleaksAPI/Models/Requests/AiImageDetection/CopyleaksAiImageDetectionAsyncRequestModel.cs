using System;
using System.Collections.Generic;
using Copyleaks.SDK.V3.API.Models.Requests.Shared;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection
{
    /// <summary>
    /// Async AI image detection of an image URL. The result is sent to <see cref="Webhooks"/>,
    /// shaped like the sync response (<c>CopyleaksAiImageDetectionResponseModel</c>).
    /// </summary>
    public class CopyleaksAiImageDetectionAsyncRequestModel
    {
        /// <summary>Public URL of the image. Required.</summary>
        [JsonProperty("url")]
        public Uri Url { get; set; }

        /// <summary>
        /// Image file name with extension, e.g. "image1.png". Required.
        /// Supported: .jpg, .jpeg, .png, .webp, .tiff, .bmp, .heic, .heif.
        /// </summary>
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// Detection model, e.g. <see cref="Constants.CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA"/>. Required.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>Webhook payload mask type: "binary" or "heatmap". Null = server default ("binary").</summary>
        [JsonProperty("maskType", NullValueHandling = NullValueHandling.Ignore)]
        public string MaskType { get; set; }

        /// <summary>Objects to locate and segment, up to 3. Do not send an empty list: it is rejected.</summary>
        [JsonProperty("objectDetection", NullValueHandling = NullValueHandling.Ignore)]
        public List<CopyleaksAiImageObjectDetectionItem> ObjectDetection { get; set; }

        /// <summary>Custom headers for fetching the image from <see cref="Url"/>, as [name, value] rows.</summary>
        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public string[,] Headers { get; set; }

        /// <summary>Where to deliver the result. Required.</summary>
        [JsonProperty("webhooks")]
        public CopyleaksAsyncWebhooks Webhooks { get; set; }

        /// <summary>Sandbox mode: mock results, no credits consumed.</summary>
        [JsonProperty("sandbox")]
        public bool Sandbox { get; set; } = false;

        /// <summary>Custom string included in the webhook payload.</summary>
        [JsonProperty("developerPayload", NullValueHandling = NullValueHandling.Ignore)]
        public string DeveloperPayload { get; set; }
    }
}
