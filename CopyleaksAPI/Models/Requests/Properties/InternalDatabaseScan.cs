using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    public class InternalDatabaseScan
    {
        /// <summary>Check against your previously submitted documents.</summary>
        [JsonProperty("includeMySubmissions")]
        public bool IncludeMySubmissions { get; set; } = false;

        /// <summary>Check against documents other users previously submitted.</summary>
        [JsonProperty("includeOthersSubmissions")]
        public bool IncludeOthersSubmissions { get; set; } = false;
    }
}