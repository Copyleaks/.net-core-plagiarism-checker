using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API.Extensions;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>
    /// The response of the attempt that ended a call, with the 1-based number of that attempt.
    /// A streamed response also owns the timeout of its attempt, which keeps running while the body is read.
    /// </summary>
    internal sealed class ApiResponse : IDisposable
    {
        private readonly CancellationTokenSource _attemptTimeout;

        public ApiResponse(HttpResponseMessage message, int attempt, CancellationToken token, CancellationTokenSource attemptTimeout = null)
        {
            Message = message;
            Attempt = attempt;
            Token = token;
            _attemptTimeout = attemptTimeout;
        }

        public HttpResponseMessage Message { get; }

        public int Attempt { get; }

        /// <summary>
        /// Cancels reading the body: the caller's token, and for a streamed response also the attempt timeout.
        /// </summary>
        public CancellationToken Token { get; }

        /// <summary>
        /// Throw a <see cref="Exceptions.CopyleaksHttpException"/> when the response is not a success.
        /// </summary>
        public Task EnsureSuccessAsync() => Message.EnsureCopyleaksSuccessAsync(Attempt, Token);

        /// <summary>
        /// Throw when the response is not a success, otherwise deserialize the JSON body.
        /// </summary>
        public Task<T> ReadJsonAsync<T>() => Message.ExtractJsonResultsAsync<T>(Attempt, Token);

        public void Dispose()
        {
            Message.Dispose();
            _attemptTimeout?.Dispose();
        }
    }
}
