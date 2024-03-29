﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class IndexingEntity
	{
		/// <summary>
		/// Specify which repositories to index the scanned document to.
		/// </summary>
		public RepositoryIndexing[] Repositories { get; set; }
	}

	public class ClientIndexingEntity : IndexingEntity
	{
		/// <summary>
		/// The scanned document will be indexed to the Copyleaks internal database.
		/// </summary>
		public bool CopyleaksDb { get; set; }
	}
}
