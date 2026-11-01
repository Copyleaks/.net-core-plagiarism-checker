using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ErrorModels
{
    /// <summary>A request parameter that caused the error.</summary>
    public class ErrorDetail
    {
        /// <summary>The parameter name.</summary>
        [JsonProperty("param")]
        public string Param { get; set; }

        /// <summary>What is wrong with the parameter.</summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
