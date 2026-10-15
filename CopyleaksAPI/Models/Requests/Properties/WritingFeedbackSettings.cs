using Copyleaks.SDK.V3.API.Models.Requests.WritingAssistant;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>Grammar Checker settings; export the report with <see cref="ExportRequest.WritingFeedback"/>.</summary>
    public class WritingFeedbackSettings
    {
        /// <summary>
        /// Enable the Grammar Checker (grammar, spelling, sentence structure and word choice).
        /// Null = server default (false).
        /// </summary>
        [JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enable { get; set; }

        /// <summary>Weight of each correction category in the overall writing score.</summary>
        [JsonProperty("score", NullValueHandling = NullValueHandling.Ignore)]
        public Score Score { get; set; }
    }
}
