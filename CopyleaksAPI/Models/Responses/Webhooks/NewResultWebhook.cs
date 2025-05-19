using System;
using System.Collections.Generic;
using System.Text;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NewResultsModels;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks
{
    public class NewResultWebhook
    {
        [JsonProperty("score")]
        public NewResultScore Score { get; set; }
        [JsonProperty("internet")]
        public NewResultInternet[] Internet { get; set; }
        [JsonProperty("database")]
        public SharedResultsModel[] Database { get; set; }
        [JsonProperty("batch")]
        public SharedResultsModel[] Batch { get; set; }
        [JsonProperty("repositories")]
        public NewResultsRepositories[] Repositories { get; set; }
    }
}
