using System;
using System.Collections.Generic;
using System.Text;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NewResultsModels
{
    public class NewResultInternet
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title{get;set;}
        [JsonProperty("introduction")]
        public string Introduction{get;set;}
        [JsonProperty("matchedWords")]
        public int MatchedWords{get;set;}
        [JsonProperty("scanId")]
        public string ScanId{get;set;}
        [JsonProperty("metadata")]
        public Metadata Metadata{get;set;}
        [JsonProperty("url")]
        public string Url{get;set;}
    }
}
