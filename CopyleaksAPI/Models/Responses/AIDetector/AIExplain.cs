using Newtonsoft.Json;
using System.Collections.Generic;

namespace Copyleaks.SDK.V3.API.Models.Responses.AIDetector
{
    /// <summary>AI Logic explanation of the AI detection result.</summary>
    public class AIExplain
    {
        /// <summary>The phrases (patterns) that contributed to the AI classification.</summary>
        [JsonProperty("patterns")]
        public AIExplainPatterns Patterns { get; set; }
    }

    /// <summary>AI Logic pattern statistics and positions.</summary>
    public class AIExplainPatterns
    {
        /// <summary>Statistics for the detected patterns.</summary>
        [JsonProperty("statistics")]
        public AIExplainStatistics Statistics { get; set; }

        /// <summary>Pattern positions in the text version.</summary>
        [JsonProperty("text")]
        public AIExplainPatternMatch Text { get; set; }

        /// <summary>Pattern positions in the HTML version. HTML sources only.</summary>
        [JsonProperty("html")]
        public AIExplainPatternMatch Html { get; set; }
    }

    /// <summary>AI Logic pattern statistics in parallel arrays: entry i of each array is the same pattern.</summary>
    public class AIExplainStatistics
    {
        /// <summary>For each pattern, how often it appears in AI-generated text.</summary>
        [JsonProperty("aiCount")]
        public List<double> AICount { get; set; }

        /// <summary>For each pattern, how often it appears in human-written text.</summary>
        [JsonProperty("humanCount")]
        public List<double> HumanCount { get; set; }

        /// <summary>For each pattern, the ratio between its AI and human frequency.</summary>
        [JsonProperty("proportion")]
        public List<double> Proportion { get; set; }

        /// <summary>For each pattern, its source: 1 = AI, 2 = Humanizer.</summary>
        [JsonProperty("source")]
        public List<int> Source { get; set; }
    }

    /// <summary>Positions of the AI Logic patterns.</summary>
    public class AIExplainPatternMatch
    {
        /// <summary>Character positions of the patterns. May be null.</summary>
        [JsonProperty("chars")]
        public MatchPositions Chars { get; set; }

        /// <summary>Word positions of the patterns.</summary>
        [JsonProperty("words")]
        public MatchPositions Words { get; set; }
    }
}
