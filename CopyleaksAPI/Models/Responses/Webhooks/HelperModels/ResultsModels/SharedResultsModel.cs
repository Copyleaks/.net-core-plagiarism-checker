using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    public class SharedResultsModel
    {
        /// <summary>Unique result ID to identify this result.</summary>
        [JsonProperty("id")]
        public string Id {  get; set; }

        /// <summary>Document title. Mostly extracted from the document content.</summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>Document brief introduction. Mostly extracted from the document content.</summary>
        [JsonProperty("introduction")]
        public string Introduction { get; set; }

        /// <summary>Total matched words between this result and the scanned document.</summary>
        [JsonProperty("matchedWords")]
        public int MatchedWords {  get; set; }

        /// <summary>Scan id of a Copyleaks internal database result that you submitted; otherwise empty.</summary>
        [JsonProperty("scanId")]
        public string ScanId { get; set; }

        /// <summary>Metadata object.</summary>
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

    }
}
