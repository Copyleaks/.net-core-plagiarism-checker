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

namespace Copyleaks.SDK.V3.API.Models.Requests.AIDetection
{
    public class NaturalLanguageDocument : AIDetectionDocument
    {
        /// <summary>
        /// The language code of your content. The selected language should be on the Supported Languages list above. 
        /// If the 'language' field is not supplied , our system will automatically detect the language of the content
        /// </summary>
        [JsonProperty("language")]
        public string Language { get; set; }

        /// <summary>
        /// Enable AI Logic, which explains the result (returned in AIDetectionResult.Explain).
        /// Explanations are returned for a limited set of languages only.
        /// Leave it unset (null) to omit it from the request and get the server default (false).
        /// </summary>
        [JsonProperty("explain", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Explain { get; set; }

        /// <summary>
        /// Controls the behavior of the AI detection, from 1 to 3:
        /// 1 detects text copied from an LLM without edits, 2 also detects LLM text with minor changes,
        /// and 3 also detects LLM text that was heavily modified.
        /// Leave it unset (null) to omit it from the request and get the server default (2).
        /// </summary>
        [JsonProperty("sensitivity", NullValueHandling = NullValueHandling.Ignore)]
        public int? Sensitivity { get; set; }
    }
}
