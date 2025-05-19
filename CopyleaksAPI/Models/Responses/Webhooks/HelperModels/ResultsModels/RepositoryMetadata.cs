using System;
using System.Collections.Generic;
using System.Text;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.BaseModels;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ResultsModels
{
    public class RepositoryMetadata : Metadata
    {
        [JsonProperty("submittedBy")]
        public string SubmittedBy { get; set; }

    }
}
