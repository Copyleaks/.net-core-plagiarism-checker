using System;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ExportModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks
{
    public class ExportCompletedWebhook:Webhook
    {
        [JsonProperty("completed")]
        public Boolean Completed { get; set; }
        [JsonProperty("tasks")]
        public Task[] Tasks {  get; set; }
    }
}
