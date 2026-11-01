using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Copyleaks.SDK.V3.API.Models.Requests
{
	/// <summary>Start `priceChecked` scans.</summary>
	public class StartRequest
    {
        /// <summary>Scan ids to start.</summary>
        [JsonProperty("trigger")]
        [Required]
        public string[] Trigger { get; set; }

        /// <summary>If any scan id fails: Cancel = cancel the entire request, Ignore = start the others.</summary>
        [JsonProperty("errorHandling")]
        public eErrorHandling ErrorHandlingMode { get; set; } = eErrorHandling.Cancel;
    }
}
