using Newtonsoft.Json;
using System.Collections.Generic;

namespace Copyleaks.SDK.V3.API.Models.Responses.WritingAssistant
{
    public class Corrections
    {
        [JsonProperty("text")]
        public TextCorrections Text { get; set; }
    }

    public class TextCorrections
    {
        [JsonProperty("chars")]
        public Chars Chars { get; set; }
    }

    public class Chars
    {
        [JsonProperty("types")]
        public List<int> Types { get; set; }

        [JsonProperty("starts")]
        public List<int> Starts { get; set; }

        [JsonProperty("lengths")]
        public List<int> Lengths { get; set; }

        [JsonProperty("operationTexts")]
        public List<string> OperationTexts { get; set; }
    }
}
