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
    /// An HTTP connection to Copyleaks API.
    /// <para>
    /// API objects are thread-safe and cheap to create. When you do not pass an <see cref="HttpClient"/>,
    /// every API object uses one shared, pooled HttpClient, so you can create API objects freely.
    /// In ASP.NET Core you can also register the API classes as typed clients with
    /// <c>services.AddHttpClient&lt;CopyleaksScansApi&gt;()</c>.
    /// </para>
    /// </summary>
    public abstract class CopyleaksBase : IDisposable
    {
        /// <summary>
        /// The HttpClient used to send requests: the one passed to the constructor, or the shared client of the SDK.
        /// </summary>
        public HttpClient Client { get; private set; }

        /// <summary>
        /// The timeout and retry settings of this API object.
        /// </summary>
        public CopyleaksClientOptions Options { get; private set; }

        protected string ApiVersion { get; set; }

        /// <summary>
        /// A new HTTP connection to Copyleaks API that sends requests with your HttpClient.
        /// The SDK never disposes a client you pass in.
        /// </summary>
        /// <param name="client">The HttpClient to use, for example one created by IHttpClientFactory</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksBase(HttpClient client, CopyleaksClientOptions options = null)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
            Initialize(options);
        }

        /// <summary>
        /// A new HTTP connection to Copyleaks API that uses the shared HttpClient of the SDK.
        /// </summary>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.
        /// Configure you client's certificate at https://copyleaks.com/Manage </param>
        /// <param name="options">Optional timeout and retry settings</param>
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

        /// <summary>
        /// The API server address: <see cref="CopyleaksClientOptions.ApiEndpoint"/> when set, otherwise the configuration.
        /// </summary>
        private protected string ResolveApiEndpoint()
        {
            return WithTrailingSlash(this.Options.ApiEndpoint ?? ConfigurationManager.Configuration[CopyleaksConstants.ApiEndPoint]);
        }

        /// <summary>
        /// The identity server address: <see cref="CopyleaksClientOptions.IdentityEndpoint"/> when set, otherwise the configuration.
        /// </summary>
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

        /// <summary>
        /// Escape a value that is used as a URL path segment, such as a scan id.
        /// </summary>
        private protected static string PathSegment(string value)
        {
            return Uri.EscapeDataString(value ?? string.Empty);
        }

        /// <summary>
        /// Send a request with retries, per-attempt timeout and cancellation, and buffer the response body.
        /// </summary>
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
        /// Send a request with retries, per-attempt timeout and cancellation.
        /// A new HttpRequestMessage (and a new content from <paramref name="createContent"/>) is built for every attempt.
        /// Returns the last response, whatever its status code, with the number of the attempt that got it.
        /// Throws the last network error or a TimeoutException when no attempt got a response.
        /// With <see cref="HttpCompletionOption.ResponseHeadersRead"/> the body is not buffered, and the returned response
        /// keeps the timeout of its attempt running, so reading the body with <see cref="ApiResponse.Token"/> is still bounded by it.
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

        /// <summary>
        /// Send a request and throw a <see cref="CopyleaksHttpException"/> when the response is not a success.
        /// </summary>
        private protected async Task SendAndEnsureSuccessAsync(
            HttpMethod method, string requestUri, string token, Func<HttpContent> createContent,
            eRetryMode retryMode, CancellationToken cancellationToken)
        {
            using (var response = await SendAsync(method, requestUri, token, createContent, retryMode, cancellationToken).ConfigureAwait(false))
            {
                await response.EnsureSuccessAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Send a request and deserialize the JSON response.
        /// </summary>
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
        /// Send a GET request and pass the body of a successful response to <paramref name="readBody"/> as it arrives,
        /// without buffering it first. The per-attempt timeout keeps running while the body is read.
        /// Retries cover the request until the response headers arrive. A failure while the body is read is not retried,
        /// because part of the body may already have been consumed.
        /// </summary>
        /// <param name="requestUri">The download address</param>
        /// <param name="token">Login Token</param>
        /// <param name="readBody">Reads the body stream; gets the Content-Length when the server sent one</param>
        /// <param name="cancellationToken">Cancels the request</param>
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
        /// Throw an <see cref="ArgumentException"/> for an empty scan id, before it is used in a URL path.
        /// </summary>
        private protected static void RequireScanId(string scanId)
        {
            if (string.IsNullOrEmpty(scanId))
                throw new ArgumentException("ScanId is mandatory", nameof(scanId));
        }

        /// <summary>
        /// Send <paramref name="body"/> as JSON (serialized once, reused by every attempt) and ensure success.
        /// </summary>
        private protected async Task SendJsonAndEnsureSuccessAsync(
            HttpMethod method, string requestUri, string token, object body, bool gzip,
            eRetryMode retryMode, CancellationToken cancellationToken)
        {
            using (var payload = BufferedBody.Json(body, gzip))
            {
                await SendAndEnsureSuccessAsync(method, requestUri, token, payload.CreateContent, retryMode, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Send <paramref name="body"/> as JSON (serialized once, reused by every attempt) and deserialize the JSON response.
        /// </summary>
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
        /// API objects do not own an HttpClient: the shared client of the SDK lives for the whole process,
        /// and a client you pass in stays yours. Disposing an API object is therefore optional and never disposes a client.
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
