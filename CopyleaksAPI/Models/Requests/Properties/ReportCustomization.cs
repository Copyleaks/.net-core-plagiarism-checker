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
		/// PDF version to generate. 
		/// By default version 1 will be generated as it our current stable version. 
		/// Version 2 is our latest iteration of our PDF report.
		/// </summary>
		public ePdfReportVersion Version { get; set; } = ePdfReportVersion.V1;
	}
}
