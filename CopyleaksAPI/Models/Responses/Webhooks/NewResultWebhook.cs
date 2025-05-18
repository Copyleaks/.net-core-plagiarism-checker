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
        public List<NewResultInternet> Internet { get; set; }
        [JsonProperty("database")]
        public List<SharedResultsModel> Database { get; set; }
        [JsonProperty("batch")]
        public List<SharedResultsModel> Batch { get; set; }
        [JsonProperty("repositories")]
        public List<NewResultsRepositories> Repositories { get; set; }
    }
}
