using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Responses.WritingAssistant
{
    public class ScannedDocument
    {
        [JsonProperty("scanId")]
        public string ScanId { get; set; }

        [JsonProperty("totalWords")]
        public int TotalWords { get; set; }

        [JsonProperty("totalExcluded")]
        public int TotalExcluded { get; set; }

        [JsonProperty("actualCredits")]
        public int ActualCredits { get; set; }

        [JsonProperty("expectedCredits")]
        public int ExpectedCredits { get; set; }

        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }
    }
}
