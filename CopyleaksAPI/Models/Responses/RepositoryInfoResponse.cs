using System;
using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
    /// <summary>Private Cloud Hub repository information.</summary>
    public class RepositoryInfoResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>Your role in the repository: 1 Viewer, 2 Contributor, 3 Admin, 4 Super Admin.</summary>
        [JsonProperty("permission")]
        public int Permission { get; set; }

        /// <summary>Status: 0 Running, 1 Pending For Deletion, 2 Error, 3 Updating, 4 Payment Required.</summary>
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("maxCredits")]
        public long? MaxCredits { get; set; }

        [JsonProperty("currentCredits")]
        public long? CurrentCredits { get; set; }

        [JsonProperty("maskingPolicy")]
        public eMaskingPolicy? MaskingPolicy { get; set; }

        [JsonProperty("creationTime")]
        public DateTime? CreationTime { get; set; }
    }
}
