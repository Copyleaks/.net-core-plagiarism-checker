using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    public class Repositories:SharedResultsModel
    {
        [JsonProperty("repositoryId")]
        public string RepositoryId {  get; set; }
        [JsonProperty("tags")]
        public Tags[] Tags { get; set; }
        [JsonProperty("metadata")]
        public new RepositoryMetadata Metadata { get; set; }

    }
}
