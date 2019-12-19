using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API.Helpers
{
    internal static class HttpClientRetrayPolicy
    {

        static object Just4lock = new object();
        static AsyncRetryPolicy<HttpResponseMessage> _RetryPolicy;
        public static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy
        {
            get
            {
                if (_RetryPolicy == null)
                    lock (Just4lock)
                        if (_RetryPolicy == null)
                            _RetryPolicy = Policy
                                .Handle<HttpRequestException>()
                                .OrResult<HttpResponseMessage>(response => (int)response.StatusCode >= 500)
                                  .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(4));

                return _RetryPolicy;
            }
        }
    }
}
