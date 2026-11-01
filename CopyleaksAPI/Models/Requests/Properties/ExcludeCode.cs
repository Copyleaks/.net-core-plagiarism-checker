using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class ExcludeCode
	{
		/// <summary>Exclude code comments.</summary>
		[JsonProperty("comments")]
		public bool Comments { get; set; } = false;
	}
}
