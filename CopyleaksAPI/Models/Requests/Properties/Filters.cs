using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>Customizable result filters.</summary>
    public class Filters
    {
        /// <summary>Include identical matches.</summary>
        [JsonProperty("identicalEnabled")]
        public bool IdenticalEnabled { get; set; } = true;

        /// <summary>Include minor-changes matches.</summary>
        [JsonProperty("minorChangesEnabled")]
        public bool MinorChangesEnabled { get; set; } = true;

        /// <summary>Include related-meaning matches.</summary>
        [JsonProperty("relatedMeaningEnabled")]
        public bool RelatedMeaningEnabled { get; set; } = true;

        /// <summary>Minimum copied words to count as plagiarized content.</summary>
        [JsonProperty("minCopiedWords")]
        public ushort? minCopiedWords { get; set; } = null;

        /// <summary>Leave out explicit results (safe search is not 100% accurate).</summary>
        [JsonProperty("safeSearch")]
        public bool SafeSearch { get; set; } = false;

        /// <summary>Domains to include or exclude from sources.</summary>
        [JsonProperty("domains")]
        public string[] Domains { get; set; } = new string[] { };

        /// <summary>Whether `domains` is an include or exclude list.</summary>
        [JsonProperty("domainsMode")]
        public eDomainsFilteringMode DomainsFilteringMode { get; set; } = eDomainsFilteringMode.Exclude;


        /// <summary>Allow results from the same domain as the submitted URL.</summary>
        [JsonProperty("allowSameDomain")]
        public bool AllowSameDomain { get; set; } = false;
    }
}
