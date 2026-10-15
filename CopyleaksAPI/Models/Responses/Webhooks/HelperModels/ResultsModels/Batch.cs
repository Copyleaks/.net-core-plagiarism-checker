using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    public class Batch : SharedResultsModel
    {
        /// <summary>Tags object array</summary>
        [JsonProperty("tags")]
        public Tags[] Tags { get; set; }
    }
}
