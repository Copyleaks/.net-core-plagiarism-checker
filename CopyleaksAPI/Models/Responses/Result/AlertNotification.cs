using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Responses.AIDetector;
using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
	public class AlertNotification
	{
		[JsonProperty("additionalData")]
		public string AdditionalData { get; set; }

		[JsonProperty("code")]
		public string Code { get; set; }

		[JsonProperty("helpLink")]
		public string HelpLink { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("severity")]
		public eAlertSeverity Severity { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		/// <summary>
		/// Decodes the additionalData of a "suspected-ai-text" alert (trailing NUL and ASCII whitespace are ignored).
		/// Null for other alerts, no data (null, empty, or only NUL/whitespace), or valid JSON that is not an object.
		/// </summary>
		/// <exception cref="JsonException">AdditionalData is not valid JSON.</exception>
		public AIDetectionResult GetAIDetectionResult()
		{
			return AIDetectionAlertParser.Parse(Code, AdditionalData);
		}
	}
}
