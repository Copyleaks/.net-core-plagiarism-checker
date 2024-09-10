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

using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// Customizeable filters
    /// </summary>
    public class Filters
    {
        /// <summary>
        /// Enable/disable identical matches
        /// </summary>
        [JsonProperty("identicalEnabled")]
        public bool IdenticalEnabled { get; set; } = true;

        /// <summary>
        /// Enable/disable minor changes matches
        /// </summary>
        [JsonProperty("minorChangesEnabled")]
        public bool MinorChangesEnabled { get; set; } = true;

        /// <summary>
        /// Enable/disable related meaning matches
        /// </summary>
        [JsonProperty("relatedMeaningEnabled")]
        public bool RelatedMeaningEnabled { get; set; } = true;

        /// <summary>
        /// Set the minimum copied words to be considered as plagiarized content 
        /// </summary>
        [JsonProperty("minCopiedWords")]
        public ushort? minCopiedWords { get; set; } = null;

        /// <summary>
        /// When set to true the result will not include explicit results.
        /// Safe search is not 100% accurate but it can help avoid explicit and inappropriate search results
        /// </summary>
        [JsonProperty("safeSearch")]
        public bool SafeSearch { get; set; } = false;

        /// <summary>
        /// A list of domain to include or exclude from sources
        /// </summary>
        [JsonProperty("domains")]
        public string[] Domains { get; set; } = new string[] { };

        /// <summary>
        /// Treat `domains` as inclide or exclude list
        /// </summary>
        [JsonProperty("domainsMode")]
        public eDomainsFilteringMode DomainsFilteringMode { get; set; } = eDomainsFilteringMode.Exclude;


        /// <summary>
        /// when set to true it will allow results from the same domain as the submitted url.
        /// </summary>
        [JsonProperty("allowSameDomain")]
        public bool AllowSameDomain { get; set; } = false;
    }
}
