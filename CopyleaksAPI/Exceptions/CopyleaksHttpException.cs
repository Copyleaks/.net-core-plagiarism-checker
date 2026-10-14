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

using Copyleaks.SDK.V3.API.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Copyleaks.SDK.V3.API.Exceptions
{
    /// <summary>
    /// This class is returned in case an error response was received from Copyleaks API
    /// </summary>
    public class CopyleaksHttpException : ApplicationException
    {
        /// <summary>
        /// The Http status code
        /// </summary>
        public HttpStatusCode HttpErrorCode { get; private set; }

        /// <summary>
        /// The raw response body. It is also the exception message.
        /// </summary>
        public string ResponseBody { get; private set; }

        /// <summary>
        /// The error type from the error response (error.type), for example "invalid_request_error". Null when the server did not send one.
        /// </summary>
        public string ErrorType { get; private set; }

        /// <summary>
        /// The machine-readable error identifier from the error response (error.id), for example "insufficient_credits". Null when the server did not send one.
        /// </summary>
        public string ErrorId { get; private set; }

        /// <summary>
        /// The numeric error code from the error response (error.code). Null when the server did not send one.
        /// </summary>
        public int? ErrorCode { get; private set; }

        /// <summary>
        /// How long the server asked to wait before trying again (the Retry-After header), when it sent one.
        /// </summary>
        public TimeSpan? RetryAfter { get; private set; }

        /// <summary>
        /// The 1-based attempt that got this error response. It is greater than 1 when the SDK retried the call.
        /// A submit (or start) that fails with 409 Conflict and an Attempt greater than 1 means an earlier attempt
        /// of the same call was accepted but its response was lost, for example to a timeout: treat the call as successful.
        /// </summary>
        public int Attempt { get; private set; } = 1;

        /// <summary>
        /// A new error response from Copyleaks API. This constructor blocks while it reads the response body;
        /// use <see cref="CreateAsync(HttpResponseMessage, CancellationToken)"/> instead.
        /// </summary>
        /// <param name="response">The error response from Copyleaks API</param>
        [Obsolete("Use CreateAsync")]
        public CopyleaksHttpException(HttpResponseMessage response) :
            this(response, ReadBody(response), attempt: 1)
        {
        }

        private CopyleaksHttpException(HttpResponseMessage response, string body, int attempt) :
            base(GetMessage(response, body))
        {
            this.HttpErrorCode = response.StatusCode;
            this.ResponseBody = body;
            this.RetryAfter = RetryHelper.GetRetryAfter(response);
            this.Attempt = attempt;
            ParseErrorEnvelope(body);
        }

        /// <summary>
        /// Create the exception for an error response from Copyleaks API. The response body is read without blocking.
        /// </summary>
        /// <param name="response">The error response from Copyleaks API</param>
        /// <param name="cancellationToken">Cancels reading the response body</param>
        /// <returns>The exception, with the response body and the parsed error fields</returns>
        public static Task<CopyleaksHttpException> CreateAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            return CreateAsync(response, attempt: 1, cancellationToken);
        }

        /// <summary>
        /// Read the response body without blocking and create the exception for the given 1-based attempt.
        /// </summary>
        internal static async Task<CopyleaksHttpException> CreateAsync(HttpResponseMessage response, int attempt, CancellationToken cancellationToken)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            string body = null;
            try
            {
                if (response.Content != null)
                    body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException) || !cancellationToken.IsCancellationRequested)
            {
                body = null;
            }
            return new CopyleaksHttpException(response, body, attempt);
        }

        private static string ReadBody(HttpResponseMessage response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            try
            {
                // Blocking read, kept only for the obsolete public constructor. The SDK itself uses CreateAsync.
                return response.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetMessage(HttpResponseMessage response, string body)
        {
            if (!string.IsNullOrWhiteSpace(body))
                return body;
            if (response == null)
                return "The application has encountered an unknown error. Please try again later.";
            return $"The request failed with HTTP {(int)response.StatusCode} ({response.StatusCode}).";
        }

        private void ParseErrorEnvelope(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return;
            string trimmed = body.TrimStart();
            if (trimmed.Length == 0 || trimmed[0] != '{')
                return;
            try
            {
                JToken error = JObject.Parse(body)["error"];
                if (error is JObject envelope)
                {
                    ErrorType = envelope.Value<string>("type");
                    ErrorId = envelope.Value<string>("id");
                    JToken code = envelope["code"];
                    if (code != null && (code.Type == JTokenType.Integer || code.Type == JTokenType.String) &&
                        int.TryParse(code.ToString(), out int parsed))
                        ErrorCode = parsed;
                }
            }
            catch (Exception)
            {
                // Not a JSON error envelope. The raw body is still in ResponseBody and Message.
            }
        }
    }
}
