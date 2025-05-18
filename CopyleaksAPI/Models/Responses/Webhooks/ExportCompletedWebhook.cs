using System;
using System.Collections.Generic;
using System.Text;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ExportModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks
{
    public class ExportCompletedWebhook:Webhook
    {
        [JsonProperty("completed")]
        private Boolean Completed { get; set; }
        [JsonProperty("tasks")]
        private Task[] Tasks {  get; set; }
    }
}
