using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NotificationsModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.CompletedModels
{
    public class Notifications
    {
        /// <summary>A list of scan alerts that were detected in the scan.</summary>
        [JsonProperty("alerts")]
        public Alerts[] Alerts { get; set; }
    }
}
