using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// Gen-AI Overview settings (sent as properties.overview).
    /// Export the overview with <see cref="ExportRequest.Overview"/>.
    /// Every flag left unset (null) is omitted from the request, so the server default (false) applies.
    /// </summary>
    public class OverviewSettings
    {
        /// <summary>
        /// Enable the Gen-AI Overview, which extracts key insights from the scan data.
        /// </summary>
        [JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enable { get; set; }

        /// <summary>
        /// Ignore AI detection when generating the overview.
        /// </summary>
        [JsonProperty("ignoreAIDetection", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IgnoreAIDetection { get; set; }

        /// <summary>
        /// Ignore plagiarism detection when generating the overview.
        /// </summary>
        [JsonProperty("ignorePlagiarismDetection", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IgnorePlagiarismDetection { get; set; }

        /// <summary>
        /// Ignore the Grammar Checker when generating the overview.
        /// </summary>
        [JsonProperty("ignoreWritingFeedback", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IgnoreWritingFeedback { get; set; }

        /// <summary>
        /// Ignore the author's historical data when generating the overview.
        /// </summary>
        [JsonProperty("ignoreAuthorData", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IgnoreAuthorData { get; set; }
    }
}
