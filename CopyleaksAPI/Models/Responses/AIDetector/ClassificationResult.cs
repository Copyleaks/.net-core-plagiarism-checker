using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Copyleaks.SDK.V3.API.Models.Responses.AIDetector
{
    public class ClassificationResult
    {
        /// <summary>The classification of the section: 1 = human, 2 = AI. Reads 0 when the server omits it.</summary>
        [JsonProperty("classification")]
        public int Classification { get; set; }

        /// <summary>Classification probability. Deprecated by the API; reads 0 when the server omits it.</summary>
        [Obsolete("probability is deprecated by the API and will be removed. Use Classification and the summary instead.")]
        [JsonProperty("probability")]
        public double Probability { get; set; }

        [JsonProperty("matches")]
        public List<Match> Matches { get; set; }
    }

    public class Match
    {
        [JsonProperty("text")]
        public MatchText Text { get; set; }

        /// <summary>Section positions in the HTML version. HTML sources only; null otherwise.</summary>
        [JsonProperty("html")]
        public MatchText Html { get; set; }
    }

    public class MatchText
    {
        [JsonProperty("chars")]
        public MatchPositions Chars { get; set; }

        [JsonProperty("words")]
        public MatchPositions Words { get; set; }
    }

    public class MatchPositions
    {
        [JsonProperty("starts")]
        public List<int> Starts { get; set; }

        [JsonProperty("lengths")]
        public List<int> Lengths { get; set; }

        /// <summary>The HTML group of each position. Present only for HTML positions; null otherwise.</summary>
        [JsonProperty("groupIds")]
        public List<int> GroupIds { get; set; }
    }
}
