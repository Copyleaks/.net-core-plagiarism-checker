using System;
using System.Globalization;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Constants;

namespace Copyleaks.SDK.V3.API
{
    /// <summary>
    /// Timeout and retry settings for any client constructor, for example
    /// <c>new CopyleaksScansApi(httpClient, new CopyleaksClientOptions { MaxRetries = 5 })</c>.
    /// </summary>
    public class CopyleaksClientOptions
    {
        private const int DefaultTimeoutMilliseconds = 60000;

        /// <summary>
        /// Time limit for one attempt (send the request, read the response); then a <see cref="TimeoutException"/> is
        /// thrown, after any retries. Default: <c>ConfigurationManager.Configuration["RequestsTimeout"]</c> (ms), 60 s.
        /// <see cref="System.Threading.Timeout.InfiniteTimeSpan"/> turns it off. The timeout of a passed-in
        /// <see cref="System.Net.Http.HttpClient"/> also applies. The PDF report and usage history downloads are streamed:
        /// this timeout (not the HttpClient one) covers reading their body, and a failure while reading it is not retried.
        /// </summary>
        public TimeSpan Timeout { get; set; } = ReadConfiguredTimeout();

        /// <summary>
        /// Retries of a failed call; default 3, 0 turns retries off. Calls safe to repeat (GETs, login, start, delete,
        /// and submits to a fixed scan id: plagiarism file/URL/OCR, AI image URL, AI video) are retried on network errors,
        /// timeouts and HTTP 408, 429, 500, 502, 503, 504. Charged or action calls (AI text detection, Grammar Checker,
        /// Text Moderation, AI image check, Image Plagiarism Detection, export, resend webhook) only on 429, which the
        /// server returns without running them. A timed-out submit may still have been accepted, so its retry can get
        /// 409 Conflict: see <see cref="Exceptions.CopyleaksHttpException.Attempt"/>.
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Base delay of the exponential backoff; default 1 s. It doubles on every retry, with jitter,
        /// up to <see cref="MaxRetryDelay"/>.
        /// </summary>
        public TimeSpan RetryBaseDelay { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Longest wait before a retry; default 30 s. A <c>Retry-After</c> header sets the wait instead; if it asks for
        /// longer than this value, the SDK returns the error without retrying.
        /// </summary>
        public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// API server, for example "https://api.copyleaks.com/".
        /// Null = <c>ConfigurationManager.Configuration["apiEndPoint"]</c>, read when the client is created.
        /// </summary>
        public string ApiEndpoint { get; set; }

        /// <summary>
        /// Identity (login) server, for example "https://id.copyleaks.com/".
        /// Null = <c>ConfigurationManager.Configuration["idEndPoint"]</c>, read when the client is created.
        /// </summary>
        public string IdentityEndpoint { get; set; }

        private static TimeSpan ReadConfiguredTimeout()
        {
            string configured = null;
            try
            {
                configured = ConfigurationManager.Configuration?[CopyleaksConstants.RequestsTimeout];
            }
            catch (Exception)
            {
                // Fall back to the default below.
            }

            if (int.TryParse(configured, NumberStyles.Integer, CultureInfo.InvariantCulture, out int milliseconds))
            {
                if (milliseconds == -1)
                    return System.Threading.Timeout.InfiniteTimeSpan;
                if (milliseconds > 0)
                    return TimeSpan.FromMilliseconds(milliseconds);
            }
            return TimeSpan.FromMilliseconds(DefaultTimeoutMilliseconds);
        }
    }
}
