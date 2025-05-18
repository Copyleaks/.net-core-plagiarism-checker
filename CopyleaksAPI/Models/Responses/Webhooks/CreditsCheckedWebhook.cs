using System;
using System.Collections.Generic;
using System.Text;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.CompletedModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks
{
    public class CreditsCheckedWebhook : StatusWebhook
    {
        [JsonProperty("credits")]
        private int Credits { get; set; }
        [JsonProperty("scannedDocument")]
        private ScannedDocument ScannedDocument { get; set; }
    }
}
