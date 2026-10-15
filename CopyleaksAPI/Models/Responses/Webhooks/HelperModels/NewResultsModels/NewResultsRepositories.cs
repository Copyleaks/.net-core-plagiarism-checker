using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NewResultsModels
{
    public class NewResultsRepositories:SharedResultsModel
    {
        /// <summary>The repository Id that has the result.</summary>
        [JsonProperty("repositoryId")]
        public string RepositoryId {  get; set; }

        /// <summary>Metadata object</summary>
        [JsonProperty("metadata")]
        public new RepositoryMetadata Metadata { get; set; }
    }
}
