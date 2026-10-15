using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    /// <summary>A match found by Internal AI Source Match in the Copyleaks internal AI database.</summary>
    public class InternalAIData
    {
        /// <summary>Short identifier for the match.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>Title of the matched database entry.</summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>Sample excerpt of the matched content.</summary>
        [JsonProperty("introduction")]
        public string Introduction { get; set; }

        /// <summary>Total number of words matched from the Copyleaks internal AI database.</summary>
        [JsonProperty("matchedWords")]
        public int MatchedWords { get; set; }

        /// <summary>Number of exactly identical words.</summary>
        [JsonProperty("identicalWords")]
        public int IdenticalWords { get; set; }

        /// <summary>Number of similar but not identical words.</summary>
        [JsonProperty("similarWords")]
        public int SimilarWords { get; set; }

        /// <summary>Number of paraphrased words.</summary>
        [JsonProperty("paraphrasedWords")]
        public int ParaphrasedWords { get; set; }

        /// <summary>Total word count of the matched database entry.</summary>
        [JsonProperty("totalWords")]
        public int TotalWords { get; set; }

        /// <summary>Tags object array</summary>
        [JsonProperty("tags")]
        public Tags[] Tags { get; set; }
    }
}
