using System.Collections.Generic;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ErrorModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection
{
    /// <summary>
    /// AI image detection result; also the async (URL submit) webhook body: check <see cref="Error"/> first there.
    /// </summary>
    public class CopyleaksAiImageDetectionResponseModel
    {
        /// <summary>AI detection model version.</summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>RLE mask (start positions and lengths) of the AI-detected regions.</summary>
        [JsonProperty("result")]
        public CopyleaksAiImageDetectionResultModel Result { get; set; }

        /// <summary>Summary statistics of the AI detection analysis.</summary>
        [JsonProperty("summary")]
        public CopyleaksAiImageDetectionSummaryModel Summary { get; set; }

        /// <summary>Information about the analyzed image.</summary>
        [JsonProperty("imageInfo")]
        public CopyleaksAiImageDetectionImageInfoModel ImageInfo { get; set; }

        /// <summary>Metadata about the scan operation.</summary>
        [JsonProperty("scannedDocument")]
        public CopyleaksAiImageDetectionScannedDocumentModel ScannedDocument { get; set; }

        /// <summary>Whether the image was determined to be AI-generated.</summary>
        [JsonProperty("isAiDetected")]
        public bool? IsAiDetected { get; set; }

        /// <summary>
        /// Objects found in the image; only when the request included ObjectDetection.
        /// Requested objects that were not found are left out, so look entries up by Object, not by position.
        /// </summary>
        [JsonProperty("detectedObjects")]
        public List<CopyleaksAiImageDetectedObjectModel> DetectedObjects { get; set; }

        /// <summary>Set when the asynchronous detection failed; then only Error and DeveloperPayload are set.</summary>
        [JsonProperty("error")]
        public Error Error { get; set; }

        /// <summary>The developer payload from the asynchronous submit request, or null.</summary>
        [JsonProperty("developerPayload")]
        public string DeveloperPayload { get; set; }
    }

    /// <summary>An object found in the image, with its segmentation mask.</summary>
    public class CopyleaksAiImageDetectedObjectModel
    {
        /// <summary>The object name from the request.</summary>
        [JsonProperty("object")]
        public string Object { get; set; }

        /// <summary>RLE segmentation mask of the object, in ImageInfo.Shape coordinates.</summary>
        [JsonProperty("mask")]
        public CopyleaksAiImageDetectionMaskModel Mask { get; set; }
    }

    /// <summary>A Run-Length Encoded mask over the flattened image.</summary>
    public class CopyleaksAiImageDetectionMaskModel
    {
        [JsonProperty("starts")]
        public int[] Starts { get; set; }

        [JsonProperty("lengths")]
        public int[] Lengths { get; set; }
    }
}

