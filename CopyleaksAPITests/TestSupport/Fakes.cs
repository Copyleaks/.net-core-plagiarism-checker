using System;
using System.Net.Http;
using Copyleaks.SDK.V3.API;

namespace CopyleaksAPITests.TestSupport
{
    public static class Fakes
    {
        public const string Token = "test-token";

        /// <summary>Options with tiny retry delays, so retry tests run fast.</summary>
        public static CopyleaksClientOptions FastRetries(int maxRetries = 3) => new CopyleaksClientOptions
        {
            MaxRetries = maxRetries,
            RetryBaseDelay = TimeSpan.FromMilliseconds(1),
            MaxRetryDelay = TimeSpan.FromSeconds(5),
            Timeout = TimeSpan.FromSeconds(30),
        };

        public static HttpClient Client(FakeHttpHandler handler) => new HttpClient(handler, disposeHandler: false);
    }
}
