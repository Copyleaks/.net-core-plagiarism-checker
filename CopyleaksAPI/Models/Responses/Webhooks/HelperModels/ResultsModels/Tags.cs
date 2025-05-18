using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    public class Tags
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

    }
}
