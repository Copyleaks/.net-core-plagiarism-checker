using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
	/// <summary>Details about the scanned document</summary>
	public class ScannedDocumentSection
	{
		/// <summary>A unique scan id provided by you.</summary>
		[JsonProperty("scanId")]
		public string ScanId { get; set; }
		/// <summary>Total number of words in the document.</summary>
		[JsonProperty("totalWords")]
		public uint TotalWords { get; set; }

		/// <summary>Number of excluded words in the document.</summary>
		[JsonProperty("totalExcluded")]
		public uint TotalExcluded { get; set; }

		/// <summary>Credits consumed by the scan.</summary>
		[JsonProperty("credits")]
		public uint Credits { get; set; }

		/// <summary>Credits expected for a full scan.</summary>
		[JsonProperty("expectedCredits")]
		public uint ExpectedCredits { get; set; }

		/// <summary>Scan creation time.</summary>
		[JsonProperty("creationTime")]
		public DateTime CreationTime { get; set; }

		/// <summary>Scanned document meta data</summary>
		[JsonProperty("metadata")]
		public ResultMetaData MetaData { get; set; }

		/// <summary>Which detections were enabled for the scan.</summary>
		[JsonProperty("enabled")]
		public Enabled Enabled { get; set; }
	}
}
