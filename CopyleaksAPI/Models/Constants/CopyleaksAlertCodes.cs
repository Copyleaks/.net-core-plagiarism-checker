namespace Copyleaks.SDK.V3.API.Models.Constants
{
    /// <summary>Scan alert codes of the completed webhook (notifications.alerts[].code); case-sensitive.</summary>
    public static class CopyleaksAlertCodes
    {
        /// <summary>
        /// AI-generated text detected (category 2, severity 4); additionalData holds the result as a JSON string.
        /// </summary>
        public const string SUSPECTED_AI_TEXT = "suspected-ai-text";
    }
}
