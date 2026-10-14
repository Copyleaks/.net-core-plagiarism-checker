using System;
using Copyleaks.SDK.V3.API.Models.Requests.Shared;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AiVideoDetection
{
    /// <summary>
    /// Request model for AI video detection of a video URL.
    /// The result is delivered to <see cref="Webhooks"/> as a <c>CopyleaksAiVideoDetectionResponseModel</c>.
    /// </summary>
    public class CopyleaksAiVideoDetectionRequestModel
    {
        /// <summary>
        /// A publicly accessible URL of the video file to analyze. Required.
        /// </summary>
        [JsonProperty("url")]
        public Uri Url { get; set; }

        /// <summary>
        /// Custom headers for fetching the video from <see cref="Url"/>, as [name, value] rows.
        /// </summary>
        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public string[,] Headers { get; set; }

        /// <summary>
        /// The HTTP method used to fetch the video: "GET", "POST" or "PUT".
        /// Leave it unset (null) to omit it from the request and get the server default ("GET").
        /// </summary>
        [JsonProperty("verb", NullValueHandling = NullValueHandling.Ignore)]
        public string Verb { get; set; }

        /// <summary>
        /// The video file name including its extension, at most 255 characters, for example "my-video.mp4". Required.
        /// Supported extensions: .mp4, .avi, .mov, .mkv, .webm, .flv, .wmv, .mpg, .m4v, .3gp, .mxf.
        /// </summary>
        [JsonProperty("filename")]
        public string Filename { get; set; }

        /// <summary>
        /// The AI video detection model, for example <see cref="Constants.CopyleaksAiVideoDetectionModels.AI_VIDEO_1_PRO"/>. Required.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

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
