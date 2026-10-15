using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>Course reference.</summary>
    public class CourseEntity
    {
        /// <summary>Unique course id.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    /// <summary>Assignment reference.</summary>
    public class AssignmentEntity
    {
        /// <summary>Unique assignment id.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    /// <summary>Institution reference.</summary>
    public class InstitutionEntity
    {
        /// <summary>Unique institution id.</summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
