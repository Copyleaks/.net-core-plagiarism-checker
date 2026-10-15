using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
    public class UrlDocument: Document
    {
        [JsonProperty("url")]
        [Required]
        public Uri Url { get; set; }

        /// <summary>Custom headers for fetching the URL, as [name, value] rows.</summary>
        [JsonProperty("headers")]
        public string[,] Headers { get; set; } = null;

        /// <summary>HTTP method used to fetch the URL: "GET", "POST" or "PUT". Null = server default ("GET").</summary>
        [JsonProperty("verb", NullValueHandling = NullValueHandling.Ignore)]
        public string Verb { get; set; }

    }
}