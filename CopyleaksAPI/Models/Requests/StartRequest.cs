// CR : License missing
using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
	// CR : Documentation
	public class StartRequest
    {
        [JsonProperty("trigger")]
        [Required]
        public string[] Trigger { get; set; }

        [JsonProperty("errorHandling")]
        public eErrorHandling ErrorHandlingMode { get; set; } = eErrorHandling.Cancel;
    }

	// CR : Seperate to diffrent file.
	// CR : Documentation
	public class StartBatchRequest : StartRequest
    {
        [JsonProperty("include")]
        public string[] Include { get; set; }
    }
   
}
