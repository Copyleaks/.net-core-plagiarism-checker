using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.WritingAssistant
{
    public class Score
    {
        [JsonProperty("corrections")]
        public CorrectionsScore Corrections { get; set; }

        [JsonProperty("readability")]
        public ReadabilityScore Readability { get; set; }

        [JsonProperty("statistics")]
        public DocumentStatistics Statistics { get; set; }
    }
    public class CorrectionsScore
    {
        [JsonProperty("grammarCorrectionsCount")]
        public int GrammarCorrectionsCount { get; set; }

        [JsonProperty("grammarCorrectionsScore")]
        public int GrammarCorrectionsScore { get; set; }

        [JsonProperty("grammarScoreWeight")]
        public double GrammarScoreWeight { get; set; }

        [JsonProperty("mechanicsCorrectionsCount")]
        public int MechanicsCorrectionsCount { get; set; }

        [JsonProperty("mechanicsCorrectionsScore")]
        public int MechanicsCorrectionsScore { get; set; }

        [JsonProperty("mechanicsScoreWeight")]
        public double MechanicsScoreWeight { get; set; }

        [JsonProperty("sentenceStructureCorrectionsCount")]
        public int SentenceStructureCorrectionsCount { get; set; }

        [JsonProperty("sentenceStructureCorrectionsScore")]
        public int SentenceStructureCorrectionsScore { get; set; }

        [JsonProperty("sentenceStructureScoreWeight")]
        public double SentenceStructureScoreWeight { get; set; }

        [JsonProperty("wordChoiceCorrectionsCount")]
        public int WordChoiceCorrectionsCount { get; set; }

        [JsonProperty("wordChoiceCorrectionsScore")]
        public int WordChoiceCorrectionsScore { get; set; }

        [JsonProperty("wordChoiceScoreWeight")]
        public double WordChoiceScoreWeight { get; set; }

        [JsonProperty("overallScore")]
        public int OverallScore { get; set; }
    }

    public class ReadabilityScore
    {
        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("readabilityLevel")]
        public int ReadabilityLevel { get; set; }

        [JsonProperty("readabilityLevelText")]
        public string ReadabilityLevelText { get; set; }

        [JsonProperty("readabilityLevelDescription")]
        public string ReadabilityLevelDescription { get; set; }
    }

    public class DocumentStatistics
    {
        [JsonProperty("sentenceCount")]
        public int SentenceCount { get; set; }

        [JsonProperty("averageSentenceLength")]
        public double AverageSentenceLength { get; set; }

        [JsonProperty("averageWordLength")]
        public double AverageWordLength { get; set; }

        [JsonProperty("readingTimeSeconds")]
        public double ReadingTimeSeconds { get; set; }

        [JsonProperty("speakingTimeSeconds")]
        public double SpeakingTimeSeconds { get; set; }
    }
}
