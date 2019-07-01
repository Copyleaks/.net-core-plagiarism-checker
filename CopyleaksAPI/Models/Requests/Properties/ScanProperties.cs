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
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// The scan request properties
    /// </summary>
    public abstract class ScanProperties
    {
        /// <summary>
		/// Define which type of task it is.
		/// </summary>
		[JsonProperty("action")]
        public eSubmitAction Action { get; set; } = eSubmitAction.Scan;

        /// <summary>
        /// Define in which format to return the results. 
        /// </summary>
        [JsonProperty("includeHtml")]
        public bool IncludeHtml{ get; set; } = false;

        /// <summary>
        /// Developer payload string.
        /// </summary>
        [JsonProperty("developerPayload")]
        [StringLength(512)]
        public string DeveloperPayload { get; set; }

        /// <summary>
		/// Scan priority
		/// </summary>
		[JsonProperty("priority")]
        public eScanPriority Priority { get; set; } = eScanPriority.Normal;

        /// <summary>
        /// Enable sandbox scan.
        /// </summary>
        [JsonProperty("sandbox")]
        public bool Sandbox { get; set; } = false;

        /// <summary>
        /// Getting the client notify changes on server.
        /// </summary>
        [JsonProperty("webhooks")]
        public Webhooks Webhooks { get; set; } = new Webhooks();

        /// <summary>
        /// How much time, in hours, to store the scan on the database.
        /// </summary>
        [JsonProperty("expiration")]
        public uint Expiration { get; set; } = 2880;

        [JsonProperty("filters")]
        public Filters Filters { get; set; } = new Filters();

        [JsonProperty("author")]
        public AuthorEntity Author { get; set; } = new AuthorEntity();

        /// <summary>
        /// The prcision level of the process.
        /// Range of values between 1 to 5.
        /// Setting a higher precision level will pontentially cause more precise results and the will increase the scan completion time.
        /// </summary>
        [JsonProperty("precisionLevel")]
        [Range(1,5)]
        public int PrecisionLevel { get; set; }

    }
}
