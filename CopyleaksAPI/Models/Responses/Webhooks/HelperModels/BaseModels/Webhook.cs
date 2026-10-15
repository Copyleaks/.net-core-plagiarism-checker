using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels
{
    public class Webhook
    {
        /// <summary>The developer payload that was provided in the submit method.</summary>
        [JsonProperty("developerPayload")]

        public string DeveloperPayload { get; set; }
    }
}
