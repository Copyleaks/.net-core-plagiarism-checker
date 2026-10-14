using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
    /// <summary>
    /// The file types supported by AI text detection in scans.
    /// </summary>
    public class SupportedAiTextFileTypesResult
    {
        /// <summary>
        /// File extensions without the dot, for example "pdf" and "docx".
        /// </summary>
        [JsonProperty("supportedAiFileTypes")]
        public string[] SupportedAiFileTypes { get; set; }
    }
}
