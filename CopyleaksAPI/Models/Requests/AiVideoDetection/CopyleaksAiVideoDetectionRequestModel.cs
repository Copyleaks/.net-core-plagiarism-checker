using System;
using Copyleaks.SDK.V3.API.Models.Requests.Shared;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AiVideoDetection
{
    /// <summary>
    /// AI video detection of a video URL. The result is sent to <see cref="Webhooks"/>
    /// as a <c>CopyleaksAiVideoDetectionResponseModel</c>.
    /// </summary>
    public class CopyleaksAiVideoDetectionRequestModel
    {
        /// <summary>Public URL of the video. Required.</summary>
        [JsonProperty("url")]
        public Uri Url { get; set; }

        /// <summary>Custom headers for fetching the video from <see cref="Url"/>, as [name, value] rows.</summary>
        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public string[,] Headers { get; set; }

        /// <summary>HTTP method to fetch the video: "GET", "POST" or "PUT". Null = server default ("GET").</summary>
        [JsonProperty("verb", NullValueHandling = NullValueHandling.Ignore)]
        public string Verb { get; set; }

        /// <summary>
        /// Video file name with extension, e.g. "my-video.mp4"; at most 255 characters. Required.
        /// Supported: .mp4, .avi, .mov, .mkv, .webm, .flv, .wmv, .mpg, .m4v, .3gp, .mxf.
        /// </summary>
        [JsonProperty("filename")]
        public string Filename { get; set; }

        /// <summary>
        /// Model, e.g. <see cref="Constants.CopyleaksAiVideoDetectionModels.AI_VIDEO_1_PRO"/>. Required.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

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
