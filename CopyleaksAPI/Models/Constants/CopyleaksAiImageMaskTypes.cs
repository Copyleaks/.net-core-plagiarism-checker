namespace Copyleaks.SDK.V3.API.Models.Constants
{
    /// <summary>
    /// Values of maskType for AI image detection.
    /// </summary>
    public static class CopyleaksAiImageMaskTypes
    {
        /// <summary>
        /// The default: a Run-Length Encoded mask of the regions flagged as AI-generated (result.starts and result.lengths).
        /// </summary>
        public const string BINARY = "binary";

        /// <summary>
        /// A gradient mask: adds result.labels, one confidence band (0 to 4) per run.
        /// </summary>
        public const string HEATMAP = "heatmap";

        /// <summary>
        /// No mask: the response has no result object. Supported by the synchronous endpoint only.
        /// </summary>
        public const string NONE = "none";
    }
}
