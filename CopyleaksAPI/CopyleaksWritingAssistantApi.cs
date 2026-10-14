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

        /// <summary>
        /// Conection to Copyleaks Grammar Checker (writing assistant) API
        /// </summary>
        /// <param name="client">The HttpClient to use, for example one created by IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksWritingAssistantApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>
        /// Conection to Copyleaks Grammar Checker (writing assistant) API, using the shared HttpClient of the SDK.
        /// </summary>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksWritingAssistantApi(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null) : base(clientCertificate, options)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ResolveApiEndpoint();
            this.WritingAssistantApiVersion = ConfigurationManager.Configuration["writingAssistantApiVersion"];
        }

        /// <summary>
        /// Use Copyleaks Writing Assistant to generate grammar, spelling and sentence corrections for a given text.
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The submission document including the text to scan</param>
        /// <param name="token">A copyleaks access token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task<WritingAssistantResult> SubmitTextAsync(string scanId, WritingAssistantDocument documentModel, string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));
            if (documentModel == null)
                throw new ArgumentNullException(nameof(documentModel));
            if (string.IsNullOrEmpty(documentModel.Text))
                throw new ArgumentException("Text is mandatory.", nameof(documentModel.Text));

            string requestUri = $"{this.CopyleaksApiServer}{this.WritingAssistantApiVersion}/writing-feedback/{PathSegment(scanId)}/check";
            // Charged per call, so only a rate-limited (429) attempt is retried.
            return await SendJsonForJsonAsync<WritingAssistantResult>(
                HttpMethod.Post, requestUri, token, documentModel, gzip: true, eRetryMode.RateLimitOnly, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a list of correction types supported within the Writing Assistant API. Correction types apply to all supported languages. The supplied language code for this request is used to determine the language of the texts returned.
        /// This endpoint is public, so no login is needed.
        /// </summary>
        /// <param name="languageCode">The language for the returned texts to be in. Language codes are in ISO 639-1 standard. Supported Values: en - English</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CopyleaksHttpException"></exception>
        public Task<CorrectionTypes> GetCorrectionTypesAsync(string languageCode, CancellationToken cancellationToken = default)
        {
            return GetCorrectionTypesAsync(null, languageCode, cancellationToken);
        }

        /// <summary>
        /// Get a list of correction types supported within the Writing Assistant API. Correction types apply to all supported languages. The supplied language code for this request is used to determine the language of the texts returned.
        /// </summary>
        /// <param name="token">A copyleaks access token. Optional: the endpoint is public, so null or empty sends no Authorization header.</param>
        /// <param name="languageCode">The language for the returned texts to be in. Language codes are in ISO 639-1 standard. Supported Values: en - English</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CopyleaksHttpException"></exception>
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
