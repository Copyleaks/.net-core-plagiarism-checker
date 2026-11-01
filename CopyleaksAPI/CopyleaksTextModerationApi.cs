using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Requests.TextModeration;
using Copyleaks.SDK.V3.API.Models.Responses.TextModeration;

namespace Copyleaks.SDK.V3.API
{
    public class CopyleaksTextModerationApi : CopyleaksBase
    {
        public string CopyleaksApiServer { get; private set; }

        private string TextModerationApiVersion { get; set; }

        /// <summary>Uses the given HttpClient.</summary>
        /// <param name="client">For example one from IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings.</param>
        public CopyleaksTextModerationApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>Uses the SDK's shared HttpClient, with an optional client certificate and options.</summary>
        public CopyleaksTextModerationApi(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null) : base(clientCertificate, options)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ResolveApiEndpoint();
            this.TextModerationApiVersion = ConfigurationManager.Configuration[CopyleaksConstants.TextModerationApiVersion];
        }

        /// <summary>Check text for content that should be moderated.</summary>
        /// <exception cref="ArgumentNullException">A required argument is missing.</exception>
        /// <exception cref="CopyleaksHttpException">The server rejected the request.</exception>
        public async Task<CopyleaksTextModerationResponseModel> SubmitTextAsync(string scanId, CopyleaksTextModerationRequestModel textModerationRequestModel, string token, CancellationToken cancellationToken = default)
        {
            #region Input validation
            RequireScanId(scanId, nullException: true);
            RequireToken(token, nullException: true);

            if (textModerationRequestModel == null)
                throw new ArgumentNullException(nameof(textModerationRequestModel));

            if (string.IsNullOrEmpty(textModerationRequestModel.Text))
                throw new ArgumentNullException(nameof(textModerationRequestModel.Text), "Text is mandatory.");
            #endregion

            string requestUri = $"{this.CopyleaksApiServer}{this.TextModerationApiVersion}/text-moderation/{PathSegment(scanId)}/check";

            // Charged per call, so only a rate-limited (429) attempt is retried.
            return await SendJsonForJsonAsync<CopyleaksTextModerationResponseModel>(
                HttpMethod.Post, requestUri, token, textModerationRequestModel, gzip: false, eRetryMode.RateLimitOnly, cancellationToken).ConfigureAwait(false);
        }
    }
}
