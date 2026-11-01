using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
    /// <summary>A file to submit for scanning.</summary>
    public class FileDocument: Document
    {
        /// <summary>Base64 file content.</summary>
        [JsonProperty("base64")]
        public string Base64 { get; set; }

        /// <summary>Original file name, e.g. "file.pdf".</summary>
        [JsonProperty("filename")]
		public string Filename { get; set; }
    }
}
