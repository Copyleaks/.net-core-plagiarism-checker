using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.WritingAssistant
{
    public class Score
    {
        /// <summary>Grammar weight in the overall score, 0.0 to 1.0. Default 1.0.</summary>
        [JsonProperty("grammarScoreWeight")]
        public double GrammarScoreWeight { get; set; } = 1.0;

        /// <summary>Mechanics weight in the overall score, 0.0 to 1.0. Default 1.0.</summary>
        [JsonProperty("mechanicsScoreWeight")]
        public double MechanicsScoreWeight { get; set; } = 1.0;

        /// <summary>Sentence structure weight in the overall score, 0.0 to 1.0. Default 1.0.</summary>
        [JsonProperty("sentenceStructureScoreWeight")]
        public double SentenceStructureScoreWeight { get; set; } = 1.0;

        /// <summary>Word choice weight in the overall score, 0.0 to 1.0. Default 1.0.</summary>
        [JsonProperty("wordChoiceScoreWeight")]
        public double WordChoiceScoreWeight { get; set; } = 1.0;
    }
}
