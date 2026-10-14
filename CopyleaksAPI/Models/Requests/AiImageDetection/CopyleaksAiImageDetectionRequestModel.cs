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
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection
{
    /// <summary>
    /// Request model for Copyleaks AI image detection.
    /// With <c>SubmitAsync</c> the request body is a JSON object containing the base64 image.
    /// With <c>SubmitFileAsync</c> the image is uploaded as a file (multipart/form-data) and <see cref="Base64"/> is not used.
    /// </summary>
    public class CopyleaksAiImageDetectionRequestModel
    {
        /// <summary>
        /// A request for <c>SubmitFileAsync</c>, which uploads the image as a file instead of base64.
        /// </summary>
        public CopyleaksAiImageDetectionRequestModel(string fileName, string model, bool sandbox = false)
            : this(null, fileName, model, sandbox)
        {
        }

        public CopyleaksAiImageDetectionRequestModel(string base64, string fileName, string model, bool sandbox = false)
        {
            Base64 = base64;
            FileName = fileName;
            Model = model;
            Sandbox = sandbox; // Assign directly if passed, otherwise the default `false` will be used
        }
        /// <summary>
        /// The base64-encoded image data to be analyzed for AI generation.
        /// </summary>
        /// <remarks>
        /// Requirements:
        /// - Minimum 512×512px, maximum 16 megapixels, less than 32MB
        /// - Supported formats: PNG, JPEG, BMP, WebP, HEIC/HEIF
        /// </remarks>
        /// <example>"aGVsbG8gd29ybGQ="</example>
        [JsonProperty("base64", NullValueHandling = NullValueHandling.Ignore)]
        public string Base64 { get; set; }

        /// <summary>
        /// The name of the image file including its extension.
        /// </summary>
        /// <remarks>
        /// Requirements:
        /// - Supported extensions: .png, .bmp, .jpg, .jpeg, .webp, .heic, .heif
        /// - Maximum 255 characters
        /// </remarks>
        /// <example>"my-image.png"</example>
        [Required]
        [JsonProperty("filename")]
        public string FileName { get; set; }

        /// <summary>
        /// The AI detection model to use for analysis.
        /// You can use either the full model name or its alias.
        /// </summary>
        /// <remarks>
        /// Available models:
        /// - AI Image 1 Ultra: "ai-image-1-ultra-01-09-2025" (full name) or "ai-image-1-ultra" (alias)
        ///   AI image detection model. Produces an overlay of the detected AI segments.
        /// </remarks>
        /// <example>"ai-image-1-ultra-01-09-2025" or "ai-image-1-ultra"</example>
        [Required]
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// Use sandbox mode to test your integration with the Copyleaks API without consuming any credits.
        /// </summary>
        /// <remarks>
        /// Submit images for AI detection and get returned mock results, simulating Copyleaks' API functionality 
        /// to ensure you have successfully integrated the API.
        /// This feature is intended to be used for development purposes only.
        /// Default value is false.
        /// </remarks>
        /// <example>false</example>
        [JsonProperty("sandbox")]
        public bool Sandbox { get; set; } = false;

        /// <summary>
        /// The type of detection mask to return: "binary" (the default), "heatmap" or "none".
        /// See <see cref="Constants.CopyleaksAiImageMaskTypes"/>. "heatmap" adds Result.Labels to the response,
        /// and "none" omits Result.
        /// Leave it unset (null) to omit it from the request and get the server default ("binary").
        /// </summary>
        [JsonProperty("maskType", NullValueHandling = NullValueHandling.Ignore)]
        public string MaskType { get; set; }

        /// <summary>
        /// Objects to locate and segment in the image, for example
        /// <c>new List&lt;CopyleaksAiImageObjectDetectionItem&gt; { new CopyleaksAiImageObjectDetectionItem("face") }</c>.
        /// Up to 3 entries, each a non-empty name of at most 32 characters. The response then has DetectedObjects.
        /// Leave it unset (null) to omit it from the request. Do not send an empty list: the server rejects it.
        /// </summary>
        [JsonProperty("objectDetection", NullValueHandling = NullValueHandling.Ignore)]
        public List<CopyleaksAiImageObjectDetectionItem> ObjectDetection { get; set; }
    }
}
