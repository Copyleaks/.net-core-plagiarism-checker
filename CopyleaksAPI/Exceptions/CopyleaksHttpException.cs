using Copyleaks.SDK.V3.API.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Copyleaks.SDK.V3.API.Exceptions
{
    /// <summary>Thrown when Copyleaks API returns an error response.</summary>
    public class CopyleaksHttpException : ApplicationException
    {
        /// <summary>HTTP status code.</summary>
        public HttpStatusCode HttpErrorCode { get; private set; }

        /// <summary>Raw response body; also the exception message.</summary>
        public string ResponseBody { get; private set; }

        /// <summary>error.type of the error response, for example "invalid_request_error"; null when not sent.</summary>
        public string ErrorType { get; private set; }

        /// <summary>error.id, a machine-readable identifier such as "insufficient_credits"; null when not sent.</summary>
        public string ErrorId { get; private set; }

        /// <summary>Numeric error.code; null when not sent.</summary>
        public int? ErrorCode { get; private set; }

        /// <summary>How long the server asked to wait before trying again (Retry-After header), when sent.</summary>
        public TimeSpan? RetryAfter { get; private set; }

        /// <summary>
        /// The 1-based attempt that got this error; greater than 1 when the SDK retried. With a new scan id for every
        /// submit, a 409 Conflict with Attempt greater than 1 means an earlier attempt was accepted but its response
        /// was lost (for example to a timeout): treat the call as successful.
        /// </summary>
        public int Attempt { get; private set; } = 1;

        /// <summary>
        /// Blocks while reading the response body; use <see cref="CreateAsync(HttpResponseMessage, CancellationToken)"/>.
        /// </summary>
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

        /// <summary>Create the exception for an error response, reading the body without blocking.</summary>
        public static Task<CopyleaksHttpException> CreateAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            return CreateAsync(response, attempt: 1, cancellationToken);
        }

        /// <summary>Same, for the given 1-based attempt.</summary>
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
