using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{

	public class Language
	{
		/// <summary>
		/// Language code for cross language plagiarism detection.
		/// </summary>
		[JsonProperty("code")]
		public string Code { get; set; }
	}
}
