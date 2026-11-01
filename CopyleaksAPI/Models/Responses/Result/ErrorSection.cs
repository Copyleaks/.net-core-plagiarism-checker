using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
    public class ErrorSection
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public sbyte Code { get; set; }
    }
}