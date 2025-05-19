using System;
using System.Collections.Generic;
using System.Text;
using Copyleaks.SDK.V3.API.Models.Responses.Result;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.CompletedModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks
{
    public class CompletedWebhook : StatusWebhook
    {
        [JsonProperty("results")]
        public Results Results { get; set; }
        [JsonProperty("notifications")]
        public Notifications Notifications { get; set; }
        [JsonProperty("scannedDocument")]
        public ScannedDocument ScannedDocument { get; set; }
    }
}
