using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	/// <summary>Which mediums to scan.</summary>
	public class ProtectionLayers
	{
		/// <summary>Scan against the internet.</summary>
		[JsonProperty("internet")]
		public bool Internet { get; set; } = true;

	}

	/// <summary>Which mediums to scan, including internal sources.</summary>
	public class ClientProtectionLayers : ProtectionLayers
	{
		/// <summary>
		/// Scan against the Copyleaks internal database. If enabled, the scanned document is also indexed into it.
		/// </summary>
		[JsonProperty("copyleaksDB")]
		public InternalDatabaseScan CopyleaksInternalDB { get; set; } = new InternalDatabaseScan();

		[JsonProperty("exclude")]
		public ExcludeResults Exclude { get; set; } = new ExcludeResults();

		/// <summary>Include only internal database and repository results whose scan id matches a pattern.</summary>
		[JsonProperty("include", NullValueHandling = NullValueHandling.Ignore)]
		public ScanningInclude Include { get; set; }

		/// <summary>Repositories to index the scanned document to.</summary>
		[JsonProperty("repositories")]
		public RepositoryScanning[] Repositories { get; set; }

		[JsonProperty("crossLanguages")]
        public CrossLanguages CrossLanguages { get; set; } = new CrossLanguages();
	}

}
