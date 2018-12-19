
using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
    public class StartRequest
    {
        [JsonProperty("trigger")]
        [Required]
        public string[] Trigger { get; set; }

        [JsonProperty("errorHandling")]
        public eErrorHandling ErrorHandlingMode { get; set; } = eErrorHandling.Cancel;
    }

    public class StartBatchRequest : StartRequest
    {
        [JsonProperty("include")]
        public string[] Include { get; set; }
    }
   
}
