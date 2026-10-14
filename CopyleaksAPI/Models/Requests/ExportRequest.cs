using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
	/// <summary>
	/// Export scan artifacts to your endpoints. Every export target is optional: set only the ones you need.
	/// </summary>
	public class ExportRequest
	{
		/// <summary>
		/// Called once the export is completed (required by the API).
		/// </summary>
		[JsonProperty("completionWebhook")]
		public Uri CompletionWebhook { get; set; }

		/// <summary>
		/// Headers to add to the completion webhook, as [name, value] rows.
		/// </summary>
		[JsonProperty("completionWebhookHeaders", NullValueHandling = NullValueHandling.Ignore)]
		public string[,] CompletionWebhookHeaders { get; set; }

		/// <summary>
		/// How many times to retry each upload before giving up, from 1 to 12. The server default is 3.
		/// </summary>
		[JsonProperty("maxRetries", NullValueHandling = NullValueHandling.Ignore)]
		public uint? MaxRetries { get; set; }

		/// <summary>
		/// A custom payload that is sent back in the export completed webhook.
		/// </summary>
		[JsonProperty("developerPayload", NullValueHandling = NullValueHandling.Ignore)]
		public string DeveloperPayload { get; set; }

		/// <summary>
		/// Results to export (up to 1000).
		/// </summary>
		[JsonProperty("results", NullValueHandling = NullValueHandling.Ignore)]
		public ResultExportTask[] Results { get; set; }

		/// <summary>
		/// Export the crawled version of the submitted document (a <c>DownloadSourceResponse</c>).
		/// </summary>
		[JsonProperty("crawledVersion", NullValueHandling = NullValueHandling.Ignore)]
		public ExportTask CrawledVersion { get; set; }

		/// <summary>
		/// Export the PDF report. Allowed only when the scan was submitted with the PDF report enabled.
		/// </summary>
		[JsonProperty("pdfReport", NullValueHandling = NullValueHandling.Ignore)]
		public ExportTask PdfReport { get; set; }

		/// <summary>
		/// Export the AI content detection report (an <c>AIDetectionResult</c>).
		/// Allowed only when the scan was submitted with AIGeneratedText.Detect set to true.
		/// </summary>
		[JsonProperty("aiDetection", NullValueHandling = NullValueHandling.Ignore)]
		public ExportTask AIDetection { get; set; }

		/// <summary>
		/// Export the Grammar Checker report (a <c>WritingAssistantResult</c>).
		/// Allowed only when the scan was submitted with WritingFeedback.Enable set to true.
		/// </summary>
		[JsonProperty("writingFeedback", NullValueHandling = NullValueHandling.Ignore)]
		public ExportTask WritingFeedback { get; set; }

		/// <summary>
		/// Export the Gen-AI Overview (an <c>OverviewResult</c>).
		/// Allowed only when the scan was submitted with Overview.Enable set to true.
		/// </summary>
		[JsonProperty("overview", NullValueHandling = NullValueHandling.Ignore)]
		public ExportTask Overview { get; set; }
	}

	public class ExportTask
	{
		/// <summary>
		/// The HTTP method used to upload the data to your endpoint, for example "PUT" or "POST".
		/// </summary>
		[JsonProperty("verb")]
		public string Verb { get; set; }

		/// <summary>
		/// The URL to upload the data to.
		/// </summary>
		[JsonProperty("endpoint")]
		public Uri Endpoint { get; set; }

		/// <summary>
		/// Headers to send with the upload, as [name, value] rows.
		/// </summary>
		[JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
		public string[,] Headers { get; set; }
	}

	public class ResultExportTask : ExportTask
	{
		/// <summary>
		/// The result id, as reported in the completed webhook.
		/// </summary>
		[JsonProperty("id")]
		public string ResultId { get; set; }
	}
}
