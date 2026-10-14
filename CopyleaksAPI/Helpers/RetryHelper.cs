using System;
using System.Net.Http;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>
    /// Which failures of a call may be retried.
    /// </summary>
    internal enum eRetryMode
    {
        /// <summary>
        /// The call is safe to repeat: retry network errors, timeouts and HTTP 408, 429, 500, 502, 503 and 504.
        /// </summary>
        Safe,

        /// <summary>
        /// The call is charged or triggers an action: retry only HTTP 429, which means the server rejected it without running it.
        /// </summary>
        RateLimitOnly,

        /// <summary>
        /// Never retry (for example, the request body is a stream that cannot be read twice).
        /// </summary>
        None,
    }

    internal static class RetryHelper
    {
        /// <summary>
        /// The Retry-After header of a response as a delay, or null when there is none.
        /// </summary>
        public static TimeSpan? GetRetryAfter(HttpResponseMessage response)
        {
            var retryAfter = response?.Headers?.RetryAfter;
            if (retryAfter == null)
                return null;
            if (retryAfter.Delta.HasValue)
                return retryAfter.Delta.Value < TimeSpan.Zero ? TimeSpan.Zero : retryAfter.Delta.Value;
            if (retryAfter.Date.HasValue)
            {
                TimeSpan wait = retryAfter.Date.Value - DateTimeOffset.UtcNow;
                return wait < TimeSpan.Zero ? TimeSpan.Zero : wait;
            }
            return null;
        }

        public static bool IsRetryableStatus(int statusCode, eRetryMode mode)
        {
            if (mode == eRetryMode.None)
                return false;
            if (statusCode == 429)
                return true;
            if (mode != eRetryMode.Safe)
                return false;
            return statusCode == 408 || statusCode == 500 || statusCode == 502 || statusCode == 503 || statusCode == 504;
        }

        /// <summary>
        /// Exponential backoff with jitter: base * 2^attempt, capped at max, scaled by a random factor between 0.5 and 1.
        /// </summary>
        public static TimeSpan Backoff(int attempt, TimeSpan baseDelay, TimeSpan maxDelay)
        {
            if (baseDelay <= TimeSpan.Zero)
                return TimeSpan.Zero;
            double exponential = baseDelay.TotalMilliseconds * Math.Pow(2, Math.Min(attempt, 30));
            double capped = maxDelay > TimeSpan.Zero ? Math.Min(exponential, maxDelay.TotalMilliseconds) : exponential;
            double jittered = capped * (0.5 + Random.Shared.NextDouble() * 0.5);
            return TimeSpan.FromMilliseconds(jittered);
        }
    }
}
