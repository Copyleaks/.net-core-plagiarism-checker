using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
    /// <summary>
    /// The Gen-AI Overview of a scan, as uploaded by the export overview target (ExportRequest.Overview).
    /// </summary>
    public class OverviewResult
    {
        /// <summary>
        /// A markdown summary of the scan.
        /// </summary>
        [JsonProperty("overview")]
        public string Overview { get; set; }

        /// <summary>
        /// The version of the model that generated the overview.
        /// </summary>
        [JsonProperty("modelVersion")]
        public string ModelVersion { get; set; }
    }
}
