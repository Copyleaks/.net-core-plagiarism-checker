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
using Copyleaks.SDK.V3.API.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
namespace Copyleaks.SDK.V3.API
{
    public class CopyleaksIdentityApi: CopyleaksBase
    {
		/// <summary>
        /// This class alows you the get the API token from Copyleaks API and manage your scans permissions
        /// </summary>
		public string CopyleaksIdServer { get; private set; }

        /// <summary>
        /// Connection to Copyleaks identity API
        /// </summary>
        /// <param name="clientCertificate">Optional: client certificate to be checked against in Copyleaks API.
        /// Configure you client's certificate at https://copyleaks.com/Manage </param>
        public CopyleaksIdentityApi(X509Certificate2 clientCertificate = null): base(clientCertificate)
        {
            SetServerEndpoint();
        }

        /// <summary>
        /// Connection to Copyleaks identity API
        /// </summary>
        /// <param name="client">Override the underlying http client with custom settings</param>
        public CopyleaksIdentityApi(HttpClient client) : base(client)
        {
            SetServerEndpoint();
        }

        private void SetServerEndpoint()
        {
            this.CopyleaksIdServer = ConfigurationManager.Configuration["idEndPoint"];
        }

        /// <summary>
        /// Login to Copyleaks API
        /// </summary>
        /// <param name="email">Your email address</param>
        /// <param name="key">Copyleaks API key</param>
        /// <returns>LoginToken.Token that can be used in order to invoke copyleaks api methods</returns>
        public async Task<LoginResponse> LoginAsync(string email, string key)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email is mandatory.", nameof(email));
            else if (string.IsNullOrEmpty(key))
                throw new ArgumentException("ApiKey is mandatory.", nameof(key));

            string requestUri = $"{this.CopyleaksIdServer}{this.ApiVersion}/account/login/api";
            var response = await Client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(new
            {
                email,
                key
            }), Encoding.UTF8, "application/json"));
            return await response.ExtractJsonResultsAsync<LoginResponse>();
        }

    }
}
