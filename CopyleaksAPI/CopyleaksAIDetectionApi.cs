/********************************************************************************
 The MIT License(MIT)
 
 Copyright(c) 2016 Copyleaks LTD (https://copyleaks.com)
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
********************************************************************************/

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

        /// <summary>
        /// Conection to Copyleaks AI Text Detection API
        /// </summary>
        /// <param name="client">The HttpClient to use, for example one created by IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksAIDetectionApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>
        /// Conection to Copyleaks AI Text Detection API, using the shared HttpClient of the SDK.
        /// </summary>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksAIDetectionApi(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null) : base(clientCertificate, options)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ResolveApiEndpoint();
            AIDetectionApiVersion = ConfigurationManager.Configuration["aiDetectionApiVersion"];
        }

        /// <summary>
        /// Use Copyleaks AI Content Detection to differentiate between human texts and AI written texts.
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The submission document including the text to scan</param>
        /// <param name="token">A copyleaks access token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CopyleaksHttpException"></exception>
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

        /// <summary>
        /// Get the credit balance of the AI Text Detection wallet.
        /// </summary>
        /// <param name="token">A copyleaks access token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>The number of available credits</returns>
        public async Task<uint> CreditBalanceAsync(string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));

            string requestUri = $"{this.CopyleaksApiServer}{this.AIDetectionApiVersion}/writer-detector/credits";
            var response = await SendForJsonAsync<AIDetectionCreditsResponse>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
            return response.Credits;
        }

        /// <summary>
        /// Get the file types supported by AI text detection in scans. No login is needed.
        /// </summary>
        /// <param name="cancellationToken">Cancels the request</param>
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
