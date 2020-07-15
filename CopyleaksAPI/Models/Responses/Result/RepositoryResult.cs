using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
	public class RepositoryResult : BasicInternalResult
	{
		[JsonProperty("repositoryId")]
		public string RepositoryId { get; set; }
	}
}
