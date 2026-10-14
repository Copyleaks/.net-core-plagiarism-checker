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

using System.Collections.Generic;
using Copyleaks.SDK.V3.API.Models.Responses.Shared;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection
{
    /// <summary>
    /// Response model for Copyleaks AI image detection analysis.
    /// Contains the AI detection results, image information, and scan metadata.
    /// It is also the body of the asynchronous (URL submit) webhook. There, check <see cref="Error"/> first:
    /// when the detection failed, only Error and DeveloperPayload are set.
    /// </summary>
    public class CopyleaksAiImageDetectionResponseModel
    {
        /// <summary>
        /// The version of the AI detection model used for analysis.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// RLE-encoded mask data containing arrays of start positions and lengths for AI-detected regions.
        /// </summary>
        [JsonProperty("result")]
        public CopyleaksAiImageDetectionResultModel Result { get; set; }

        /// <summary>
        /// Summary statistics of the AI detection analysis.
        /// </summary>
        [JsonProperty("summary")]
        public CopyleaksAiImageDetectionSummaryModel Summary { get; set; }

        /// <summary>
        /// Information about the analyzed image.
        /// </summary>
        [JsonProperty("imageInfo")]
        public CopyleaksAiImageDetectionImageInfoModel ImageInfo { get; set; }

        /// <summary>
        /// Metadata about the scan operation.
        /// </summary>
        [JsonProperty("scannedDocument")]
        public CopyleaksAiImageDetectionScannedDocumentModel ScannedDocument { get; set; }

        /// <summary>
        /// Whether the image was determined to be AI-generated.
        /// </summary>
        [JsonProperty("isAiDetected")]
        public bool? IsAiDetected { get; set; }

        /// <summary>
        /// The objects found in the image, present only when the request included ObjectDetection.
        /// Objects that were requested but not found are left out, so look entries up by Object, not by position.
        /// </summary>
        [JsonProperty("detectedObjects")]
        public List<CopyleaksAiImageDetectedObjectModel> DetectedObjects { get; set; }

        /// <summary>
        /// Set in a webhook when the asynchronous detection failed; the result fields are then empty. Check Error first.
        /// </summary>
        [JsonProperty("error")]
        public CopyleaksAsyncDetectionError Error { get; set; }

        /// <summary>
        /// The developer payload from the asynchronous submit request, or null.
        /// </summary>
        [JsonProperty("developerPayload")]
        public string DeveloperPayload { get; set; }
    }

    /// <summary>
    /// An object found in the image, with its segmentation mask.
    /// </summary>
    public class CopyleaksAiImageDetectedObjectModel
    {
        /// <summary>
        /// The object name from the request.
        /// </summary>
        [JsonProperty("object")]
        public string Object { get; set; }

        /// <summary>
        /// The Run-Length Encoded segmentation mask of the object, in the coordinate space of ImageInfo.Shape.
        /// </summary>
        [JsonProperty("mask")]
        public CopyleaksAiImageDetectionMaskModel Mask { get; set; }
    }

    /// <summary>
    /// A Run-Length Encoded mask over the flattened image.
    /// </summary>
    public class CopyleaksAiImageDetectionMaskModel
    {
        [JsonProperty("starts")]
        public int[] Starts { get; set; }

        [JsonProperty("lengths")]
        public int[] Lengths { get; set; }
    }
}

