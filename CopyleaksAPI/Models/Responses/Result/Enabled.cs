using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
	public class Enabled
	{
		/// <summary>Whether plagiarism detection is enabled.</summary>
		[JsonProperty("plagiarismDetection")]
		public bool PlagiarismDetection { get; set; }

		/// <summary>Whether AI detection is enabled.</summary>
		[JsonProperty("aiDetection")]
		public bool AiDetection { get; set; }

        /// <summary>Whether AI source match is enabled.</summary>
        [JsonProperty("aiSourceMatch")]
        public bool AISourceMatch { get; set; } = false;
    }
}
