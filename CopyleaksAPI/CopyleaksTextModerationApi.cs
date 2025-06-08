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
using System.Text;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Extensions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Requests.TextModeration;
using Copyleaks.SDK.V3.API.Models.Responses.TextModeration;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API
{
    public class CopyleaksTextModerationApi : CopyleaksBase
    {
        public string CopyleaksApiServer { get; private set; }

        private string TextModerationApiVersion { get; set; }

        public CopyleaksTextModerationApi(HttpClient client) : base(client)
        {
            SetUpService();
        }

        public CopyleaksTextModerationApi(X509Certificate2 clientCertificate = null) : base(clientCertificate)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ConfigurationManager.Configuration["ApiEndPoint"];
            this.TextModerationApiVersion = ConfigurationManager.Configuration["textModerationApiVersion"];
        }

        public async Task<TextModerationResponseModel> SubmitTextAsync(string scanId, TextModerationRequestModel textModerationRequestModel, string token)
        {
            #region Input validation
            if (string.IsNullOrEmpty(scanId))
                throw new ArgumentNullException("ScanId is mandatory", nameof(scanId));

            if (string.IsNullOrEmpty(scanId))
                throw new ArgumentNullException("Token is mandatory", nameof(token));

            if (string.IsNullOrEmpty(textModerationRequestModel.Text))
                throw new ArgumentNullException("Text is mandatory.", nameof(textModerationRequestModel.Text));
            #endregion

            var method = new HttpMethod("POST");
            string requestUri = $"{this.CopyleaksApiServer}{this.TextModerationApiVersion}/text-moderation/{scanId}/check";

            // Add requerst body and headers
            var request = new HttpRequestMessage(method, requestUri);
            request.Content = new StringContent(JsonConvert.SerializeObject(textModerationRequestModel), Encoding.UTF8, "application/json");
            request.SetupHeaders(token);

            using (var response = await Client.SendAsync(request).ConfigureAwait(false))
            {
                // if the response not success then thow CopyleaksHttpException
                if (!response.IsSuccessStatusCode)
                    throw new CopyleaksHttpException(response);

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<TextModerationResponseModel>(json);
            }
            ;
        }
    }
}
