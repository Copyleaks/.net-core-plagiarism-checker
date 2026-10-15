using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	/// <summary>PDF report customization.</summary>
	public class ReportCustomization
	{
		/// <summary>Generate a PDF report.</summary>
		public bool Create { get; set; } = false;

		/// <summary>Report title.</summary>
		public string Title { get; set; }

		/// <summary>Header logo, as a base64 string.</summary>
		public string LargeLogo { get; set; }

		/// <summary>Footer logo, as a base64 string.</summary>
		public string SmallLogo { get; set; }

		/// <summary>Right-to-left text direction.</summary>
		public bool RTL { get; set; }

		/// <summary>Report colors.</summary>
		public ReportCustomizationColors Colors { get; set; }

		/// <summary>Deprecated: use ReportVersion. PDF version to generate. Null = server default (latest).</summary>
		[Obsolete("pdf.version is deprecated by the API. Use ReportVersion (\"v1\", \"v2\", \"v3\" or \"latest\") instead.")]
		[JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
		public ePdfReportVersion? Version { get; set; }

		/// <summary>
		/// PDF report version: "v1", "v2", "v3" or "latest" (lowercase; anything else, even "", gets HTTP 400).
		/// Used only when Create is true; overrides Version. Null = Version if set, else server default ("latest").
		/// </summary>
		[JsonProperty("reportVersion", NullValueHandling = NullValueHandling.Ignore)]
		public string ReportVersion { get; set; }
	}
}
