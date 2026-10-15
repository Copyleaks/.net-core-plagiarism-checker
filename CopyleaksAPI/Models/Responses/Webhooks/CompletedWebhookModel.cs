using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Responses.CitationChecker;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.CompletedModels;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NotificationsModels;
using Newtonsoft.Json;
using System;
using System.Linq;
using AIDetectionResult = Copyleaks.SDK.V3.API.Models.Responses.AIDetector.AIDetectionResult;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks
{
    public class CompletedWebhookModel : StatusWebhook
    {
        [JsonProperty("results")]
        public Results Results { get; set; }
        [JsonProperty("notifications")]
        public Notifications Notifications { get; set; }
        [JsonProperty("scannedDocument")]
        public ScannedDocument ScannedDocument { get; set; }

        /// <summary>
        /// Citation Checker summary, present when the scan was submitted with References.Validate = true.
        /// Per-reference results are in the crawled version export.
        /// </summary>
        [JsonProperty("referencesValidation")]
        public ReferencesValidation ReferencesValidation { get; set; }

        /// <summary>The first <see cref="CopyleaksAlertCodes.SUSPECTED_AI_TEXT"/> alert, or null.</summary>
        public Alerts GetAIDetectionAlert()
        {
            return Notifications?.Alerts?.FirstOrDefault(alert =>
                alert != null && string.Equals(alert.Code, CopyleaksAlertCodes.SUSPECTED_AI_TEXT, StringComparison.Ordinal));
        }

        /// <summary>
        /// Same as GetAIDetectionAlert()?.GetAIDetectionResult().
        /// Null when there is no AI alert, the alert has no data, or the data is valid JSON that is not an object.
        /// </summary>
        /// <exception cref="JsonException">The alert's additionalData is not valid JSON.</exception>
        public AIDetectionResult GetAIDetectionResult()
        {
            return GetAIDetectionAlert()?.GetAIDetectionResult();
        }
    }
}
