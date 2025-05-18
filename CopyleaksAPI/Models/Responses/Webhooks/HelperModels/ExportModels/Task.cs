using System;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ExportModels
{
    public class Task
    {
        [JsonProperty("endpoint")]
        public string Endpoint {  get; set; }
        [JsonProperty("isHealthy")]
        public Boolean IsHealthy { get; set; }
        [JsonProperty("httpStatusCode")]
        public int HttpStatusCode { get; set; }
    }
}
