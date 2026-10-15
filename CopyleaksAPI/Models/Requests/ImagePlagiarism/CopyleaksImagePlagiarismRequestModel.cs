namespace Copyleaks.SDK.V3.API.Models.Requests.ImagePlagiarism
{
    /// <summary>
    /// Form fields of an Image Plagiarism Detection request. The image itself goes to
    /// <c>CopyleaksImagePlagiarismApi.SubmitAsync</c> as a Stream or byte array (multipart/form-data).
    /// </summary>
    public class CopyleaksImagePlagiarismRequestModel
    {
        public CopyleaksImagePlagiarismRequestModel()
        {
        }

        public CopyleaksImagePlagiarismRequestModel(string filename, bool sandbox = false)
        {
            Filename = filename;
            Sandbox = sandbox;
        }

        /// <summary>
        /// Image file name with extension, at most 255 characters. Required.
        /// Allowed: .jpg, .jpeg, .png, .gif, .bmp, .webp, .ico.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>Sandbox mode: mock results, no credits consumed.</summary>
        public bool Sandbox { get; set; } = false;

        /// <summary>Custom string echoed back in the response's DeveloperPayload. Null = not sent.</summary>
        public string DeveloperPayload { get; set; }
    }
}
