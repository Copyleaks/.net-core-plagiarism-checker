using System.Collections.Generic;
using Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.ImagePlagiarism
{
    /// <summary>The result of an Image Plagiarism Detection check.</summary>
    public class CopyleaksImagePlagiarismResponseModel
    {
        /// <summary>The developer payload from the request, or null.</summary>
        [JsonProperty("developerPayload")]
        public string DeveloperPayload { get; set; }

        /// <summary>Metadata about the submitted image and the scan.</summary>
        [JsonProperty("scannedImage")]
        public CopyleaksImagePlagiarismScannedImage ScannedImage { get; set; }

        /// <summary>Where copies of the image were found.</summary>
        [JsonProperty("matches")]
        public CopyleaksImagePlagiarismMatches Matches { get; set; }
    }

    public class CopyleaksImagePlagiarismScannedImage : CopyleaksAiImageDetectionScannedDocumentModel
    {
        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }
    }

    public class CopyleaksImagePlagiarismMatches
    {
        /// <summary>Matching images found on the web: full matches first, then partial matches.</summary>
        [JsonProperty("internet")]
        public List<CopyleaksImagePlagiarismInternetMatch> Internet { get; set; }

        [JsonProperty("score")]
        public CopyleaksImagePlagiarismScore Score { get; set; }
    }

    public class CopyleaksImagePlagiarismInternetMatch
    {
        /// <summary>The URL of the matching image.</summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>0 = full (exact or near-exact copy), 1 = partial (cropped, resized or modified).</summary>
        [JsonProperty("matchType")]
        public int MatchType { get; set; }

        /// <summary>The web pages where the image was found. Null when the image was not found on any page.</summary>
        [JsonProperty("webPages")]
        public List<CopyleaksImagePlagiarismWebPage> WebPages { get; set; }
    }

    public class CopyleaksImagePlagiarismWebPage
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class CopyleaksImagePlagiarismScore
    {
        [JsonProperty("totalMatches")]
        public int TotalMatches { get; set; }

        [JsonProperty("fullMatches")]
        public int FullMatches { get; set; }

        [JsonProperty("partialMatches")]
        public int PartialMatches { get; set; }
    }
}
