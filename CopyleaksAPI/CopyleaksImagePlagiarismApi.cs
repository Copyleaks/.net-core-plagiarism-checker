using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Requests.ImagePlagiarism;
using Copyleaks.SDK.V3.API.Models.Responses.ImagePlagiarism;

namespace Copyleaks.SDK.V3.API
{
    /// <summary>
    /// Copyleaks Image Plagiarism Detection: search the web for copies of an image, in one synchronous call.
    /// </summary>
    public class CopyleaksImagePlagiarismApi : CopyleaksBase
    {
        public string CopyleaksApiServer { get; private set; }

        private string ImagePlagiarismApiVersion { get; set; }

        /// <summary>
        /// Conection to Copyleaks Image Plagiarism Detection API
        /// </summary>
        /// <param name="client">The HttpClient to use, for example one created by IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksImagePlagiarismApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>
        /// Conection to Copyleaks Image Plagiarism Detection API, using the shared HttpClient of the SDK.
        /// </summary>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksImagePlagiarismApi(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null) : base(clientCertificate, options)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ResolveApiEndpoint();
            this.ImagePlagiarismApiVersion = ConfigurationManager.Configuration[CopyleaksConstants.ImagePlagiarismApiVersion];
        }

        /// <summary>
        /// Search the web for copies of an image. The image is uploaded as multipart/form-data.
        /// It must be smaller than 20MB and at most 75 megapixels, in JPG, JPEG, PNG, GIF, BMP, WebP or ICO format.
        /// The stream is read from its current position and is not disposed.
        /// </summary>
        /// <param name="scanId">A unique scan Id, 3 to 36 characters</param>
        /// <param name="image">The image content</param>
        /// <param name="requestModel">The filename and options</param>
        /// <param name="token">A copyleaks access token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task<CopyleaksImagePlagiarismResponseModel> SubmitAsync(string scanId, Stream image, CopyleaksImagePlagiarismRequestModel requestModel, string token, CancellationToken cancellationToken = default)
        {
            Validate(scanId, requestModel, token);
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            long start = image.CanSeek ? image.Position : 0;
            // Charged per call, so only a rate-limited (429) attempt is retried, and only when the stream can be read again.
            return await SendForJsonAsync<CopyleaksImagePlagiarismResponseModel>(
                HttpMethod.Post, CheckUri(scanId), token,
                () => BuildForm(new SourceStreamContent(image, start), requestModel),
                image.CanSeek ? eRetryMode.RateLimitOnly : eRetryMode.None, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Search the web for copies of an image. The image is uploaded as multipart/form-data.
        /// </summary>
        /// <param name="scanId">A unique scan Id, 3 to 36 characters</param>
        /// <param name="image">The image bytes</param>
        /// <param name="requestModel">The filename and options</param>
        /// <param name="token">A copyleaks access token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task<CopyleaksImagePlagiarismResponseModel> SubmitAsync(string scanId, byte[] image, CopyleaksImagePlagiarismRequestModel requestModel, string token, CancellationToken cancellationToken = default)
        {
            Validate(scanId, requestModel, token);
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            return await SendForJsonAsync<CopyleaksImagePlagiarismResponseModel>(
                HttpMethod.Post, CheckUri(scanId), token,
                () => BuildForm(new ByteArrayContent(image), requestModel),
                eRetryMode.RateLimitOnly, cancellationToken).ConfigureAwait(false);
        }

        private string CheckUri(string scanId) =>
            $"{this.CopyleaksApiServer}{this.ImagePlagiarismApiVersion}/image-plagiarism-detector/{PathSegment(scanId)}/check";

        private static void Validate(string scanId, CopyleaksImagePlagiarismRequestModel requestModel, string token)
        {
            if (string.IsNullOrEmpty(scanId))
                throw new ArgumentNullException(nameof(scanId), "ScanId is mandatory");
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token), "Token is mandatory");
            if (requestModel == null)
                throw new ArgumentNullException(nameof(requestModel));
            if (string.IsNullOrEmpty(requestModel.Filename))
                throw new ArgumentException("Filename is mandatory.", nameof(requestModel.Filename));
        }

        private static MultipartFormDataContent BuildForm(HttpContent image, CopyleaksImagePlagiarismRequestModel requestModel)
        {
            var form = new MultipartFormDataContent();
            MultipartHelper.AddFile(form, "image", image, requestModel.Filename);
            MultipartHelper.AddField(form, "filename", requestModel.Filename);
            MultipartHelper.AddField(form, "sandbox", requestModel.Sandbox ? "true" : "false");
            if (requestModel.DeveloperPayload != null)
                MultipartHelper.AddField(form, "developerPayload", requestModel.DeveloperPayload);
            return form;
        }
    }
}
