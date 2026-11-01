using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ErrorModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks
{
    public class ErrorWebhookModel : StatusWebhook
    {
        [JsonProperty("error")]
        public Error error {  get; set; }

    }
}
