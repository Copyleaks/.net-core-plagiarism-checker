﻿/********************************************************************************
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

using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    public class SharedResultsModel
    {
        /// <summary>
        /// Unique result ID to identify this result.
        /// </summary>
        [JsonProperty("id")]
        public string Id {  get; set; }

        /// <summary>
        /// Document title. Mostly extracted from the document content.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Document brief introduction. Mostly extracted from the document content.
        /// </summary>
        [JsonProperty("introduction")]
        public string Introduction { get; set; }

        /// <summary>
        /// Total matched words between this result and the scanned document.
        /// </summary>
        [JsonProperty("matchedWords")]
        public int MatchedWords {  get; set; }

        /// <summary>
        /// In case a result was found in the Copyleaks internal database, and was submitted by you, this will show the scan id of the specific result. Otherwise, this field will remain empty.
        /// </summary>
        [JsonProperty("scanId")]
        public string ScanId { get; set; }

        /// <summary>
        /// Metdata object
        /// </summary>
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

    }
}
