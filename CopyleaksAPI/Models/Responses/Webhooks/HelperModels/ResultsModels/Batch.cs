using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    public class Batch : SharedResultsModel
    {
        [JsonProperty("tags")]
        public Tags[] Tags { get; set; }
    }
}
