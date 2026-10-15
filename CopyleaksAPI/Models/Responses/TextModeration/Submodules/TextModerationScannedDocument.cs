using System;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.TextModeration.Submodules
{
    public class TextModerationScannedDocument
    {
        /// <summary>The scan id given by the user.</summary>
        [JsonProperty("scanId")]
        public string ScanId { get; set; }

        /// <summary>Total number of words found in the scanned text.</summary>
        [JsonProperty("totalWords")]
        public int TotalWords { get; set; }

        /// <summary>Total excluded words from the text.</summary>
        [JsonProperty("totalExcluded")]
        public int TotalExcluded { get; set; }

        /// <summary>The cost of credits for this scan.</summary>
        [JsonProperty("actualCredits")]
        public int ActualCredits { get; set; }

        /// <summary>The amount of credits that was expected to be spent on the scan.</summary>
        [JsonProperty("expectedCredits")]
        public int ExpectedCredits { get; set; }

        /// <summary>Creation time of the scan.</summary>
        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }
    }
}
