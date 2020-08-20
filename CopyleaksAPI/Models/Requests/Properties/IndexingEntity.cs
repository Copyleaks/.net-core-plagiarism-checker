using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class IndexingEntity
	{
		/// <summary>
		/// Specify which repositories to index the scanned document to.
		/// </summary>
		public Repository[] Repositories { get; set; }
	}
}
