using System;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NotificationsModels
{
    public class Alerts
    {
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("helpLink")]
        public string HelpLink { get; set; }
        [JsonProperty("severity")]
        public string Severity { get; set; }
        [JsonProperty("additionalData")]
        public string AdditionalData { get; set; }
    }
}