using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
	/// <summary>Export scan artifacts to your endpoints. Every target is optional: set only those you need.</summary>
	public class ExportRequest
	{
		/// <summary>Called when the export is completed. Required by the API.</summary>
		[JsonProperty("completionWebhook")]
		public Uri CompletionWebhook { get; set; }

		/// <summary>Headers for the completion webhook, as [name, value] rows.</summary>
		[JsonProperty("completionWebhookHeaders", NullValueHandling = NullValueHandling.Ignore)]
		public string[,] CompletionWebhookHeaders { get; set; }

		/// <summary>Retries per upload before giving up, 1 to 12. Null = server default (3).</summary>
		[JsonProperty("maxRetries", NullValueHandling = NullValueHandling.Ignore)]
		public uint? MaxRetries { get; set; }

		/// <summary>Custom payload sent back in the export completed webhook.</summary>
		[JsonProperty("developerPayload", NullValueHandling = NullValueHandling.Ignore)]
		public string DeveloperPayload { get; set; }

		/// <summary>Results to export (up to 1000).</summary>
		[JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
		public ResultExportTask[] Results { get; set; }

		/// <summary>Export the crawled version of the document (a <c>DownloadSourceResponse</c>).</summary>
		[JsonProperty("crawledVersion", NullValueHandling = NullValueHandling.Ignore)]
		public ExportTask CrawledVersion { get; set; }

		/// <summary>Export the PDF report. Allowed only if the scan was submitted with PDF report enabled.</summary>
		[JsonProperty("pdfReport", NullValueHandling = NullValueHandling.Ignore)]
		public ExportTask PdfReport { get; set; }

		/// <summary>
		/// Export the AI detection report (<c>AIDetectionResult</c>). Scan needs AIGeneratedText.Detect true.
		/// </summary>
		[JsonProperty("aiDetection", NullValueHandling = NullValueHandling.Ignore)]
		public ExportTask AIDetection { get; set; }

		/// <summary>
		/// Export the Grammar Checker report (<c>WritingAssistantResult</c>). Scan needs WritingFeedback.Enable true.
		/// </summary>
		[JsonProperty("writingFeedback", NullValueHandling = NullValueHandling.Ignore)]
		public ExportTask WritingFeedback { get; set; }

		/// <summary>Export the Gen-AI Overview (<c>OverviewResult</c>). Scan needs Overview.Enable true.</summary>
		[JsonProperty("overview", NullValueHandling = NullValueHandling.Ignore)]
		public ExportTask Overview { get; set; }
	}

	public class ExportTask
	{
		/// <summary>HTTP method for the upload, e.g. "PUT" or "POST".</summary>
		[JsonProperty("verb")]
		public string Verb { get; set; }

		/// <summary>URL to upload the data to.</summary>
		[JsonProperty("endpoint")]
		public Uri Endpoint { get; set; }

		/// <summary>Headers to send with the upload, as [name, value] rows.</summary>
		[JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
		public string[,] Headers { get; set; }
	}

	public class ResultExportTask : ExportTask
	{
		/// <summary>The result id, as reported in the completed webhook.</summary>
		[JsonProperty("id")]
		public string ResultId { get; set; }
	}
}
