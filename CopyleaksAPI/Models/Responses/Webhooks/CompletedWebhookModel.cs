/********************************************************************************
 The MIT License(MIT)
 
 Copyright(c) 2016 Copyleaks LTD (https://copyleaks.com)
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
********************************************************************************/

using Copyleaks.SDK.V3.API.Models.Constants;
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
        /// Get the first <see cref="CopyleaksAlertCodes.SUSPECTED_AI_TEXT"/> alert of the scan.
        /// Returns null when the completed webhook contains no suspected-ai-text alert.
        /// </summary>
        /// <returns>The AI alert, or null</returns>
        public Alerts GetAIDetectionAlert()
        {
            return Notifications?.Alerts?.FirstOrDefault(alert =>
                alert != null && string.Equals(alert.Code, CopyleaksAlertCodes.SUSPECTED_AI_TEXT, StringComparison.Ordinal));
        }

        /// <summary>
        /// Get the AI detection result decoded from the <see cref="CopyleaksAlertCodes.SUSPECTED_AI_TEXT"/> alert.
        /// Same as GetAIDetectionAlert()?.GetAIDetectionResult().
        /// Returns null when there is no AI alert (see <see cref="GetAIDetectionAlert"/>), the alert carries no data,
        /// or the data is valid JSON that is not a JSON object.
        /// </summary>
        /// <returns>The AI detection result, or null</returns>
        /// <exception cref="JsonException">The alert's additionalData is not valid JSON (JsonReaderException or JsonSerializationException)</exception>
        public AIDetectionResult GetAIDetectionResult()
        {
            return GetAIDetectionAlert()?.GetAIDetectionResult();
        }
    }
}
