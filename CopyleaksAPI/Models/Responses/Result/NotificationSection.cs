using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
	public class NotificationSection
	{
		[JsonProperty("alerts")]
		public AlertNotification[] Alerts { get; set; }
	}
}
