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
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection;
using Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API
{
    public class CopyleaksAiImageDetectionApi : CopyleaksBase
    {
        public string CopyleaksApiServer { get; private set; }

        private string ImageDetectionApiVersion { get; set; }

        public CopyleaksAiImageDetectionApi(HttpClient client) : base(client)
        {
            SetUpService();
        }

        public CopyleaksAiImageDetectionApi(X509Certificate2 clientCertificate = null) : base(clientCertificate)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ConfigurationManager.Configuration[CopyleaksConstants.ApiEndPoint];
            this.ImageDetectionApiVersion = ConfigurationManager.Configuration[CopyleaksConstants.ImageDetectionApiVersion];
        }

        public async Task<CopyleaksAiImageDetectionResponseModel> SubmitAsync(string scanId, CopyleaksAiImageDetectionRequestModel requestModel, string token)
        {
            #region Input validation
            if (string.IsNullOrEmpty(scanId))
                throw new ArgumentNullException("ScanId is mandatory", nameof(scanId));

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("Token is mandatory", nameof(token));
            #endregion

            string requestUri = $"{this.CopyleaksApiServer}{this.ImageDetectionApiVersion}/ai-image-detector/{scanId}/check";

            // Add requerst body and headers
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Content = new StringContent(JsonConvert.SerializeObject(requestModel), Encoding.UTF8, "application/json");
            request.SetupHeaders(token);

            using (var response = await Client.SendAsync(request).ConfigureAwait(false))
            {
                // if the response not success then thow CopyleaksHttpException
                if (!response.IsSuccessStatusCode)
                    throw new CopyleaksHttpException(response);

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<CopyleaksAiImageDetectionResponseModel>(json);
            }
            ;
        }
    }
}
