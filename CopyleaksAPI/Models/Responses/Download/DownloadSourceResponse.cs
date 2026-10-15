using Copyleaks.SDK.V3.API.Models.Responses.CitationChecker;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
    public class DownloadSourceResponse
    {
        [JsonProperty("metadata")]
        public MetadataSection Metadata { get; set; }

        [JsonProperty("html")]
        public FullHtmlSourceSubVersionSection Html { get; set; }

        [JsonProperty("text")]
        public FullTextSourceSubVersionSection Text { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }

        /// <summary>Citation Checker summary and per-reference results (when References.Validate was true).</summary>
        [JsonProperty("referencesValidation")]
        public ReferencesValidation ReferencesValidation { get; set; }
    }
}