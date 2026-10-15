using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Responses;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API
{
    /// <summary>Gets the login token that the other API clients need.</summary>
    public class CopyleaksIdentityApi : CopyleaksBase
    {
        /// <summary>Identity server address.</summary>
        public string CopyleaksIdServer { get; private set; }

        /// <summary>Uses the SDK's shared HttpClient.</summary>
        /// <param name="clientCertificate">Optional client certificate; configure it at https://copyleaks.com/Manage</param>
        /// <param name="options">Optional timeout and retry settings.</param>
        public CopyleaksIdentityApi(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null) : base(clientCertificate, options)
        {
            SetServerEndpoint();
        }

        /// <summary>Uses the given HttpClient.</summary>
        /// <param name="client">For example one from IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings.</param>
        public CopyleaksIdentityApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetServerEndpoint();
        }

        private void SetServerEndpoint()
        {
            this.CopyleaksIdServer = ResolveIdentityEndpoint();
        }

        /// <summary>Log in with your email and API key.</summary>
        /// <returns>The login token to pass to the other API calls.</returns>
        /// <exception cref="CopyleaksHttpException">The server rejected the request.</exception>
        public async Task<LoginResponse> LoginAsync(string email, string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email is mandatory.", nameof(email));
            else if (string.IsNullOrEmpty(key))
                throw new ArgumentException("ApiKey is mandatory.", nameof(key));

            string requestUri = $"{this.CopyleaksIdServer}{this.ApiVersion}/account/login/api";

            // Login has no side effects, so it is safe to retry.
            return await SendJsonForJsonAsync<LoginResponse>(
                HttpMethod.Post, requestUri, token: null, body: new { email, key }, gzip: true,
                eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }
    }
}
