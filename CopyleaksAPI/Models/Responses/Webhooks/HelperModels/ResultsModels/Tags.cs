using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    public class Tags
    {
        /// <summary>Unique ID that signifies the type of result tag.</summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>A simple name for this tag.</summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>Short text describing this tag.</summary>
        [JsonProperty("description")]
        public string Description { get; set; }

    }
}
