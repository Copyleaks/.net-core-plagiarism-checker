using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// The course of the submitted content (sent as properties.course).
    /// </summary>
    public class CourseEntity
    {
        /// <summary>
        /// A unique course identifier for tracking analytics.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    /// <summary>
    /// The assignment of the submitted content (sent as properties.assignment).
    /// </summary>
    public class AssignmentEntity
    {
        /// <summary>
        /// A unique assignment identifier for tracking analytics.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    /// <summary>
    /// The institution of the submitted content (sent as properties.institution).
    /// </summary>
    public class InstitutionEntity
    {
        /// <summary>
        /// A unique institution identifier for tracking analytics.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
