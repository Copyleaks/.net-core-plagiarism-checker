using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>PDF report colors.</summary>
    public class ReportCustomizationColors
    {
        /// <summary>AI detection highlight color, in HEX. Null = server default.</summary>
        [JsonProperty("aiDetection", NullValueHandling = NullValueHandling.Ignore)]
        public string AIDetection { get; set; }

        /// <summary>Grammar Checker correction highlight color, in HEX. Null = server default.</summary>
        [JsonProperty("writingFeedback", NullValueHandling = NullValueHandling.Ignore)]
        public string WritingFeedback { get; set; }

        /// <summary>Color of the main strip in the header.</summary>
        public string MainStrip { get; set; }

        /// <summary>Title color.</summary>
        public string titles { get; set; }

        /// <summary>Highlight color for identical matches.</summary>
        public string Identical { get; set; }

        /// <summary>Highlight color for minor-changes matches.</summary>
        public string MinorChanges { get; set; }

        /// <summary>Highlight color for related-meaning matches.</summary>
        public string RelatedMeaning { get; set; }
    }
}
