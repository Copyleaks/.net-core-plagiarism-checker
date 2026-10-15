using System;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    public class CrossLanguages
    {
        /// <summary>Languages to scan your content against (cross-language plagiarism detection).</summary>
        [JsonProperty("languages")]
        public Language[] Languages { get; set; } = Array.Empty<Language>();
    }
}
