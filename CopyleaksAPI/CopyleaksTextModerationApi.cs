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

        /// <summary>
        /// Conection to Copyleaks Text Moderation API
        /// </summary>
        /// <param name="client">The HttpClient to use, for example one created by IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksTextModerationApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>
        /// Conection to Copyleaks Text Moderation API, using the shared HttpClient of the SDK.
        /// </summary>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksTextModerationApi(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null) : base(clientCertificate, options)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ResolveApiEndpoint();
            this.TextModerationApiVersion = ConfigurationManager.Configuration[CopyleaksConstants.TextModerationApiVersion];
        }

        /// <summary>
        /// This function for submitting text to the copyleaks text moderation client.
        /// it recieves a scanId a requestModel and token. creates a post request to copyleaks servers adn return the response as TextModerationResponseModel.
        /// </summary>
        /// <param name="scanId"></param>
        /// <param name="textModerationRequestModel"></param>
        /// <param name="token"></param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns> model of TextModerationResponseModel represents the response from copyleaks servers</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task<CopyleaksTextModerationResponseModel> SubmitTextAsync(string scanId, CopyleaksTextModerationRequestModel textModerationRequestModel, string token, CancellationToken cancellationToken = default)
        {
            #region Input validation
            if (string.IsNullOrEmpty(scanId))
                throw new ArgumentNullException(nameof(scanId), "ScanId is mandatory");

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token), "Token is mandatory");

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
