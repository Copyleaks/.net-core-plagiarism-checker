using System;
using System.Globalization;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Constants;

namespace Copyleaks.SDK.V3.API
{
    /// <summary>
    /// Timeout and retry settings for the Copyleaks API clients.
    /// Pass an instance to any client constructor, for example
    /// <c>new CopyleaksScansApi(httpClient, new CopyleaksClientOptions { MaxRetries = 5 })</c>.
    /// </summary>
    public class CopyleaksClientOptions
    {
        private const int DefaultTimeoutMilliseconds = 60000;

        /// <summary>
        /// How long a single attempt (sending the request and reading the response) may take before it is cancelled.
        /// The default is read from <c>ConfigurationManager.Configuration["RequestsTimeout"]</c> (in milliseconds), which is 60 seconds.
        /// Use <see cref="System.Threading.Timeout.InfiniteTimeSpan"/> to turn the SDK timeout off.
        /// The timeout of an <see cref="System.Net.Http.HttpClient"/> you pass in still applies as well, except to reading
        /// the body of the PDF report and usage history downloads.
        /// When an attempt times out, the SDK throws a <see cref="TimeoutException"/> (after any retries).
        /// The PDF report and usage history downloads are streamed: their timeout also covers reading the body,
        /// and a failure while the body is read is not retried.
        /// </summary>
        public TimeSpan Timeout { get; set; } = ReadConfiguredTimeout();

        /// <summary>
        /// How many times a failed call is retried. The default is 3. Set it to 0 to turn retries off.
        /// <para>
        /// Calls that are safe to repeat are retried on network errors, timeouts and HTTP 408, 429, 500, 502, 503 and 504.
        /// They are the GET requests, login, start, delete, and the submits to a fixed scan id (plagiarism file, URL and OCR
        /// submits, AI image URL submit and AI video submit), where the server rejects a duplicate with 409 Conflict.
        /// Calls that are charged per call or that trigger an action (AI text detection, Grammar Checker, Text Moderation,
        /// AI image check, Image Plagiarism Detection, export and resend webhook) are retried only on HTTP 429,
        /// because then the server rejected them without running them.
        /// </para>
        /// <para>
        /// An attempt can time out after the server accepted it. The retry of such a submit then fails with
        /// 409 Conflict. <see cref="Exceptions.CopyleaksHttpException.Attempt"/> is the attempt that got the error:
        /// if you use a new scan id for every submit, a 409 with an Attempt greater than 1 means an earlier attempt
        /// was accepted, so treat the call as successful.
        /// </para>
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// The base delay of the exponential backoff between retries. The default is 1 second.
        /// The delay doubles on every retry, with random jitter, up to <see cref="MaxRetryDelay"/>.
        /// </summary>
        public TimeSpan RetryBaseDelay { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The longest the SDK waits before a retry. The default is 30 seconds.
        /// When the server sends a <c>Retry-After</c> header, the SDK waits that long instead.
        /// When <c>Retry-After</c> asks for a longer wait than this value, the SDK does not retry and returns the error.
        /// </summary>
        public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The API server address, for example "https://api.copyleaks.com/".
        /// Leave it null to use <c>ConfigurationManager.Configuration["apiEndPoint"]</c>, read when the API object is created.
        /// </summary>
        public string ApiEndpoint { get; set; }

        /// <summary>
        /// The identity (login) server address, for example "https://id.copyleaks.com/".
        /// Leave it null to use <c>ConfigurationManager.Configuration["idEndPoint"]</c>, read when the API object is created.
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
