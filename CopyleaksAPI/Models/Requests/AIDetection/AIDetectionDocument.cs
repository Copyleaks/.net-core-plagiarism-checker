using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests.AIDetection
{
    public class AIDetectionDocument
    {
        /// <summary>Text to check.</summary>
        [JsonProperty("text")]
        [Required]
        public string Text { get; set; }

        /// <summary>Sandbox mode: test your integration for free.</summary>
        [JsonProperty("sandbox")]
        public bool Sandbox { get; set; } = false;
    }
}
