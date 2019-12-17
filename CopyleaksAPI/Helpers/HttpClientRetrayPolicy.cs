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
    public static class HttpClientRetrayPolicy
    {
        private static HttpStatusCode[] httpStatusCodesWorthRetrying = {                
                HttpStatusCode.RequestTimeout, // 408
                HttpStatusCode.InternalServerError, // 500
                HttpStatusCode.BadGateway, // 502
                HttpStatusCode.ServiceUnavailable, // 503
                HttpStatusCode.GatewayTimeout // 504
        };

        private static AsyncRetryPolicy<HttpResponseMessage> RetrayPolicy { get; set; }

        static HttpClientRetrayPolicy()
        {
            RetrayPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                .WaitAndRetryAsync(new[] {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }, (exception, timeSpan) =>
                {
                    Console.WriteLine("********************************************");
                    Console.WriteLine("********************************************");
                    Console.WriteLine("********************************************");
                    Console.WriteLine($"{timeSpan} - result {JsonConvert.SerializeObject(exception.Result)}");
                    Console.WriteLine("********************************************");
                    Console.WriteLine("********************************************");
                    Console.WriteLine("********************************************");
                });
        }

        public static AsyncRetryPolicy<HttpResponseMessage> GetPolicy()
        {
            return RetrayPolicy;
        }
    }
}
