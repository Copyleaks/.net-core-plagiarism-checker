﻿/********************************************************************************
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

using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
	/// <summary>
	/// Details about the scanned document
	/// </summary>
	public class ScannedDocumentSection
	{
		/// <summary>
		/// A unique scan id provided by you.
		/// </summary>
		[JsonProperty("scanId")]
		public string ScanId { get; set; }
		/// <summary>
		/// Total number of excluded words in the document
		/// </summary>
		[JsonProperty("totalWords")]
		public uint TotalWords { get; set; }

		/// <summary>
		/// Total number of words in the document
		/// </summary>
		[JsonProperty("totalExcluded")]
		public uint TotalExcluded { get; set; }

		/// <summary>
		/// The amount of credits that will be consumed by the scan
		/// </summary>
		[JsonProperty("credits")]
		public uint Credits { get; set; }

		/// <summary>
		/// The amount of credits that will expected for full scan
		/// </summary>
		[JsonProperty("expectedCredits")]
		public uint ExpectedCredits { get; set; }

		/// <summary>
		/// The amount of credits that will be consumed by the scan
		/// </summary>
		[JsonProperty("creationTime")]
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Scanned document meta data
		/// </summary>
		[JsonProperty("metadata")]
		public ResultMetaData MetaData { get; set; }

		/// <summary>
		/// Specify whether AiDetection or PlagiarismDetection are enabled
		/// </summary>
		[JsonProperty("enabled")]
		public Enabled Enabled { get; set; }
	}
}
