using System.Collections.Generic;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.CitationChecker
{
    /// <summary>
    /// Citation Checker results, returned when the scan was submitted with References.Validate set to true.
    /// The completed webhook has the <see cref="Summary"/> only. The crawled version export also has the per-reference <see cref="Results"/>.
    /// </summary>
    public class ReferencesValidation
    {
        [JsonProperty("summary")]
        public ReferencesValidationSummary Summary { get; set; }

        /// <summary>
        /// One entry per detected reference. Null in the completed webhook.
        /// </summary>
        [JsonProperty("results")]
        public List<ReferenceValidationResult> Results { get; set; }
    }

    /// <summary>
    /// An aggregated count of the references that were detected and validated.
    /// </summary>
    public class ReferencesValidationSummary
    {
        /// <summary>
        /// The total number of references detected in the scanned content.
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; set; }

        /// <summary>
        /// The number of detected references that were identified as academic.
        /// </summary>
        [JsonProperty("academic")]
        public int Academic { get; set; }

        /// <summary>
        /// The number of academic references that were fully corroborated against the Copyleaks academic citation index.
        /// </summary>
        [JsonProperty("academicValidated")]
        public int AcademicValidated { get; set; }

        /// <summary>
        /// The number of non-academic references that were fully corroborated against their cited source.
        /// </summary>
        [JsonProperty("nonAcademicValidated")]
        public int NonAcademicValidated { get; set; }
    }

    /// <summary>
    /// A detected reference, what was parsed from it, where it appears and the sources that corroborate it.
    /// </summary>
    public class ReferenceValidationResult
    {
        /// <summary>
        /// "academic" or "nonAcademic".
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Character offsets of the citation in the extracted text.
        /// </summary>
        [JsonProperty("textRanges")]
        public ReferenceRanges TextRanges { get; set; }

        /// <summary>
        /// Positions of the citation in the rendered HTML report.
        /// </summary>
        [JsonProperty("htmlRanges")]
        public ReferenceRanges HtmlRanges { get; set; }

        [JsonProperty("parsed")]
        public ParsedReference Parsed { get; set; }

        /// <summary>
        /// The corroborating sources found for the reference. Empty when none were found.
        /// </summary>
        [JsonProperty("suggestions")]
        public List<ReferenceSuggestion> Suggestions { get; set; }
    }

    public class ReferenceRanges
    {
        [JsonProperty("starts")]
        public List<int> Starts { get; set; }

        [JsonProperty("lengths")]
        public List<int> Lengths { get; set; }

        /// <summary>
        /// Groups the spans that belong to one citation. HTML ranges only.
        /// </summary>
        [JsonProperty("groupIds")]
        public List<int> GroupIds { get; set; }
    }

    /// <summary>
    /// The fields parsed from the citation.
    /// </summary>
    public class ParsedReference
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("authors")]
        public List<string> Authors { get; set; }

        [JsonProperty("year")]
        public int? Year { get; set; }

        [JsonProperty("journal")]
        public string Journal { get; set; }

        /// <summary>
        /// The cited link. Present for non-academic references.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    /// <summary>
    /// A source that corroborates a reference.
    /// </summary>
    public class ReferenceSuggestion
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("doi")]
        public string Doi { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("authors")]
        public List<string> Authors { get; set; }

        [JsonProperty("publishedYear")]
        public int? PublishedYear { get; set; }

        [JsonProperty("modifiedYear")]
        public int? ModifiedYear { get; set; }

        [JsonProperty("journal")]
        public string Journal { get; set; }

        /// <summary>
        /// Which parsed fields this suggestion corroborates. A null signal was not checked.
        /// </summary>
        [JsonProperty("signals")]
        public ReferenceSignals Signals { get; set; }
    }

    public class ReferenceSignals
    {
        [JsonProperty("title")]
        public bool? Title { get; set; }

        [JsonProperty("year")]
        public bool? Year { get; set; }

        [JsonProperty("authors")]
        public bool? Authors { get; set; }
    }
}
