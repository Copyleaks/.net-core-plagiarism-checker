using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Requests.WritingAssistant;
using Copyleaks.SDK.V3.API.Models.Responses.WritingFeedback;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API
{
    public class CopyleaksWritingAssistantApi : CopyleaksBase
    {

        public string CopyleaksApiServer { get; private set; }

        private string WritingAssistantApiVersion { get; set; }

        /// <summary>Grammar Checker (writing assistant) client that uses the given HttpClient.</summary>
        /// <param name="client">For example one from IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings.</param>
        public CopyleaksWritingAssistantApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>Uses the SDK's shared HttpClient, with an optional client certificate and options.</summary>
        public CopyleaksWritingAssistantApi(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null) : base(clientCertificate, options)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ResolveApiEndpoint();
            this.WritingAssistantApiVersion = ConfigurationManager.Configuration["writingAssistantApiVersion"];
        }

        /// <summary>Get grammar, spelling and sentence corrections for a text.</summary>
        /// <exception cref="ArgumentException">A required argument is missing.</exception>
        /// <exception cref="CopyleaksHttpException">The server rejected the request.</exception>
        public async Task<WritingAssistantResult> SubmitTextAsync(string scanId, WritingAssistantDocument documentModel, string token, CancellationToken cancellationToken = default)
        {
            RequireScanId(scanId);
            RequireToken(token);
            if (documentModel == null)
                throw new ArgumentNullException(nameof(documentModel));
            if (string.IsNullOrEmpty(documentModel.Text))
                throw new ArgumentException("Text is mandatory.", nameof(documentModel.Text));

            string requestUri = $"{this.CopyleaksApiServer}{this.WritingAssistantApiVersion}/writing-feedback/{PathSegment(scanId)}/check";
            // Charged per call, so only a rate-limited (429) attempt is retried.
            return await SendJsonForJsonAsync<WritingAssistantResult>(
                HttpMethod.Post, requestUri, token, documentModel, gzip: true, eRetryMode.RateLimitOnly, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Get the correction types (the same for all languages). No login needed.</summary>
        /// <param name="languageCode">ISO 639-1 language of the returned texts. Supported: "en".</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <exception cref="ArgumentException">The language code is missing.</exception>
        /// <exception cref="CopyleaksHttpException">The server rejected the request.</exception>
        public Task<CorrectionTypes> GetCorrectionTypesAsync(string languageCode, CancellationToken cancellationToken = default)
        {
            return GetCorrectionTypesAsync(null, languageCode, cancellationToken);
        }

        /// <summary>Get the correction types (the same for all languages).</summary>
        /// <param name="token">Optional login token; null or empty sends no Authorization header.</param>
        /// <param name="languageCode">ISO 639-1 language of the returned texts. Supported: "en".</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        /// <exception cref="ArgumentException">The language code is missing.</exception>
        /// <exception cref="CopyleaksHttpException">The server rejected the request.</exception>
        public async Task<CorrectionTypes> GetCorrectionTypesAsync(string token, string languageCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(languageCode))
                throw new ArgumentException("Language Code is mandatory", nameof(languageCode));

            string requestUri = $"{this.CopyleaksApiServer}{this.WritingAssistantApiVersion}/writing-feedback/correction-types/{PathSegment(languageCode)}";
            return await SendForJsonAsync<CorrectionTypes>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }
    }
}
