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
using System.Collections.Generic;

namespace Copyleaks.SDK.V3.API.Models.Responses.AIDetector
{
    /// <summary>
    /// AI Logic explanation of the AI detection result.
    /// Present only when AI Logic (explain) was enabled for the scan.
    /// </summary>
    public class AIExplain
    {
        /// <summary>
        /// The phrases (patterns) that contributed to the AI classification.
        /// </summary>
        [JsonProperty("patterns")]
        public AIExplainPatterns Patterns { get; set; }
    }

    /// <summary>
    /// The AI Logic patterns: their statistics and their positions in the document.
    /// </summary>
    public class AIExplainPatterns
    {
        /// <summary>
        /// Statistics for the detected patterns.
        /// </summary>
        [JsonProperty("statistics")]
        public AIExplainStatistics Statistics { get; set; }

        /// <summary>
        /// The pattern positions in the text version of the document.
        /// </summary>
        [JsonProperty("text")]
        public AIExplainPatternMatch Text { get; set; }

        /// <summary>
        /// The pattern positions in the HTML version of the document.
        /// Present only for HTML sources.
        /// </summary>
        [JsonProperty("html")]
        public AIExplainPatternMatch Html { get; set; }
    }

    /// <summary>
    /// Statistics for the AI Logic patterns.
    /// </summary>
    public class AIExplainStatistics
    {
        /// <summary>
        /// For each pattern, how often it appears in AI-generated text.
        /// The statistics arrays are parallel: entry i of each array belongs to the same pattern.
        /// </summary>
        [JsonProperty("aiCount")]
        public List<double> AICount { get; set; }

        /// <summary>
        /// For each pattern, how often it appears in human-written text.
        /// </summary>
        [JsonProperty("humanCount")]
        public List<double> HumanCount { get; set; }

        /// <summary>
        /// For each pattern, the ratio between its AI and human frequency.
        /// </summary>
        [JsonProperty("proportion")]
        public List<double> Proportion { get; set; }

        /// <summary>
        /// For each pattern, its source: 1 = AI, 2 = Humanizer.
        /// </summary>
        [JsonProperty("source")]
        public List<int> Source { get; set; }
    }

    /// <summary>
    /// Positions of the AI Logic patterns.
    /// </summary>
    public class AIExplainPatternMatch
    {
        /// <summary>
        /// Character positions of the patterns. May be null.
        /// </summary>
        [JsonProperty("chars")]
        public MatchPositions Chars { get; set; }

        /// <summary>
        /// Word positions of the patterns.
        /// </summary>
        [JsonProperty("words")]
        public MatchPositions Words { get; set; }
    }
}
