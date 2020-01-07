using Newtonsoft.Json;
using System.Collections.Generic;

namespace Copyleaks.SDK.V3.API.Models.Callbacks
{
	/// <summary>
	/// Callback payload that will be sent from Copyleaks' api once an export request is done 
	/// </summary>
	public class ExportCompletedCallback
	{
		[JsonProperty("completed")]
		public bool Completed { get; set; }

		[JsonProperty("tasks")]
		public IEnumerable<ExportRow> Tasks { get; set; }

		[JsonProperty("developerPayload")]
		public string DeveloperPayload { get; set; }
	}

	/// <summary>
	/// Represents a summary of a single export task
	/// </summary>
	public class ExportRow
	{
		[JsonProperty("endpoint")]
		public string Endpoint { get; set; }

		[JsonProperty("httpStatusCode")]
		public uint HttpStatusCode { get; set; }

		[JsonProperty("isHealthy")]
		public bool IsHealthy { get; set; }
	}
}
