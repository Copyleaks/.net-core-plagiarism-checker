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


using Newtonsoft.Json;
using System.Collections.Generic;

namespace Copyleaks.SDK.V3.API.Models.Responses.AIDetector
{
    /// <summary>
    /// AI text detection result.
    /// Returned by the writer-detector check, and decoded from the additionalData of the
    /// "suspected-ai-text" alert of the completed webhook.
    /// </summary>
    public class AIDetectionResult
    {
        [JsonProperty("scanType")]
        public string ScanType { get; set; }

        /// <summary>
        /// The version of the AI Content Detection used.
        /// </summary>
        [JsonProperty("modelVersion")]
        public string ModelVersion { get; set; }

        /// <summary>
        /// An array of all the classifications. The sections contain their position, classification and the probability for that classification.
        /// </summary>
        [JsonProperty("results")]
        public List<ClassificationResult> Results { get; set; }

        /// <summary>
        /// Aggregated detection results.
        /// </summary>
        [JsonProperty("summary")]
        public Summary Summary { get; set; }

        /// <summary>
        /// General information about the scanned document.
        /// </summary>
        [JsonProperty("scannedDocument")]
        public ScannedDocument ScannedDocument { get; set; }

        /// <summary>
        /// The translation provider, when the text was machine-translated to English before detection.
        /// 0 means the text was not translated.
        /// Sent in the completed webhook AI alert data; null when absent.
        /// </summary>
        [JsonProperty("translationProvider")]
        public int? TranslationProvider { get; set; }

        /// <summary>
        /// The full English translation that was scanned, when the text was machine-translated.
        /// Sent in the completed webhook AI alert data; null when absent.
        /// </summary>
        [JsonProperty("translation")]
        public string Translation { get; set; }

        /// <summary>
        /// AI Logic explanation of the result.
        /// Present only when AI Logic (explain) was enabled; null otherwise.
        /// </summary>
        [JsonProperty("explain")]
        public AIExplain Explain { get; set; }
    }
}
