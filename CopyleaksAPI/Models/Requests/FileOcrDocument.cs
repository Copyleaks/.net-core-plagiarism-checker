using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
    /// <summary>An image file to OCR and scan.</summary>
    public class FileOcrDocument :FileDocument
    {
        /// <summary>Language code of the text in the image.</summary>
        [JsonProperty("langCode")]
		public string Language { get; set; }
    }
}
