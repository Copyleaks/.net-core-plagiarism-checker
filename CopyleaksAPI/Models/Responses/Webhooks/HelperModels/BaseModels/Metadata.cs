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

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels
{
    public class Metadata
    {
        /// <summary>
        /// The submitted url after all http redirects.
        /// </summary>
        [JsonProperty("finalUrl")]
        public string FinalUrl { get; set; }

        /// <summary>
        /// Extracted canonical url from the scanned document.
        /// </summary>
        [JsonProperty("canonicalUrl")]
        public string CanonicalUrl { get; set; }

        /// <summary>
        /// Publication date of the scanned document.
        /// </summary>
        [JsonProperty("publishDate")]
        public string PublishDate { get; set; }

        /// <summary>
        /// Creation date of the scanned document.
        /// </summary>
        [JsonProperty("creationDate")]
        public string CreationDate { get; set; }

        /// <summary>
        /// Last modification date of the scanned document.
        /// </summary>
        [JsonProperty("lastModificationDate")]
        public string LastModificationDate { get; set; }

        /// <summary>
        /// Scanned document author.
        /// </summary>
        [JsonProperty("author")]
        public string Author { get; set; }

        /// <summary>
        /// Scanned document organization.
        /// </summary>
        [JsonProperty("organization")]
        public string Organization { get; set; }

        /// <summary>
        /// Scanned document filename.
        /// </summary>
        [JsonProperty("filename")]
        public string Filename { get; set; }
    }
}
