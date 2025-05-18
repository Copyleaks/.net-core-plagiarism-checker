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
        public List<Database> Database { get; set; }
        [JsonProperty("batch")]
        public List<Batch> Batch { get; set; }
        [JsonProperty("repositories")]
        public List<Repositories> Repositories { get; set; }
        [JsonProperty("score")]
        public Score Score { get; set; }
        [JsonProperty("internet")]
        public List<Internet> Internet { get; set; }
    }
}
