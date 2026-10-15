using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AIDetection
{
    public class NaturalLanguageDocument : AIDetectionDocument
    {
        /// <summary>Language code of the content; must be a supported language. Null = auto-detect.</summary>
        [JsonProperty("language")]
        public string Language { get; set; }

        /// <summary>
        /// AI Logic: explain the result (in AIDetectionResult.Explain), for a limited set of languages only.
        /// Null = server default (false).
        /// </summary>
        [JsonProperty("explain", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Explain { get; set; }

        /// <summary>
        /// AI detection sensitivity, 1 to 3: 1 = unedited LLM text, 2 = also minor edits, 3 = also heavy edits.
        /// Null = server default (2).
        /// </summary>
        [JsonProperty("sensitivity", NullValueHandling = NullValueHandling.Ignore)]
        public int? Sensitivity { get; set; }
    }
}
