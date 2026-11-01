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
    /// <summary>AI Video Detection: submit a video URL; the result is delivered to your webhook.</summary>
    public class CopyleaksAiVideoDetectionApi : CopyleaksBase
    {
        public string CopyleaksApiServer { get; private set; }

        private string VideoDetectionApiVersion { get; set; }

        /// <summary>Uses the given HttpClient.</summary>
        /// <param name="client">For example one from IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings.</param>
        public CopyleaksAiVideoDetectionApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>Uses the SDK's shared HttpClient, with an optional client certificate and options.</summary>
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
        /// Submit a video URL for AI-generated content detection. Returns once accepted (201); the result is posted to
        /// <see cref="CopyleaksAiVideoDetectionRequestModel.Webhooks"/> as a <c>CopyleaksAiVideoDetectionResponseModel</c>.
        /// </summary>
        /// <param name="scanId">Unique scan id, 3 to 36 characters.</param>
        /// <param name="requestModel">Video URL, filename, model, webhook and options.</param>
        /// <param name="token">Login token.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <exception cref="CopyleaksHttpException">The server rejected the request.</exception>
        public async Task SubmitAsync(string scanId, CopyleaksAiVideoDetectionRequestModel requestModel, string token, CancellationToken cancellationToken = default)
        {
            RequireScanId(scanId, nullException: true);
            RequireToken(token, nullException: true);
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
