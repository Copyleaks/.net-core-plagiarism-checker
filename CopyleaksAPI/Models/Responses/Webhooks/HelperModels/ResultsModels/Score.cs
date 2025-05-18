using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    public class Score
    {
        [JsonProperty("identicalWords")]
        public int IdenticalWords { get; set; }
        [JsonProperty("minorChangedWords")]
        public int MinorChangedWords { get; set; }
        [JsonProperty("relatedMeaningWords")]
        public int RelatedMeaningWords { get; set; }
        [JsonProperty("aggregatedScore")]
        public decimal AggregatedScore { get; set; }
    }
}
