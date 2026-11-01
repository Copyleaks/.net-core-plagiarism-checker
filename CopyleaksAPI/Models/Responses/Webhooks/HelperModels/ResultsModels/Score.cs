using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    public class Score
    {
        /// <summary>Number of words which matched exactly</summary>
        [JsonProperty("identicalWords")]
        public int IdenticalWords { get; set; }

        /// <summary>Number of nearly identical words with small differences like 'slow' and 'slowly'.</summary>
        [JsonProperty("minorChangedWords")]
        public int MinorChangedWords { get; set; }

        /// <summary>Number of paraphrased words showing similar ideas with different words.</summary>
        [JsonProperty("relatedMeaningWords")]
        public int RelatedMeaningWords { get; set; }

        /// <summary>Aggregated similar-word percentage, not counting excluded references, quotations, etc.</summary>
        [JsonProperty("aggregatedScore")]
        public decimal AggregatedScore { get; set; }
    }
}
