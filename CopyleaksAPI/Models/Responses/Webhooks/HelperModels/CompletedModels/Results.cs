using System;
using System.Collections.Generic;
using System.Text;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.CompletedModels
{
    public class Results
    {
        [JsonProperty("database")]
        public Database[] Database { get; set; }
        [JsonProperty("batch")]
        public Batch[] Batch { get; set; }
        [JsonProperty("repositories")]
        public Repositories[] Repositories { get; set; }
        [JsonProperty("score")]
        public Score Score { get; set; }
        [JsonProperty("internet")]
        public Internet[] Internet { get; set; }
    }
}
