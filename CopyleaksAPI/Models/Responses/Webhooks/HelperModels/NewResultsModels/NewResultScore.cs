using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NewResultsModels
{
    public class NewResultScore
    {
        /// <summary>Aggregated similar-word percentage, not counting excluded references, quotations, etc.</summary>
        [JsonProperty("aggregatedScore")]
        public decimal AggregatedScore {  get; set; }
    }
}
