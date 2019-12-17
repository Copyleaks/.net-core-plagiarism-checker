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

using Copyleaks.SDK.V3.API.Extensions;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>
    /// An HTTP connection to Copyleaks API
    /// </summary>
    public abstract class CopyleaksBase : IDisposable
    {
        public HttpClient Client { get; private set; }

        protected string ApiVersion { get; set; }

        /// <summary>
        /// A new dispoable HTTP Connection to Copyleaks API
        /// </summary>
        /// <param name="client">Override the underlying http client with custom settings</param>
        public CopyleaksBase(HttpClient client)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
            SetCopyleaksHeaders();
        }

        /// <summary>
        /// A new dispoable HTTP Connection to Copyleaks API
        /// </summary>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.
        /// Configure you client's certificate at https://copyleaks.com/Manage </param>
        public CopyleaksBase(X509Certificate2 clientCertificate = null)
        {
            if (clientCertificate != null)
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.SslProtocols = SslProtocols.Tls12;
                handler.ClientCertificates.Add(clientCertificate);
                handler.AutomaticDecompression = DecompressionMethods.GZip;                               
                this.Client = new HttpClient(handler);
            }
            else
                this.Client = new HttpClient();
            SetCopyleaksHeaders();

        }

        private void SetCopyleaksHeaders()
        {            
            this.ApiVersion = ConfigurationManager.Configuration["apiVersion"];
        }

        public void Dispose()
        {
            //this.Client.Dispose();
        }
    }
}
