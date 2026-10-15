using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
    public abstract class FullSuspectedVersionSection<FULL_SUSPECTED_COMPARISON_DESCRIPTION>
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("pages")]
        public FullSuspectedPagesSection PagesSection { get; set; }

        [JsonProperty("comparison")]
        public FULL_SUSPECTED_COMPARISON_DESCRIPTION Comparison { get; set; }
    }

    public class FullTextSuspectedVersionSection : FullSuspectedVersionSection<FullTextSuspectedComparisonSection> { }

    public class FullHtmlSuspectedVersionSection : FullSuspectedVersionSection<FullHtmlSuspectedComparisonSection> { }
    
}