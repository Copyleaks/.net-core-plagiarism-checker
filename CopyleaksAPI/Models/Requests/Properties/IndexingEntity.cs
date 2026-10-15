using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class IndexingEntity
	{
		/// <summary>Repositories to index the scanned document to.</summary>
		public RepositoryIndexing[] Repositories { get; set; }
	}

	public class ClientIndexingEntity : IndexingEntity
	{
		/// <summary>Index the scanned document to the Copyleaks internal database.</summary>
		public bool CopyleaksDb { get; set; }
	}
}
