using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
    public class BaseResponse
    {
        [JsonProperty("status")]
        public eScanStatus Status { get; set; }

        [JsonProperty("error")]
        public ErrorSection Error { get; set; }

        [JsonProperty("developerPayload")]
        public string Payload { get; set; }

        [JsonProperty("notifications")]
        public NotificationSection Notifications { get; set; }
    }
}
