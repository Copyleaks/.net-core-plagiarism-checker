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
using Copyleaks.SDK.V3.API.Extensions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Requests.AIDetection;
using Copyleaks.SDK.V3.API.Models.Responses.AIDetector;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API
{
    public class CopyleaksAIDetectionApi : CopyleaksBase
    {

        public string CopyleaksApiServer { get; private set; }

        private string AIDetectionApiVersion { get; set; }


        /// <summary>
        /// Conection to Copyleaks scan API
        /// </summary>
        /// <param name="client">Override the underlying http client with custom settings</param>
        public CopyleaksAIDetectionApi(HttpClient client) : base(client)
        {
            SetUpService();
        }

        /// <summary>
        /// Conection to Copyleaks scan API
        /// </summary>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.
        public CopyleaksAIDetectionApi(X509Certificate2 clientCertificate = null) : base(clientCertificate)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ConfigurationManager.Configuration["ApiEndPoint"];
            AIDetectionApiVersion = ConfigurationManager.Configuration["aiDetectionApiVersion"];
        }

        /// <summary>
        /// Use Copyleaks AI Content Detection to differentiate between human texts and AI written texts.
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The submission document including the text to scan</param>
        /// <param name="token">A copyleaks access token</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task<AIDetectionResult> SubmitNaturalLanguageAsync(string scanId, NaturalLanguageDocument documentModel, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));

            if (string.IsNullOrEmpty(documentModel.Text))
                throw new ArgumentException("Text is mandatory.", nameof(documentModel.Text));

            var method = new HttpMethod("POST");
            string requestUri = $"{this.CopyleaksApiServer}{this.AIDetectionApiVersion}/writer-detector/{scanId}/check";
            HttpRequestMessage msg = new HttpRequestMessage(method, requestUri);
            msg.SetupHeaders(token);
            msg.Content = new StringContent(JsonConvert.SerializeObject(documentModel), Encoding.UTF8, "application/json");

            using (var stream = new MemoryStream())
            using (var streamContent = new StreamContent(stream))
            using (var response = await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new CopyleaksHttpException(response);
                }
                return await response.ExtractJsonResultsAsync<AIDetectionResult>().ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Use Copyleaks AI Content Detection to differentiate between human source code and AI written source code.
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The submission document including the text to scan</param>
        /// <param name="token">A copyleaks access token</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task<AIDetectionResult> SubmitSourceCodeAsync(string scanId, SourceCodeDocument documentModel, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));

            if (string.IsNullOrEmpty(documentModel.Text))
                throw new ArgumentException("Text is mandatory.", nameof(documentModel.Text));

            if (string.IsNullOrEmpty(documentModel.Filename))
                throw new ArgumentException("Filename is mandatory.", nameof(documentModel.Filename));

            var method = new HttpMethod("POST");
            string requestUri = $"{this.CopyleaksApiServer}{this.AIDetectionApiVersion}/writer-detector/source-code/{scanId}/check";
            HttpRequestMessage msg = new HttpRequestMessage(method, requestUri);
            msg.SetupHeaders(token);
            msg.Content = new StringContent(JsonConvert.SerializeObject(documentModel), Encoding.UTF8, "application/json");

            using (var stream = new MemoryStream())
            using (var streamContent = new StreamContent(stream))
            using (var response = await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new CopyleaksHttpException(response);
                }
                return await response.ExtractJsonResultsAsync<AIDetectionResult>().ConfigureAwait(false);
            }
        }
    }
}
