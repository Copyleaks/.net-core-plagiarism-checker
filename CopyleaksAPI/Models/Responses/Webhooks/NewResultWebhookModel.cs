using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NewResultsModels;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks
{
    /// <summary>New result webhook, sent while the scan is still running.</summary>
    public class NewResultWebhookModel : Webhook
    {
        /// <summary>Matches found by Internal AI Source Match.</summary>
        [JsonProperty("internalAIData")]
        public InternalAIData[] InternalAIData { get; set; }

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
