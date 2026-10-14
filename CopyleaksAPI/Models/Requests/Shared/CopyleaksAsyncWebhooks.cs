using System;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Shared
{
    /// <summary>
    /// Where Copyleaks delivers the result of an asynchronous AI image or AI video detection.
    /// </summary>
    public class CopyleaksAsyncWebhooks
    {
        /// <summary>
        /// The URL that Copyleaks calls with POST and the detection result. Required.
        /// </summary>
        [JsonProperty("url")]
        public Uri Url { get; set; }

        /// <summary>
        /// Custom headers to add to the webhook request, as [name, value] rows.
        /// Leave it unset (null) to omit it from the request.
        /// </summary>
        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public string[,] Headers { get; set; }
    }
}
