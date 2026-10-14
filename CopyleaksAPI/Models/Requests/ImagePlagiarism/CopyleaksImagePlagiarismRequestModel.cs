namespace Copyleaks.SDK.V3.API.Models.Requests.ImagePlagiarism
{
    /// <summary>
    /// The form fields of an Image Plagiarism Detection request. The image itself is passed to
    /// <c>CopyleaksImagePlagiarismApi.SubmitAsync</c> as a Stream or a byte array and is uploaded as multipart/form-data.
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
        /// The image file name including its extension, at most 255 characters. Required.
        /// Allowed extensions: .jpg, .jpeg, .png, .gif, .bmp, .webp, .ico.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Use sandbox mode to test your integration without consuming credits. Returns mock results.
        /// </summary>
        public bool Sandbox { get; set; } = false;

        /// <summary>
        /// A custom string that is echoed back in the response under DeveloperPayload.
        /// Leave it unset (null) to leave it out of the request.
        /// </summary>
        public string DeveloperPayload { get; set; }
    }
}
