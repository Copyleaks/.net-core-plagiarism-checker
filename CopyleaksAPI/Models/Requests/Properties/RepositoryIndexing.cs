using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class RepositoryIndexing
	{
		/// <summary>
		/// Id of a repository to add the scanned document to.
		/// </summary>
		[JsonProperty("id")]
		public string Id { get; set; }

		/// <summary>
		/// specify the document maskig ploicy
		/// </summary>
		[JsonProperty("maskingPolicy")]
		[Range(0, 2)]
		public eMaskingPolicy MaskingPolicy { get; set; } = eMaskingPolicy.NoMask;
	}
}
