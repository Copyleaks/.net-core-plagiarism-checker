using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
    /// <summary>The score for the scan</summary>
    public class ScoreSection
    {

        /// <summary>Number of exact words in the text</summary>
        [JsonProperty("identicalWords")]
        public uint IdenticalWords { get; set; }

        /// <summary>Number of nearly identical words with small differences like *slow* becomes *slowly*</summary>
        [JsonProperty("minorChangedWords")]
        public uint MinorChangedWords { get; set; }

        /// <summary>Number of paraphrased words stating similar ideas with different words</summary>
        [JsonProperty("relatedMeaningWords")]
        public uint RelatedMeaningWords { get; set; }

        /// <summary>Aggregated similar-word percentage, not counting excluded references, quotations, etc.</summary>
        [JsonProperty("aggregatedScore")]
        public double AggregatedScore { get; set; }
    }
}