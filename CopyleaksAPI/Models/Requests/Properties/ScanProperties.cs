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
using System;
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	/// <summary>
	/// The scan request properties
	/// </summary>
	public abstract class ScanProperties
	{
		/// <summary>
		/// Define which type of task it is.
		/// </summary>
		[JsonProperty("action")]
		public eSubmitAction Action { get; set; } = eSubmitAction.Scan;

		/// <summary>
		/// Define in which format to return the results. 
		/// </summary>
		[JsonProperty("includeHtml")]
		public bool IncludeHtml { get; set; } = false;

		/// <summary>
		/// Developer payload string.
		/// </summary>
		[JsonProperty("developerPayload")]
		[StringLength(512)]
		public string DeveloperPayload { get; set; }

		/// <summary>
		/// Scan priority
		/// </summary>
		[JsonProperty("priority")]
		public eScanPriority Priority { get; set; } = eScanPriority.Normal;

		/// <summary>
		/// Enable sandbox scan.
		/// </summary>
		[JsonProperty("sandbox")]
		public bool Sandbox { get; set; } = false;

		/// <summary>
		/// Getting the client notify changes on server.
		/// </summary>
		[JsonProperty("webhooks")]
		public Webhooks Webhooks { get; set; } = new Webhooks();

		/// <summary>
		/// How much time, in hours, to store the scan on the database.
		/// </summary>
		[JsonProperty("expiration")]
		public uint Expiration { get; set; } = 2800;

		/// <summary>
		/// Choose the algorithm goal
		/// </summary>
		[JsonProperty("scanMethodAlgorithm")]
		[Range(0, 1)]
		public eScanMethodAlgorithm ScanMethodAlgorithm { get; set; } = eScanMethodAlgorithm.MaximumCoverage;

		[JsonProperty("filters")]
		public Filters Filters { get; set; } = new Filters();

		[JsonProperty("author")]
		public AuthorEntity Author { get; set; } = new AuthorEntity();

		/// <summary>
		/// Control the level of plagiarism sensitivity that will be identified according to the speed of the scan. If you prefer a faster scan with the results that contains the highest amount of plagiarism choose 1, and if a slower, more comprehensive scan, that will also detect the smallest instances choose 5.  
		/// Range between 1 (faster ) to 5 (slower but more comprehensive)
		/// </summary>
		[JsonProperty("sensitivityLevel")]
		[Range(1, 5)]
		public int SensitivityLevel { get; set; } = 3;

		/// <summary>
		/// Enable cheatDetection scan.
		/// </summary>
		[JsonProperty("cheatDetection")]
		public bool CheatDetection { get; set; } = false;


		[JsonProperty("sensitiveDataProtection")]
		public SensitiveDataProtection SensitiveDataProtection { get; set; } = new SensitiveDataProtection();

		[JsonProperty("aiGeneratedText")]
		public AIGeneratedText AIGeneratedText { get; set; } = new AIGeneratedText(); 

		/// <summary>
		/// Add custom properties that will be attached to your document in a Copyleaks repository.
		/// </summary>
		[JsonProperty("customMetadata")]
		public CustomMetadata[] CustomMetadata { get; set; } = Array.Empty<CustomMetadata>();

		/// <summary>
		/// The language the PDF report is generated in (sent as properties.displayLanguage).
		/// Allowed values are lowercase and case-sensitive: "en", "es", "pt", "fr", "de" or "it".
		/// Any other value, such as "EN" or "en-US", is rejected by the server with HTTP 400.
		/// Only takes effect when the PDF report is created (ReportSection.Create is true).
		/// Leave it unset (null) to omit it from the request and get the server default ("en").
		/// Do not set an empty string: it is sent as-is and replaces the default.
		/// </summary>
		[JsonProperty("displayLanguage", NullValueHandling = NullValueHandling.Ignore)]
		public string DisplayLanguage { get; set; }

		/// <summary>
		/// AI Source Match: identify online sources suspected of containing AI generated text.
		/// Currently only applies to documents detected as English.
		/// Leave it unset (null) to omit it from the request and get the server default (disabled).
		/// </summary>
		[JsonProperty("aiSourceMatch", NullValueHandling = NullValueHandling.Ignore)]
		public AISourceMatch AISourceMatch { get; set; }
	}
}
