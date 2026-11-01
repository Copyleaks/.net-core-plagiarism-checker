using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
    public class InternetResult : BasicResult
    {
        /// <summary>Public URL of the resource</summary>
		[JsonProperty("url")]
        public string Url { get; set; }
    }
}