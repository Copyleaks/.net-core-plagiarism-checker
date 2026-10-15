using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
    public abstract class FullSuspectedComparisonSection<FULL_SUSPECTED_COMPARISON_DESCRIPTION>
    {
        [JsonProperty("identical")]
        public FULL_SUSPECTED_COMPARISON_DESCRIPTION Identical { get; set; }

        [JsonProperty("minorChanges")]
        public FULL_SUSPECTED_COMPARISON_DESCRIPTION MinorChanges { get; set; }

        [JsonProperty("relatedMeaning")]
        public FULL_SUSPECTED_COMPARISON_DESCRIPTION RelatedMeaning { get; set; }
    }
    public class FullTextSuspectedComparisonSection : FullSuspectedComparisonSection<FullTextSuspectedComparisonDescription>
    {
    }

    public class FullHtmlSuspectedComparisonSection: FullSuspectedComparisonSection<FullHtmlSuspectedComparisonDescription>
    {
    }
}