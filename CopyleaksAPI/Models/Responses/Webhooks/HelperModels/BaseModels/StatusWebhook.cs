using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels
{
    public class StatusWebhook : Webhook 
    {
        [JsonProperty("status")]
        public int Status { get; set; }
    }
}
