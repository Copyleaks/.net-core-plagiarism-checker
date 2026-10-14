using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Requests.AiVideoDetection;

namespace Copyleaks.SDK.V3.API
{
    /// <summary>
    /// Copyleaks AI Video Detection: submit a video URL, and the result is delivered to your webhook.
    /// </summary>
    public class CopyleaksAiVideoDetectionApi : CopyleaksBase
    {
        public string CopyleaksApiServer { get; private set; }

        private string VideoDetectionApiVersion { get; set; }

        /// <summary>
        /// Conection to Copyleaks AI Video Detection API
        /// </summary>
        /// <param name="client">The HttpClient to use, for example one created by IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksAiVideoDetectionApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>
        /// Conection to Copyleaks AI Video Detection API, using the shared HttpClient of the SDK.
        /// </summary>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksAiVideoDetectionApi(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null) : base(clientCertificate, options)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ResolveApiEndpoint();
            this.VideoDetectionApiVersion = ConfigurationManager.Configuration[CopyleaksConstants.VideoDetectionApiVersion];
        }

        /// <summary>
        /// Submit a video URL for AI-generated content detection. The request returns as soon as it is accepted
        /// (201 Created). The result is posted to <see cref="CopyleaksAiVideoDetectionRequestModel.Webhooks"/> as a
        /// <c>CopyleaksAiVideoDetectionResponseModel</c>.
        /// </summary>
        /// <param name="scanId">A unique scan Id, 3 to 36 characters</param>
        /// <param name="requestModel">The video URL, filename, model, webhook and options</param>
        /// <param name="token">A copyleaks access token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task SubmitAsync(string scanId, CopyleaksAiVideoDetectionRequestModel requestModel, string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(scanId))
                throw new ArgumentNullException(nameof(scanId), "ScanId is mandatory");
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token), "Token is mandatory");
            if (requestModel == null)
                throw new ArgumentNullException(nameof(requestModel));
            if (requestModel.Url == null)
                throw new ArgumentException("Url is mandatory.", nameof(requestModel.Url));
            if (string.IsNullOrEmpty(requestModel.Filename))
                throw new ArgumentException("Filename is mandatory.", nameof(requestModel.Filename));
            if (string.IsNullOrEmpty(requestModel.Model))
                throw new ArgumentException("Model is mandatory.", nameof(requestModel.Model));
            if (requestModel.Webhooks?.Url == null)
                throw new ArgumentException("Webhooks.Url is mandatory.", nameof(requestModel.Webhooks));

            string requestUri = $"{this.CopyleaksApiServer}{this.VideoDetectionApiVersion}/ai-video-detector/{PathSegment(scanId)}/submit";
            // A submit to a fixed scan id is safe to repeat: a duplicate is rejected with 409 Conflict,
            // and CopyleaksHttpException.Attempt tells the caller when that 409 answered a retry.
            await SendJsonAndEnsureSuccessAsync(
                HttpMethod.Post, requestUri, token, requestModel, gzip: false, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }
    }
}
