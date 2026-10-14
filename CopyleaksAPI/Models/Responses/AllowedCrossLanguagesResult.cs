using System.Collections.Generic;
using System.Linq;
using Copyleaks.SDK.V3.API.Helpers;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
    /// <summary>
    /// The languages supported by cross-language plagiarism detection, as ISO-639-1 codes.
    /// </summary>
    public class AllowedCrossLanguagesResult
    {
        /// <summary>
        /// Languages that a submitted document can be in, keyed by language name (for example "english" to "en").
        /// </summary>
        [JsonProperty("documentLanguages")]
        [JsonConverter(typeof(LanguageMapConverter))]
        public Dictionary<string, string> DocumentLanguages { get; set; }

        /// <summary>
        /// Languages that results can be found in (the values for CrossLanguages.Languages), keyed by language name.
        /// </summary>
        [JsonProperty("resultLanguages")]
        [JsonConverter(typeof(LanguageMapConverter))]
        public Dictionary<string, string> ResultLanguages { get; set; }

        /// <summary>
        /// The ISO-639-1 codes of <see cref="DocumentLanguages"/>.
        /// </summary>
        [JsonIgnore]
        public string[] DocumentLanguageCodes => DocumentLanguages?.Values.ToArray() ?? new string[0];

        /// <summary>
        /// The ISO-639-1 codes of <see cref="ResultLanguages"/>.
        /// </summary>
        [JsonIgnore]
        public string[] ResultLanguageCodes => ResultLanguages?.Values.ToArray() ?? new string[0];
    }
}
