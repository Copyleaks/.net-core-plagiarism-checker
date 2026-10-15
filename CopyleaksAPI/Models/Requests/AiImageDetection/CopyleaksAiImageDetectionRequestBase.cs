using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection
{
    /// <summary>Options shared by the synchronous and asynchronous AI image detection requests.</summary>
    public abstract class CopyleaksAiImageDetectionRequestBase
    {
        // FileName is declared by each request: the synchronous endpoint reads "filename", the asynchronous one "fileName".

        /// <summary>
        /// Detection model, full name or alias, e.g. <see cref="Constants.CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA"/>. Required.
        /// </summary>
        [Required]
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>Sandbox mode for development: mock results, no credits consumed. Default false.</summary>
        [JsonProperty("sandbox")]
        public bool Sandbox { get; set; } = false;

        /// <summary>
        /// Mask type (<see cref="Constants.CopyleaksAiImageMaskTypes"/>): "binary", "heatmap" (adds Result.Labels) or, for the
        /// synchronous submits only, "none" (omits Result). Null = server default ("binary").
        /// </summary>
        [JsonProperty("maskType", NullValueHandling = NullValueHandling.Ignore)]
        public string MaskType { get; set; }

        /// <summary>
        /// Objects to locate and segment, up to 3; the response then has DetectedObjects.
        /// Do not send an empty list: the server rejects it.
        /// </summary>
        [JsonProperty("objectDetection", NullValueHandling = NullValueHandling.Ignore)]
        public List<CopyleaksAiImageObjectDetectionItem> ObjectDetection { get; set; }
    }
}
