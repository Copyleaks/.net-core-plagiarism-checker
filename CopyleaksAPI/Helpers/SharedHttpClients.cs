using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>
    /// The HttpClients used when the caller passes none: one per client certificate, created once per process and
    /// never disposed, so every API object shares the same connection pool.
    /// </summary>
    internal static class SharedHttpClients
    {
        private static readonly Lazy<HttpClient> Default = new Lazy<HttpClient>(() => Create(null), LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly ConcurrentDictionary<string, Lazy<HttpClient>> WithCertificate =
            new ConcurrentDictionary<string, Lazy<HttpClient>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Get the shared client, or the shared client for a client certificate.</summary>
        public static HttpClient Get(X509Certificate2 clientCertificate)
        {
            if (clientCertificate == null)
                return Default.Value;

            return WithCertificate.GetOrAdd(
                clientCertificate.Thumbprint ?? string.Empty,
                _ => new Lazy<HttpClient>(() => Create(clientCertificate), LazyThreadSafetyMode.ExecutionAndPublication)).Value;
        }

        private static HttpClient Create(X509Certificate2 clientCertificate)
        {
            var handler = new SocketsHttpHandler
            {
                // Recycle pooled connections so DNS changes are picked up.
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                AutomaticDecompression = DecompressionMethods.All,
            };

            if (clientCertificate != null)
                handler.SslOptions.ClientCertificates = new X509CertificateCollection { clientCertificate };

            // The per-attempt timeout comes from CopyleaksClientOptions.Timeout.
            return new HttpClient(handler, disposeHandler: true) { Timeout = Timeout.InfiniteTimeSpan };
        }
    }
}
