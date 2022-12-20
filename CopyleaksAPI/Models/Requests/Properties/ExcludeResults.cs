using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class ExcludeResults
	{
		/// <summary>
		/// Exclude quotes from the scan.
		/// </summary>
		[JsonProperty("idPattern")]
		public string IdPattern { get; set; } = null;
	}
}
