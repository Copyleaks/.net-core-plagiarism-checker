using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class RepositoryScanning
	{
		/// <summary>
		/// Id of a repository to add the scanned document to.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; set; }

		/// <summary>
		/// Check against documents I previously submitted
		/// </summary>
		[JsonProperty("includeMySubmissions")]
		public bool IncludeMySubmissions { get; set; } = false;

		/// <summary>
		/// Check against documents other users previously submitted
		/// </summary>
		[JsonProperty("includeOthersSubmissions")]
		public bool IncludeOthersSubmissions { get; set; } = false;
	}
}
