using System;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	/// <summary>What to exclude from the scan.</summary>
	public class Exclude
	{
		/// <summary>Exclude quotes.</summary>
		[JsonProperty("quotes")]
		public bool Quotes { get; set; } = false;

		/// <summary>Exclude citations.</summary>
		[JsonProperty("citations")]
		public bool Citations { get; set; } = false;

		/// <summary>Exclude titles.</summary>
		[JsonProperty("titles")]
		public bool Titles { get; set; } = false;

		/// <summary>For HTML documents, exclude HTML tags and attribute metadata.</summary>
		[JsonProperty("htmlTemplate")]
		public bool HtmlTemplate { get; set; } = false;

		/// <summary>Exclude the table of contents.</summary>
		[JsonProperty("tableOfContents")]
		public bool TableOfContents { get; set; } = false;

		/// <summary>Exclude text that is found in these other documents.</summary>
		[JsonProperty("documentTemplateIds")]
		public string[] DocumentTemplateIds { get; set; } = Array.Empty<string>();

        /// <summary>Source code exclusions.</summary>
		[JsonProperty("code")]
        public ExcludeCode Code { get; set; } = new ExcludeCode();
	}

	/// <summary>What to exclude from the scan, including references.</summary>
	public class ClientExclude : Exclude
	{
		/// <summary>Exclude references.</summary>
		[JsonProperty("references")]
		public bool References { get; set; } = false;
	}
}
