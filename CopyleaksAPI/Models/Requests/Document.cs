using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
	
    /// <summary>A document to submit for scanning.</summary>
	public class Document
    {
        /// <summary>Scan properties.</summary>
        [JsonProperty("properties")]
        public ScanProperties PropertiesSection { get; set; }
    }
}
