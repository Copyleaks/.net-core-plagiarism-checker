using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	/// <summary>Base scan request properties.</summary>
	public abstract class ScanProperties
	{
		/// <summary>Task type.</summary>
		[JsonProperty("action")]
		public eSubmitAction Action { get; set; } = eSubmitAction.Scan;

		/// <summary>Format to return the results in.</summary>
		[JsonProperty("includeHtml")]
		public bool IncludeHtml { get; set; } = false;

		/// <summary>Developer payload string.</summary>
		[JsonProperty("developerPayload")]
		[StringLength(512)]
		public string DeveloperPayload { get; set; }

		/// <summary>Scan priority. Null = server default.</summary>
		[JsonProperty("priority", NullValueHandling = NullValueHandling.Ignore)]
		public eScanPriority? Priority { get; set; } = null;

		/// <summary>Enable sandbox scan.</summary>
		[JsonProperty("sandbox")]
		public bool Sandbox { get; set; } = false;

		/// <summary>Callbacks that notify you of scan changes.</summary>
		[JsonProperty("webhooks")]
		public Webhooks Webhooks { get; set; } = new Webhooks();

		/// <summary>Hours to store the scan, 1 to 2880. Default 2880 (120 days, the API maximum).</summary>
		[JsonProperty("expiration")]
		public uint Expiration { get; set; } = 2880;

		/// <summary>Algorithm goal.</summary>
		[JsonProperty("scanMethodAlgorithm")]
		[Range(0, 1)]
		public eScanMethodAlgorithm ScanMethodAlgorithm { get; set; } = eScanMethodAlgorithm.MaximumCoverage;

		[JsonProperty("filters")]
		public Filters Filters { get; set; } = new Filters();

		[JsonProperty("author")]
		public AuthorEntity Author { get; set; } = new AuthorEntity();

		/// <summary>Plagiarism sensitivity, 1 (faster) to 5 (slower, also detects the smallest instances).</summary>
		[JsonProperty("sensitivityLevel")]
		[Range(1, 5)]
		public int SensitivityLevel { get; set; } = 3;

		/// <summary>Enable cheat detection.</summary>
		[JsonProperty("cheatDetection")]
		public bool CheatDetection { get; set; } = false;


		[JsonProperty("sensitiveDataProtection")]
		public SensitiveDataProtection SensitiveDataProtection { get; set; } = new SensitiveDataProtection();

		[JsonProperty("aiGeneratedText")]
		public AIGeneratedText AIGeneratedText { get; set; } = new AIGeneratedText(); 

		/// <summary>Custom properties attached to your document in a Copyleaks repository.</summary>
		[JsonProperty("customMetadata")]
		public CustomMetadata[] CustomMetadata { get; set; } = Array.Empty<CustomMetadata>();

		/// <summary>
		/// PDF report language: "en", "es", "pt", "fr", "de" or "it" (lowercase; anything else gets HTTP 400).
		/// Used only when the PDF report is created. Null = server default ("en"); do not send an empty string.
		/// </summary>
		[JsonProperty("displayLanguage", NullValueHandling = NullValueHandling.Ignore)]
		public string DisplayLanguage { get; set; }

		/// <summary>AI Source Match settings. Null = server default (disabled).</summary>
		[JsonProperty("aiSourceMatch", NullValueHandling = NullValueHandling.Ignore)]
		public AISourceMatch AISourceMatch { get; set; }

		/// <summary>Internal AI Source Match settings. Null = server default (disabled).</summary>
		[JsonProperty("internalAiSourceMatch", NullValueHandling = NullValueHandling.Ignore)]
		public InternalAISourceMatch InternalAISourceMatch { get; set; }

		/// <summary>Citation Checker settings. Null = server default (disabled).</summary>
		[JsonProperty("references", NullValueHandling = NullValueHandling.Ignore)]
		public ReferencesSettings References { get; set; }

		/// <summary>Grammar Checker settings. Null = server default (disabled).</summary>
		[JsonProperty("writingFeedback", NullValueHandling = NullValueHandling.Ignore)]
		public WritingFeedbackSettings WritingFeedback { get; set; }

		/// <summary>Gen-AI Overview settings. Null = server default (disabled).</summary>
		[JsonProperty("overview", NullValueHandling = NullValueHandling.Ignore)]
		public OverviewSettings Overview { get; set; }

		/// <summary>
		/// IANA time zone (case-sensitive), e.g. "America/New_York", for the scan time on the PDF report.
		/// Null = the user's country time zone, or UTC.
		/// </summary>
		[JsonProperty("scanTimeZone", NullValueHandling = NullValueHandling.Ignore)]
		public string ScanTimeZone { get; set; }

		/// <summary>Course of the submitted content, for analytics.</summary>
		[JsonProperty("course", NullValueHandling = NullValueHandling.Ignore)]
		public CourseEntity Course { get; set; }

		/// <summary>Assignment of the submitted content, for analytics.</summary>
		[JsonProperty("assignment", NullValueHandling = NullValueHandling.Ignore)]
		public AssignmentEntity Assignment { get; set; }

		/// <summary>Institution of the submitted content, for analytics.</summary>
		[JsonProperty("institution", NullValueHandling = NullValueHandling.Ignore)]
		public InstitutionEntity Institution { get; set; }
	}
}
