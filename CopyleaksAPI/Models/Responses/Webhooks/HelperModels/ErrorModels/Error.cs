using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ErrorModels
{
    public class Error
    {
        /// <summary>
        /// Error type, e.g. "authentication_error", "payment_error", "invalid_request_error" or "api_error".
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>Machine-readable error id, e.g. "invalid_credentials" or "insufficient_credits".</summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>Error code of the failure reason.</summary>
        [JsonProperty("code")]
        public int Code {  get; set; }

        /// <summary>Error message of the failure reason.</summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>A URL to the documentation page for this error.</summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>The request parameters that caused the error.</summary>
        [JsonProperty("details")]
        public ErrorDetail[] Details { get; set; }
    }
}
