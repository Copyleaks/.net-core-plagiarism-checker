using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    public class AIGeneratedText
    {
        /// <summary>Detect whether the text was written by an AI.</summary>
        [JsonProperty("detect")]
        public bool Detect { get; set; } = false;

        /// <summary>
        /// AI detection sensitivity, 1 to 3: 1 = unedited LLM text, 2 = also minor edits, 3 = also heavy edits.
        /// Null = server default (2).
        /// </summary>
        [JsonProperty("sensitivity", NullValueHandling = NullValueHandling.Ignore)]
        public int? Sensitivity { get; set; }

        /// <summary>AI Logic: explain the AI detection result. Null = server default (disabled).</summary>
        [JsonProperty("explain", NullValueHandling = NullValueHandling.Ignore)]
        public AIExplainSettings Explain { get; set; }
    }
}
