using System.Collections.Generic;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ErrorModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Shared
{
    /// <summary>
    /// Why an asynchronous AI image or AI video detection failed. It is posted to your webhook in place of the result.
    /// </summary>
    public class CopyleaksAsyncDetectionError
    {
        /// <summary>
        /// The error type, for example "invalid_request_error" or "api_error".
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// The machine-readable error identifier.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The numeric error code.
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// A human-readable description of the error.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// A URL to the documentation page for this error, or null.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// The request parameters that caused the error, or null.
        /// </summary>
        [JsonProperty("details")]
        public List<ErrorDetail> Details { get; set; }
    }
}
