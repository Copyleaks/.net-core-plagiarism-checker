using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class Repository
	{
		/// <summary>
		/// Id of a repository to add the scanned document to.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; set; }
	}
}
