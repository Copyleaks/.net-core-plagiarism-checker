using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NewResultsModels
{
    public class NewResultScore
    {
        [JsonProperty("aggregatedScore")]
        public decimal AggregatedScore {  get; set; }
    }
}
