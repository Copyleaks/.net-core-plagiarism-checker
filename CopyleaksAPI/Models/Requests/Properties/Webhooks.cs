using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	/// <summary>Your HTTP callback endpoints.</summary>
	public class Webhooks
	{
		/// <summary>Called on every scan status change; use the {STATUS} token in the URL.</summary>
		[JsonProperty("status")]
		public Uri Status { get; set; }

		/// <summary>Called each time a new result is found for the scan.</summary>
		[JsonProperty("newResult")]
		public Uri NewResult { get; set; }

		[JsonProperty("newResultHeaders")]
		public string[,] NewResultHeaders { get; set; } = null;

		[JsonProperty("statusHeaders")]
		public string[,] StatusHeaders { get; set; } = null;
	}
}
