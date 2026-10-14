using Copyleaks.SDK.V3.API.Models.Requests.WritingAssistant;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// Grammar Checker settings for a scan (sent as properties.writingFeedback).
    /// Export the Grammar Checker report with <see cref="ExportRequest.WritingFeedback"/>.
    /// </summary>
    public class WritingFeedbackSettings
    {
        /// <summary>
        /// Enable the automated Grammar Checker for grammar, spelling, sentence structure and word choice.
        /// Leave it unset (null) to omit it from the request and get the server default (false).
        /// </summary>
        [JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enable { get; set; }

        /// <summary>
        /// The weight of each correction category in the overall writing score. Every weight defaults to 1.0.
        /// Leave it unset (null) to omit it from the request.
        /// </summary>
        [JsonProperty("score", NullValueHandling = NullValueHandling.Ignore)]
        public Score Score { get; set; }
    }
}
