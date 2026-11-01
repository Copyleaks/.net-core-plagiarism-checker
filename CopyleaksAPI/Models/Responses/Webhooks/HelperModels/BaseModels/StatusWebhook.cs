using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels
{
    public class StatusWebhook : Webhook 
    {
        /// <summary>The current status of the scan.</summary>
        [JsonProperty("status")]
        public int Status { get; set; }
    }
}
