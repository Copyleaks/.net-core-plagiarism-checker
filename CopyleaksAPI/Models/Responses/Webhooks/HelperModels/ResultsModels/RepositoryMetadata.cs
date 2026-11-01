using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    public class RepositoryMetadata : Metadata
    {
        /// <summary>Email of the user which added this document to the repository.</summary>
        [JsonProperty("submittedBy")]
        public string SubmittedBy { get; set; }

    }
}
