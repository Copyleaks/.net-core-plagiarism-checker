using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels
{
    public class Metadata
    {
        /// <summary>The submitted url after all http redirects.</summary>
        [JsonProperty("finalUrl")]
        public string FinalUrl { get; set; }

        /// <summary>Extracted canonical url from the scanned document.</summary>
        [JsonProperty("canonicalUrl")]
        public string CanonicalUrl { get; set; }

        /// <summary>Publication date of the scanned document.</summary>
        [JsonProperty("publishDate")]
        public string PublishDate { get; set; }

        /// <summary>Creation date of the scanned document.</summary>
        [JsonProperty("creationDate")]
        public string CreationDate { get; set; }

        /// <summary>Last modification date of the scanned document.</summary>
        [JsonProperty("lastModificationDate")]
        public string LastModificationDate { get; set; }

        /// <summary>Scanned document author.</summary>
        [JsonProperty("author")]
        public string Author { get; set; }

        /// <summary>Scanned document organization.</summary>
        [JsonProperty("organization")]
        public string Organization { get; set; }

        /// <summary>Scanned document filename.</summary>
        [JsonProperty("filename")]
        public string Filename { get; set; }
    }
}
