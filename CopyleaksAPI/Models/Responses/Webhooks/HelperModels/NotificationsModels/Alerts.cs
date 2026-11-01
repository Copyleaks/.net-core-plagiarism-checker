using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Responses.AIDetector;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NotificationsModels
{
    public class Alerts
    {
        /// <summary>Scan alert category.</summary>
        [JsonProperty("category")]
        public int Category { get; set; }

        /// <summary>Scan alert code. The code is unique for each scan alert.</summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>Scan alert human-readable title.</summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>Provides human-readable information about the scan alert.</summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>Url to a resource describing the specific scan alert.</summary>
        [JsonProperty("helpLink")]
        public string HelpLink { get; set; }

        /// <summary>Specifies the importance of the scan alert.</summary>
        [JsonProperty("severity")]
        public int Severity { get; set; }

        /// <summary>Additional data about the scan alert. Supplied as a json string.</summary>
        [JsonProperty("additionalData")]
        public string AdditionalData { get; set; }

        /// <summary>
        /// Decodes the additionalData of a "suspected-ai-text" alert (trailing NUL and ASCII whitespace are ignored).
        /// Null for other alerts, no data (null, empty, or only NUL/whitespace), or valid JSON that is not an object.
        /// </summary>
        /// <exception cref="JsonException">AdditionalData is not valid JSON.</exception>
        public AIDetectionResult GetAIDetectionResult()
        {
            return AIDetectionAlertParser.Parse(Code, AdditionalData);
        }
    }
}