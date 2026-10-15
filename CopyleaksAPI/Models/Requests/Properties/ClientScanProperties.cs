using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	/// <summary>Scan request properties.</summary>
	public class ClientScanProperties : ScanProperties
	{
		[JsonProperty("scanning")]
		public ClientProtectionLayers ProtectionLayersSection { get; set; } = new ClientProtectionLayers();

		[JsonProperty("exclude")]
		public ClientExclude Exclude { get; set; } = new ClientExclude();

		[JsonProperty("pdf")]
		public ReportCustomization ReportSection { get; set; } = new ReportCustomization();

		[JsonProperty("indexing")]
		public ClientIndexingEntity Indexing { get; set; } = new ClientIndexingEntity();
	}
}
