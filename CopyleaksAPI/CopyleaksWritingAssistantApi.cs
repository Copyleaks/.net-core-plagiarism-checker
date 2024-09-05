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
using Copyleaks.SDK.V3.API.Models.Requests.WritingAssistant;
using Copyleaks.SDK.V3.API.Models.Responses.WritingFeedback;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API
{
    public class CopyleaksWritingAssistantApi : CopyleaksBase
    {

        public string CopyleaksApiServer { get; private set; }

        private string WritingAssistantApiVersion { get; set; }


        public CopyleaksWritingAssistantApi(HttpClient client) : base(client)
        {
            SetUpService();
        }

        public CopyleaksWritingAssistantApi(X509Certificate2 clientCertificate = null) : base(clientCertificate)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ConfigurationManager.Configuration["ApiEndPoint"];
            this.WritingAssistantApiVersion = ConfigurationManager.Configuration["writingAssistantApiVersion"];
        }

        /// <summary>
        /// Use Copyleaks Writing Assistant to generate grammar, spelling and sentence corrections for a given text.
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The submission document including the text to scan</param>
        /// <param name="token">A copyleaks access token</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task<WritingAssistantResult> SubmitTextAsync(string scanId, WritingAssistantDocument documentModel, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));

            if (string.IsNullOrEmpty(documentModel.Text))
                throw new ArgumentException("Text is mandatory.", nameof(documentModel.Text));

            var method = new HttpMethod("POST");
            string requestUri = $"{this.CopyleaksApiServer}{this.WritingAssistantApiVersion}/writing-feedback/{scanId}/check";
            HttpRequestMessage msg = new HttpRequestMessage(method, requestUri);
            msg.SetupHeaders(token);
            var serializedDocument = JsonConvert.SerializeObject(documentModel);
            msg.Content = new StringContent(serializedDocument, Encoding.UTF8, "application/json");

            using (var stream = new MemoryStream())
            using (var streamContent = new StreamContent(stream))
            using (var response = await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new CopyleaksHttpException(response);
                }
                return await response.ExtractJsonResultsAsync<WritingAssistantResult>().ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Get a list of correction types supported within the Writing Assistant API. Correction types apply to all supported languages. The supplied language code for this request is used to determine the language of the texts returned.
        /// </summary>
        /// <param name="token"> a copyleaks access token</param>
        /// <param name="languageCode">The language for the returned texts to be in. Language codes are in ISO 639-1 standard. Supported Values: en - English</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task<CorrectionTypes> GetCorrectionTypesAsync(string token, string languageCode)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));

            if (string.IsNullOrEmpty(languageCode))
                throw new ArgumentException("Language Code is mandatory", nameof(languageCode));

            string requestUri = $"{this.CopyleaksApiServer}{this.WritingAssistantApiVersion}/writing-feedback/correction-types/{languageCode}";
            var msg = new HttpRequestMessage(HttpMethod.Get, requestUri);
            msg.SetupHeaders(token);

            using (var stream = new MemoryStream())
            using (var streamContent = new StreamContent(stream))
            using (var response = await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new CopyleaksHttpException(response);
                }
                return await response.ExtractJsonResultsAsync<CorrectionTypes>().ConfigureAwait(false);
            }
        }
    }
}
