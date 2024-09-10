/********************************************************************************
 The MIT License(MIT)
 
 Copyright(c) 2016 Copyleaks LTD (https://copyleaks.com)
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
********************************************************************************/

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
