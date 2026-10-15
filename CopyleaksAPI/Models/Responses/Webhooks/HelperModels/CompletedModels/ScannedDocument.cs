using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.CompletedModels
{
    public class ScannedDocument
    {
        /// <summary>The unique scan id provided by you.</summary>
        [JsonProperty("scanId")]
        public string ScanId;

        /// <summary>Total number of words found in the scanned text.</summary>
        [JsonProperty("totalWords")]
        public int TotalWords;

        /// <summary>Number of excluded words in the submitted content.</summary>
        [JsonProperty("totalExcluded")]
        public int TotalExcluded;

        /// <summary>Overall credits used for the scan.</summary>
        [JsonProperty("credits")]
        public int Credits;

        /// <summary>The creation time of the scan.</summary>
        [JsonProperty("creationTime")]
        public string CreationTime;

        /// <summary>Metadata object</summary>
        [JsonProperty("metadata")]
        public Metadata Metadata;
    }
}
