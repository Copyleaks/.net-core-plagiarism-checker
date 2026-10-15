using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Models.Constants;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>
    /// Base of the API clients. They are thread-safe and cheap to create: without an <see cref="HttpClient"/> argument
    /// they all share one pooled HttpClient. In ASP.NET Core they can also be typed clients:
    /// <c>services.AddHttpClient&lt;CopyleaksScansApi&gt;()</c>.
    /// </summary>
    public abstract class CopyleaksBase : IDisposable
    {
        /// <summary>The HttpClient passed to the constructor, or the SDK's shared one.</summary>
        public HttpClient Client { get; private set; }

        /// <summary>Timeout and retry settings.</summary>
        public CopyleaksClientOptions Options { get; private set; }

        protected string ApiVersion { get; set; }

        /// <summary>Uses your HttpClient (for example from IHttpClientFactory), which the SDK never disposes.</summary>
        public CopyleaksBase(HttpClient client, CopyleaksClientOptions options = null)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
            Initialize(options);
        }

        /// <summary>Uses the SDK's shared HttpClient.</summary>
        /// <param name="clientCertificate">Optional client certificate; configure it at https://copyleaks.com/Manage</param>
        /// <param name="options">Optional timeout and retry settings.</param>
        public CopyleaksBase(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null)
        {
            this.Client = SharedHttpClients.Get(clientCertificate);
            Initialize(options);
        }

        private void Initialize(CopyleaksClientOptions options)
        {
            this.Options = options ?? new CopyleaksClientOptions();
            this.ApiVersion = ConfigurationManager.Configuration["apiVersion"];
        }

        /// <summary>API server: <see cref="CopyleaksClientOptions.ApiEndpoint"/>, else the configuration.</summary>
        private protected string ResolveApiEndpoint()
        {
            return WithTrailingSlash(this.Options.ApiEndpoint ?? ConfigurationManager.Configuration[CopyleaksConstants.ApiEndPoint]);
        }

        /// <summary>Identity server: <see cref="CopyleaksClientOptions.IdentityEndpoint"/>, else the configuration.</summary>
        private protected string ResolveIdentityEndpoint()
        {
            return WithTrailingSlash(this.Options.IdentityEndpoint ?? ConfigurationManager.Configuration[CopyleaksConstants.IdEndPoint]);
        }

        private static string WithTrailingSlash(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint) || endpoint.EndsWith("/", StringComparison.Ordinal))
                return endpoint;
            return endpoint + "/";
        }

        /// <summary>Escape a value that is used as a URL path segment, such as a scan id.</summary>
        private protected static string PathSegment(string value)
        {
            return Uri.EscapeDataString(value ?? string.Empty);
        }

        /// <summary>Send with retries, per-attempt timeout and cancellation; buffers the response body.</summary>
        private protected Task<ApiResponse> SendAsync(
            HttpMethod method,
            string requestUri,
            string token,
            Func<HttpContent> createContent,
            eRetryMode retryMode,
            CancellationToken cancellationToken)
        {
            return SendAsync(method, requestUri, token, createContent, retryMode, HttpCompletionOption.ResponseContentRead, cancellationToken);
        }

        /// <summary>
        /// Send with retries, per-attempt timeout and cancellation; each attempt builds a new request and content.
        /// Returns the last response, whatever its status, with its attempt number; throws the last network error or a
        /// TimeoutException when no attempt got a response. With <see cref="HttpCompletionOption.ResponseHeadersRead"/>
        /// the body is not buffered and the attempt timeout keeps running, bounding reads with <see cref="ApiResponse.Token"/>.
        /// </summary>
        private protected async Task<ApiResponse> SendAsync(
            HttpMethod method,
            string requestUri,
            string token,
            Func<HttpContent> createContent,
            eRetryMode retryMode,
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken)
        {
            CopyleaksClientOptions options = this.Options;
            int maxRetries = retryMode == eRetryMode.None ? 0 : Math.Max(0, options.MaxRetries);
            TimeSpan timeout = options.Timeout;
            bool hasTimeout = HasTimeout(timeout);

            for (int attempt = 1; ; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                HttpResponseMessage response = null;
                ExceptionDispatchInfo failure = null;
                var attemptCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                try
                {
                    using (var request = new HttpRequestMessage(method, requestUri))
                    {
                        request.Headers.TryAddWithoutValidation("User-Agent", AssemblyHelper.UserAgent);
                        if (!string.IsNullOrEmpty(token))
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        if (createContent != null)
                            request.Content = createContent();

                        if (hasTimeout)
                            attemptCts.CancelAfter(timeout);

                        try
                        {
                            response = await Client.SendAsync(request, completionOption, attemptCts.Token).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
                        {
                            // Our per-attempt timeout or the HttpClient's own timeout, not the caller.
                            failure = ExceptionDispatchInfo.Capture(CreateTimeoutException(method, requestUri, timeout, ex));
                        }
                        catch (HttpRequestException ex)
                        {
                            failure = ExceptionDispatchInfo.Capture(ex);
                        }
                    }

                    if (attempt <= maxRetries && TryGetRetryDelay(retryMode, attempt - 1, response, failure != null, options, out TimeSpan delay))
                    {
                        response?.Dispose();
                        attemptCts.Dispose();
                        attemptCts = null;
                        if (delay > TimeSpan.Zero)
                            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    failure?.Throw();

                    if (completionOption != HttpCompletionOption.ResponseHeadersRead)
                        return new ApiResponse(response, attempt, cancellationToken);

                    // The body is still on the network: hand the attempt timeout over to the response.
                    var streamed = new ApiResponse(response, attempt, attemptCts.Token, attemptCts);
                    attemptCts = null;
                    return streamed;
                }
                finally
                {
                    attemptCts?.Dispose();
                }
            }
        }

        private static bool HasTimeout(TimeSpan timeout)
        {
            return timeout > TimeSpan.Zero && timeout != Timeout.InfiniteTimeSpan;
        }

        private static TimeoutException CreateTimeoutException(HttpMethod method, string requestUri, TimeSpan timeout, Exception inner)
        {
            string limit = HasTimeout(timeout) ? $" within {timeout.TotalSeconds:0.###} seconds" : string.Empty;
            return new TimeoutException($"The request {method} {new Uri(requestUri).AbsolutePath} did not complete{limit}.", inner);
        }

        private static bool TryGetRetryDelay(eRetryMode mode, int attempt, HttpResponseMessage response, bool failed,
                                             CopyleaksClientOptions options, out TimeSpan delay)
        {
            delay = TimeSpan.Zero;
            if (failed)
            {
                if (mode != eRetryMode.Safe)
                    return false;
                delay = RetryHelper.Backoff(attempt, options.RetryBaseDelay, options.MaxRetryDelay);
                return true;
            }

            if (response == null || !RetryHelper.IsRetryableStatus((int)response.StatusCode, mode))
                return false;

            TimeSpan? retryAfter = RetryHelper.GetRetryAfter(response);
            if (retryAfter.HasValue)
            {
                // Never retry sooner than the server asked. When it asks for longer than we are willing to wait, give up.
                if (retryAfter.Value > options.MaxRetryDelay)
                    return false;
                delay = retryAfter.Value;
                return true;
            }

            delay = RetryHelper.Backoff(attempt, options.RetryBaseDelay, options.MaxRetryDelay);
            return true;
        }

        /// <summary>Send a request; throw <see cref="CopyleaksHttpException"/> on a non-success response.</summary>
        private protected async Task SendAndEnsureSuccessAsync(
            HttpMethod method, string requestUri, string token, Func<HttpContent> createContent,
            eRetryMode retryMode, CancellationToken cancellationToken)
        {
            using (var response = await SendAsync(method, requestUri, token, createContent, retryMode, cancellationToken).ConfigureAwait(false))
            {
                await response.EnsureSuccessAsync().ConfigureAwait(false);
            }
        }

        /// <summary>Send a request and deserialize the JSON response.</summary>
        private protected async Task<T> SendForJsonAsync<T>(
            HttpMethod method, string requestUri, string token, Func<HttpContent> createContent,
            eRetryMode retryMode, CancellationToken cancellationToken)
        {
            using (var response = await SendAsync(method, requestUri, token, createContent, retryMode, cancellationToken).ConfigureAwait(false))
            {
                return await response.ReadJsonAsync<T>().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// GET, then pass a successful body to <paramref name="readBody"/> as it arrives, with its Content-Length if sent.
        /// The attempt timeout keeps running while it is read. Retries stop at the response headers: a failure while
        /// reading the body is not retried, because part of it may already have been consumed.
        /// </summary>
        private protected async Task DownloadAsync(
            string requestUri, string token, Func<Stream, long?, CancellationToken, Task> readBody, CancellationToken cancellationToken)
        {
            using (var response = await SendAsync(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    await response.EnsureSuccessAsync().ConfigureAwait(false);
                    HttpContent content = response.Message.Content;
                    using (Stream body = await content.ReadAsStreamAsync(response.Token).ConfigureAwait(false))
                    {
                        await readBody(body, content.Headers.ContentLength, response.Token).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
                {
                    // The attempt timeout ran out while the body was read.
                    throw CreateTimeoutException(HttpMethod.Get, requestUri, this.Options.Timeout, ex);
                }
            }
        }

        /// <summary>
        /// Throw for an empty scan id, before it goes into a URL path: <see cref="ArgumentException"/>, or
        /// <see cref="ArgumentNullException"/> when <paramref name="nullException"/>, for the clients that throw that.
        /// </summary>
        private protected static void RequireScanId(string scanId, bool nullException = false)
        {
            if (string.IsNullOrEmpty(scanId))
                throw Missing(nameof(scanId), "ScanId is mandatory", nullException);
        }

        /// <summary>Throw for an empty login token, like <see cref="RequireScanId"/>.</summary>
        private protected static void RequireToken(string token, bool nullException = false)
        {
            if (string.IsNullOrEmpty(token))
                throw Missing(nameof(token), "Token is mandatory", nullException);
        }

        private static ArgumentException Missing(string paramName, string message, bool nullException)
        {
            return nullException ? new ArgumentNullException(paramName, message) : new ArgumentException(message, paramName);
        }

        /// <summary>Send <paramref name="body"/> as JSON (serialized once for all attempts) and ensure success.</summary>
        private protected async Task SendJsonAndEnsureSuccessAsync(
            HttpMethod method, string requestUri, string token, object body, bool gzip,
            eRetryMode retryMode, CancellationToken cancellationToken)
        {
            using (var payload = BufferedBody.Json(body, gzip))
            {
                await SendAndEnsureSuccessAsync(method, requestUri, token, payload.CreateContent, retryMode, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>Like <see cref="SendJsonAndEnsureSuccessAsync"/>, but deserializes the JSON response.</summary>
        private protected async Task<T> SendJsonForJsonAsync<T>(
            HttpMethod method, string requestUri, string token, object body, bool gzip,
            eRetryMode retryMode, CancellationToken cancellationToken)
        {
            using (var payload = BufferedBody.Json(body, gzip))
            {
                return await SendForJsonAsync<T>(method, requestUri, token, payload.CreateContent, retryMode, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Optional, and never disposes an HttpClient: the SDK's shared client lives for the whole process,
        /// and one you pass in stays yours.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
