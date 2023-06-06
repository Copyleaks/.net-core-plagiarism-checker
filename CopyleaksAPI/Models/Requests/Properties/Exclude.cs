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

using System;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	/// <summary>
	/// Exclude properties from scan
	/// </summary>
	public class Exclude
	{
		/// <summary>
		/// Exclude quotes from the scan.
		/// </summary>
		[JsonProperty("quotes")]
		public bool Quotes { get; set; } = false;

		/// <summary>
		/// Exclude citations from the scan.
		/// </summary>
		[JsonProperty("citations")]
		public bool Citations { get; set; } = false;

		/// <summary>
		/// Exclude titles from the scan.
		/// </summary>
		[JsonProperty("titles")]
		public bool Titles { get; set; } = false;

		/// <summary>
		/// When HTML document, exclude the HTML tags and attribute metadata from the scan. 
		/// </summary>
		[JsonProperty("htmlTemplate")]
		public bool HtmlTemplate { get; set; } = false;

		/// <summary>
		/// Exclude table of contents from the scan. 
		/// </summary>
		[JsonProperty("tableOfContents")]
		public bool TableOfContents { get; set; } = false;

		/// <summary>
		/// Exclude text based on text found within other documents
		/// </summary>
		[JsonProperty("documentTemplateIds")]
		public string[] DocumentTemplateIds { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Exclude sections of source code
        /// </summary>
		[JsonProperty("code")]
        public ExcludeCode Code { get; set; } = new ExcludeCode();
	}

	/// <summary>
	/// Exclude properties from scan
	/// </summary>
	public class ClientExclude : Exclude
	{
		/// <summary>
		/// Exclude references from the scan.
		/// </summary>
		[JsonProperty("references")]
		public bool References { get; set; } = false;
	}
}
