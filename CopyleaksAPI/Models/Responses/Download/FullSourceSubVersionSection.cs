using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
    public abstract class BaseFulloSourceSubVersionSection
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class FullHtmlSourceSubVersionSection: BaseFulloSourceSubVersionSection
    {
        
        [JsonProperty("exclude")]
        public FullHtmlSourceExcludeSection Exclude { get; set; }
    }

    public class FullTextSourceSubVersionSection: BaseFulloSourceSubVersionSection
    {

        [JsonProperty("exclude")]
        public FullTextSourceExcludeSection Exclude { get; set; }

        [JsonProperty("pages")]
        public FullSourcePagesSection Pages { get; set; }
    }
}