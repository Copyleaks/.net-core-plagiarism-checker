using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
    /// <summary>Request to delete scans.</summary>
    public class DeleteRequest
    {
        /// <summary>Scans to delete.</summary>
        [JsonProperty("scans")]
        public DeleteScanItem[] Scans { get; set; }

        /// <summary>Also delete from the internal database; purged scans no longer appear as results.</summary>
        [JsonProperty("purge")]
        public bool Purge { get; set; } = false;

        /// <summary>URL that Copyleaks calls with POST (no body) when the removal is completed.</summary>
        [JsonProperty("completionWebhook", NullValueHandling = NullValueHandling.Ignore)]
        public Uri CompletionWebhook { get; set; }

        /// <summary>Headers for the completion webhook, as [name, value] rows.</summary>
        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public string[,] Headers { get; set; }

    }

    /// <summary>A scan to delete.</summary>
    public class DeleteScanItem
    {
        /// <summary>Scan id.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }

}
    

