using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests.WritingAssistant
{
    public class WritingAssistantDocument
    {
        /// <summary>Text to check, 1 to 25000 characters.</summary>
        [Required]
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>Sandbox mode: test your integration without consuming credits.</summary>
        [JsonProperty("sandbox")]
        public bool Sandbox { get; set; } = false;

        /// <summary>Language code of the content; must be a supported language. Null = auto-detect.</summary>
        [JsonProperty("language", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(null)]
        public string Language { get; set; }

        [JsonProperty("score", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(null)]
        public Score Score { get; set; }
    }
}
