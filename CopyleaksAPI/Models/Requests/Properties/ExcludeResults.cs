using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class ExcludeResults
	{
		/// <summary>
		/// Exclude internal database and repository results whose scan id matches this pattern.
		/// "*" matches any number of characters and "." matches exactly one non-whitespace character.
		/// </summary>
		[JsonProperty("idPattern")]
		public string IdPattern { get; set; } = null;

		/// <summary>
		/// Exclude results that contain backlinks to these domains. Up to 10 domains, each 1 to 256 characters.
		/// Leave it unset (null) to omit it from the request.
		/// </summary>
		[JsonProperty("backlinksDomains", NullValueHandling = NullValueHandling.Ignore)]
		public string[] BacklinksDomains { get; set; }

		/// <summary>
		/// Exclude results that contain text matching these phrases. Up to 5 phrases, each 1 to 30 characters.
		/// Leave it unset (null) to omit it from the request.
		/// </summary>
		[JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Text { get; set; }
	}
}
