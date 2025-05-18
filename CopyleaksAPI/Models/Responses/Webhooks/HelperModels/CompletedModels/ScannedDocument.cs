using System;
using System.Collections.Generic;
using System.Text;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.CompletedModels
{
    public class ScannedDocument
    {
        [JsonProperty("scanId")]
        public string ScanId;
        [JsonProperty("totalWords")]
        public int TotalWords;
        [JsonProperty("totalExcluded")]
        public int TotalExcluded;
        [JsonProperty("credits")]
        public int Credits;
        [JsonProperty("creationTime")]
        public string CreationTime;
        [JsonProperty("metadata")]
        public Metadata Metadata;
    }
}
