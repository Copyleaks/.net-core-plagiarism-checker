using Newtonsoft.Json;
using System.Collections.Generic;

namespace Copyleaks.SDK.V3.API.Models.Responses.WritingFeedback
{
    public class CorrectionTypes
    {
        /// <summary>List of correction types and corresponding user interface information.</summary>
        [JsonProperty("correctionTypes")]
        public List<CorrectionType> CorrectionType { get; set; }
    }

    public class CorrectionType
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("category")]
        public int Category { get; set; }
    }
}
