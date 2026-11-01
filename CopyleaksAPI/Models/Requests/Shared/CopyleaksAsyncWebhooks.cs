using System;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Shared
{
    /// <summary>Where Copyleaks delivers the result of an async AI image or AI video detection.</summary>
    public class CopyleaksAsyncWebhooks
    {
        /// <summary>URL that Copyleaks calls with POST and the detection result. Required.</summary>
        [JsonProperty("url")]
        public Uri Url { get; set; }

        /// <summary>Custom headers for the webhook request, as [name, value] rows.</summary>
        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public string[,] Headers { get; set; }
    }
}
