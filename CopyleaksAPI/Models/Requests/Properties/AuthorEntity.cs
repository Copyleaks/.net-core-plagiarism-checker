using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>Author of the submitted content.</summary>
    public class AuthorEntity
    {
        /// <summary>Unique author id.</summary>
        [JsonProperty("id")]
        public string Id { get; set; } = null;
    }
}
