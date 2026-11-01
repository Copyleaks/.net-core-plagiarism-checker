using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses
{

    /// <summary>Supported file types. See https://api.copyleaks.com/documentation/specifications</summary>
    public class SupportedTypesResult
    {
        /// <summary>List of textual file types supported.</summary>
        [JsonProperty("textual")]
        public string[] Textual { get; set; }

        /// <summary>List of file types supported using OCR</summary>
        [JsonProperty("ocr")]
        public string[] Ocr { get; set; }

    }
}
