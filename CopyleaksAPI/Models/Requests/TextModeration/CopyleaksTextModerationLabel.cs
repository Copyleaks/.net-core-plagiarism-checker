using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.TextModeration
{
    public class CopyleaksTextModerationLabel
    {
        /// <summary>Label id.</summary>
        [JsonProperty("id")]
        public string Id { get; }

        public CopyleaksTextModerationLabel(string id)
        {
            Id = id;
        }
    }
}
