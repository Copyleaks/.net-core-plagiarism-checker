using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.TextModeration
{
    public class CopyleaksTextModerationRequestModel
    {
        /// <summary>Maximum labels per request.</summary>
        public const int MaxLabels = 16;

        /// <summary>Text to moderate.</summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>Sandbox mode: test your integration without consuming credits.</summary>
        [JsonProperty("sandbox")]
        public bool Sandbox { get; set; } = false;

        /// <summary>Language code of the content; must be a supported language. Null = auto-detect.</summary>
        [JsonProperty("language")]
        public string Language { get; set; }

        /// <summary>
        /// Labels to moderate for (1 to 16),
        /// e.g. <c>new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.TOXIC_V1)</c>.
        /// </summary>
        [JsonProperty("labels")]
        [MinLength(1, ErrorMessage = "Labels array must have at least 1 element.")]
        [MaxLength(MaxLabels, ErrorMessage = "Labels array must have at most 16 elements.")]
        public object[] Labels { get; set; }

        public CopyleaksTextModerationRequestModel(string text, bool sandbox, string language, object[] labels)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Sandbox = sandbox;
            Language = language;
            if (labels == null || !labels.Any())
                throw new ArgumentException("Labels array must have at least 1 element.", nameof(labels));
            if (labels.Length > MaxLabels)
                throw new ArgumentException($"Labels array must have at most {MaxLabels} elements.", nameof(labels));
            Labels = labels;
        }
    }
}
