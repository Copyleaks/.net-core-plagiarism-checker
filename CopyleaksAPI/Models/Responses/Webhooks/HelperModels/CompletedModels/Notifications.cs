using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NotificationsModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.CompletedModels
{
    public class Notifications
    {
        [JsonProperty("alerts")]
        public Alerts[] Alerts { get; set; }
    }
}
