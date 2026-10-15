using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
	/// <summary>Scan delete response.</summary>
	public class DeleteResponse
    {
        /// <summary>Scans that failed to be deleted.</summary>
        [JsonProperty(PropertyName = "failures")]
        public Failure [] Failures { get; set; }
    }
}
