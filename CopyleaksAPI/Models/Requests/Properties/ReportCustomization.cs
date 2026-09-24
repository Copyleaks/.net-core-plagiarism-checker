/********************************************************************************
 The MIT License(MIT)
 
 Copyright(c) 2016 Copyleaks LTD (https://copyleaks.com)
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
********************************************************************************/

using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	/// <summary>
	/// Specify customized attributes for Copyleaks scan results report
	/// </summary>
	public class ReportCustomization
	{
		/// <summary>
		/// When set to true a pdf report will be generated
		/// </summary>
		public bool Create { get; set; } = false;

		/// <summary>
		/// Customizable title
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Customizable logo for the header of the report.
		/// The logo should be in string base64 format
		/// </summary>
		public string LargeLogo { get; set; }

		/// <summary>
		/// Customizable logo for the footer of the report.
		/// The logo should be in string base64 format
		/// </summary>
		public string SmallLogo { get; set; }

		/// <summary>
		/// Customizable direction of text.
		/// When set to true the direction of the text will be from right to lef
		/// </summary>
		public bool RTL { get; set; }

		/// <summary>
		/// Customizable colors
		/// </summary>
		public ReportCustomizationColors Colors { get; set; }

		/// <summary>
		/// Legacy PDF report version to generate (sent as the integer pdf.version).
		/// This SDK always sends it, and the default is V1. This enum only offers V1 and V2.
		/// Ignored by the server when ReportVersion is set.
		/// To get the newest report, set ReportVersion to "v3" or "latest" instead.
		/// Only takes effect when Create is true.
		/// </summary>
		public ePdfReportVersion Version { get; set; } = ePdfReportVersion.V1;

		/// <summary>
		/// PDF report version to generate (sent as pdf.reportVersion).
		/// Allowed values are lowercase and case-sensitive: "v1", "v2", "v3" or "latest".
		/// Any other value, such as "V3", "Latest" or "", is rejected by the server with HTTP 400.
		/// Only takes effect when Create is true.
		/// When set, it overrides Version.
		/// Leave it unset (null) to omit it from the request. The server then uses Version,
		/// which this SDK always sends (default V1), so leaving both unset yields the v1 report.
		/// </summary>
		[JsonProperty("reportVersion", NullValueHandling = NullValueHandling.Ignore)]
		public string ReportVersion { get; set; }
	}
}
