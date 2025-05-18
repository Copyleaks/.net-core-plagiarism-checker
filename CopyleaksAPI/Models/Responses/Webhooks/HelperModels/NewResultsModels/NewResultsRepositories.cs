using System;
using System.Collections.Generic;
using System.Text;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NewResultsModels
{
    public class NewResultsRepositories:SharedResultsModel
    {
        [JsonProperty("repositoryId")]
        public string RepositoryId {  get; set; }
        [JsonProperty("metdata")]
        public new RepositoryMetadata Metdata { get; set; }
    }
}
