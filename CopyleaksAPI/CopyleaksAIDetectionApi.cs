using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Requests.AIDetection;
using Copyleaks.SDK.V3.API.Models.Responses;
using Copyleaks.SDK.V3.API.Models.Responses.AIDetector;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API
{
    public class CopyleaksAIDetectionApi : CopyleaksBase
    {

        public string CopyleaksApiServer { get; private set; }

        private string AIDetectionApiVersion { get; set; }

        /// <summary>Uses the given HttpClient.</summary>
        /// <param name="client">For example one from IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings.</param>
        public CopyleaksAIDetectionApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>Uses the SDK's shared HttpClient, with an optional client certificate and options.</summary>
        public CopyleaksAIDetectionApi(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null) : base(clientCertificate, options)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ResolveApiEndpoint();
            AIDetectionApiVersion = ConfigurationManager.Configuration["aiDetectionApiVersion"];
        }

        /// <summary>Detect whether text was written by a human or by AI.</summary>
        /// <exception cref="ArgumentException">A required argument is missing.</exception>
        /// <exception cref="CopyleaksHttpException">The server rejected the request.</exception>
        public async Task<AIDetectionResult> SubmitNaturalLanguageAsync(string scanId, NaturalLanguageDocument documentModel, string token, CancellationToken cancellationToken = default)
        {
            RequireScanId(scanId);
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));
            if (documentModel == null)
                throw new ArgumentNullException(nameof(documentModel));
            if (string.IsNullOrEmpty(documentModel.Text))
                throw new ArgumentException("Text is mandatory.", nameof(documentModel.Text));

            string requestUri = $"{this.CopyleaksApiServer}{this.AIDetectionApiVersion}/writer-detector/{PathSegment(scanId)}/check";
            // Charged per call, so only a rate-limited (429) attempt is retried.
            return await SendJsonForJsonAsync<AIDetectionResult>(
                HttpMethod.Post, requestUri, token, documentModel, gzip: true, eRetryMode.RateLimitOnly, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Get the credit balance of the AI Text Detection wallet.</summary>
        public async Task<uint> CreditBalanceAsync(string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));

            string requestUri = $"{this.CopyleaksApiServer}{this.AIDetectionApiVersion}/writer-detector/credits";
            var response = await SendForJsonAsync<AIDetectionCreditsResponse>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
            return response.Credits;
        }

        /// <summary>Get the file types supported by AI text detection in scans. No login needed.</summary>
        public async Task<SupportedAiTextFileTypesResult> GetSupportedFileTypesAsync(CancellationToken cancellationToken = default)
        {
            string requestUri = $"{this.CopyleaksApiServer}v3/miscellaneous/supported-ai-text-detection-file-types";
            return await SendForJsonAsync<SupportedAiTextFileTypesResult>(
                HttpMethod.Get, requestUri, null, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        internal sealed class AIDetectionCreditsResponse
        {
            [JsonProperty("credits")]
            public uint Credits { get; set; }
        }
    }
}
