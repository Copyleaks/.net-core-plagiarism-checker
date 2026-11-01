using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
    /// <summary>The progress of a scan</summary>
	public class ProgressResponse
    {
        /// <summary>Scan progress in percent.</summary>
        [JsonProperty("percents")]
        public ushort Percents { get; set; }
    }
}
