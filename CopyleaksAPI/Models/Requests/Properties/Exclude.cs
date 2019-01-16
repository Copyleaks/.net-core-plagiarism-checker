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

using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// Exclude properties from scan
    /// </summary>
    public class Exclude
    {
        /// <summary>
        /// Exclude references from the scan.
        /// </summary>
        [JsonProperty("references")]
        public bool References { get; set; } = false;

        /// <summary>
        /// Exclude quotes from the scan.
        /// </summary>
        [JsonProperty("quotes")]
        public bool Quotes { get; set; } = false;

        /// <summary>
        /// Exclude titles from the scan.
        /// </summary>
        [JsonProperty("titles")]
        public bool Titles { get; set; } = false;

        /// <summary>
        /// When HTML document, exclude the HTML tags and attribute metadata from the scan. 
        /// </summary>
        [JsonProperty("htmlTemplate")]
        public bool HtmlTemplate { get; set; } = false;

    }
}
