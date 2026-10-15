using System;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ExportModels
{
    public class Task
    {
        /// <summary>The endpoint address of the export task.</summary>
        [JsonProperty("endpoint")]
        public string Endpoint {  get; set; }

        /// <summary>Whether the scan completed without internal errors on the Copyleaks side.</summary>
        [JsonProperty("isHealthy")]
        public Boolean IsHealthy { get; set; }

        /// <summary>Status code returned by your server; null when IsHealthy is false.</summary>
        [JsonProperty("httpStatusCode")]
        public int? HttpStatusCode { get; set; }
    }
}
