using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class ExcludeResults
	{
		/// <summary>
		/// Exclude internal database and repository results whose scan id matches this pattern.
		/// "*" = any number of characters, "." = exactly one non-whitespace character.
		/// </summary>
		[JsonProperty("idPattern")]
		public string IdPattern { get; set; } = null;

		/// <summary>Exclude results with backlinks to these domains: up to 10, each 1 to 256 characters.</summary>
		[JsonProperty("backlinksDomains", NullValueHandling = NullValueHandling.Ignore)]
		public string[] BacklinksDomains { get; set; }

		/// <summary>Exclude results with text matching these phrases: up to 5, each 1 to 30 characters.</summary>
		[JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Text { get; set; }
	}
}
