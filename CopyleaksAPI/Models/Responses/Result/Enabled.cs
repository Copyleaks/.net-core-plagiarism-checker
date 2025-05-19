using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
	public class Enabled
	{
		/// <summary>
		/// Determines whether the PlagiarismDetection is enabled or disabled
		/// </summary>
		[JsonProperty("plagiarismDetection")]
		public bool PlagiarismDetection { get; set; }

		/// <summary>
		/// Determines whether the AiDetection is enabled or disabled
		/// </summary>
		[JsonProperty("aiDetection")]
		public bool AiDetection { get; set; }

        /// <summary>
        /// Determines whether the aiSourceMatch is enabled or disabled
        /// </summary>
        [JsonProperty("aiSourceMatch")]
        public bool AISourceMatch { get; set; } = false;
    }
}
