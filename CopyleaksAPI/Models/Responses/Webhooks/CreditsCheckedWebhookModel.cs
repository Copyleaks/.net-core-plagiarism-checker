using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.CompletedModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks
{
    public class CreditsCheckedWebhookModel : StatusWebhook
    {
        [JsonProperty("credits")]
        public int Credits { get; set; }
        [JsonProperty("scannedDocument")]
        public ScannedDocument ScannedDocument { get; set; }
    }
}
