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
    /// <summary>Image Plagiarism Detection: search the web for copies of an image, in one synchronous call.</summary>
    public class CopyleaksImagePlagiarismApi : CopyleaksBase
    {
        public string CopyleaksApiServer { get; private set; }

        private string ImagePlagiarismApiVersion { get; set; }

        /// <summary>Uses the given HttpClient.</summary>
        /// <param name="client">For example one from IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings.</param>
        public CopyleaksImagePlagiarismApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>Uses the SDK's shared HttpClient, with an optional client certificate and options.</summary>
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
        /// Search the web for copies of an image: under 20 MB, at most 75 megapixels, JPG, JPEG, PNG, GIF, BMP, WebP or ICO.
        /// <paramref name="scanId"/> is 3 to 36 characters. The stream is read from its current position and is not disposed.
        /// </summary>
        /// <exception cref="CopyleaksHttpException">The server rejected the request.</exception>
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

        /// <summary>Search the web for copies of an image. <paramref name="scanId"/> is 3 to 36 characters.</summary>
        /// <exception cref="CopyleaksHttpException">The server rejected the request.</exception>
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
