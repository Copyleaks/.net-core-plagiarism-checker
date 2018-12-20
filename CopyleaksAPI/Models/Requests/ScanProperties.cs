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
using System;
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
	// CR : Documentation is missing. 
	public class ScanProperties
    {
        /// <summary>
		/// Define which type of task it is.
		/// </summary>
		[JsonProperty("action")]
        public eSubmitAction Action { get; set; } = eSubmitAction.Scan;

        /// <summary>
        /// Define in which format to return the results. 
        /// </summary>
        [JsonProperty("outputMode")]
        public eSubmitOutputMode OutputMode { get; set; } = eSubmitOutputMode.TXT;

        /// <summary>
        /// Developer payload string.
        /// </summary>
        [JsonProperty("developerPayload")]
        [StringLength(512)]
        public string DeveloperPayload { get; set; }

        /// <summary>
        /// Enable sandbox scan.
        /// </summary>
        [JsonProperty("sandbox")]
        public bool Sandbox { get; set; } = false;

        /// <summary>
        /// Getting the client notify changes on server.
        /// </summary>
        [JsonProperty("callbacks")]
        public Callbacks CallbacksSection { get; set; } = new Callbacks();

        /// <summary>
        /// How much time, in hours, to store the scan on the database.
        /// </summary>
        [JsonProperty("experation")]
        [Range(1, 2880)] // Up to 4 months
        public uint Experation { get; set; } = 2880;

        [JsonProperty("scanning")]
        public ProtectionLayers ProtectionLayersSection { get; set; } = new ProtectionLayers();

        [JsonProperty("exclude")]
        public Exclude ExcludeSection { get; set; } = new Exclude();

        [JsonProperty("filters")]
        public Filters FiltersSection { get; set; } = new Filters();

        [JsonProperty("author")]
        public AuthorEntity Author { get; set; } = new AuthorEntity();

        [JsonProperty("reportExport")]
        public ReportCustomization ReportSection { get; set; } = null;

    }

	// CR : Seperate to diffrent file.
    /// <summary>
    /// Defines which mediums to scan.
    /// </summary>
    public class ProtectionLayers
    {
        /// <summary>
        /// Scan agianst the internet?
        /// </summary>
        [JsonProperty("internet")]
        public bool Internet { get; set; } = true;

        /// <summary>
        /// Scan agianst copyleaks internal database?
        /// If specified true, it will also index the scanned document into the database.
        /// </summary>
        [JsonProperty("copyleaksDB")]
        public bool CopyleaksInternalDB { get; set; } = true;
    }

	// CR : Seperate to diffrent file.
	// CR : Documentation
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
        /// When HTML document, exlude the document template from the scan. 
        /// </summary>
        [JsonProperty("htmlTemplate")]
        public bool HtmlTemplate { get; set; } = false;

    }

	// CR : Seperate to diffrent file.
	// CR : Documentation
	public class Filters
    {
        [JsonProperty("idenitcalEnabled")]
        public bool IdenitcalEnabled { get; set; } = true;

        [JsonProperty("minorChangedEnabled")]
        public bool MinorChangedEnabled { get; set; } = true;

        [JsonProperty("relatedMeaningEnabled")]
        public bool RelatedMeaningEnabled { get; set; } = true;

        [JsonProperty("minCopiedWords")]
        public ushort? minCopiedWords { get; set; } = null;

        [JsonProperty("safeSearch")]
        public bool SafeSearch { get; set; } = false;

        [JsonProperty("domains")]
        //[DomainListsValidator(MaxLength = 512)] // CR : Remove
        public string[] Domains { get; set; } = new string[] { };

        [JsonProperty("domainsMode")]
        public eDomainsFilteringMode DomainsFilteringMode { get; set; } = eDomainsFilteringMode.WhiteList;
    }

	// CR : Seperate to diffrent file.
	// CR : Documentation
	public class Callbacks
    {
        [JsonProperty("completion")]
        public Uri Completion { get; set; }

        [JsonProperty("onNewResult")]
        public Uri NewResult { get; set; }
    }

	// CR : Seperate to diffrent file.
	// CR : Documentation
	public class AuthorEntity
    {
        [StringLength(36)] // CR : Needed?
        [JsonProperty("id")]
        public string Id { get; set; } = null;
    }
}
