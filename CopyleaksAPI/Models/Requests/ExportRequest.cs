using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
	public class ExportRequest
	{
		[JsonProperty("completionWebhook")]
		public Uri CompletionWebhook { get; set; }

		[JsonProperty("maxRetries")]
		public uint? MaxRetries { get; set; }

		[JsonProperty("developerPayload")]
		public string DeveloperPayload { get; set; }

		[JsonProperty("results")]
		public ResultExportTask[] Results { get; set; }

		[JsonProperty("crawledVersion")]
		public ExportTask CrawledVersion { get; set; }

		[JsonProperty("pdfReport")]
		public ExportTask PdfReport { get; set; }
	}

	public class ExportTask
	{
		[JsonProperty("verb")]
		public string Verb { get; set; }

		[JsonProperty("endpoint")]
		public Uri Endpoint { get; set; }

		[JsonProperty("headers")]
		public string[,] Headers { get; set; }
	}

	public class ResultExportTask : ExportTask
	{
		[JsonProperty("id")]
		public string ResultId { get; set; }
	}
}
