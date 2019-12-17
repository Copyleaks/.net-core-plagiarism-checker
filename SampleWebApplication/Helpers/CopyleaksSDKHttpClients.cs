using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Copyleaks.SDK.Demo.Helpers
{
    public class CopyleaksSDKHttpClients
    {
        public const string PRODUCT_EDUCATION = "education-ui";
        public const string PRODUCT_BUSINESSES = "businesses-ui";
        private static HttpClient _HttpClient { get; set; }
        private static CopyleaksIdentityApi _IdentityClient { get; set; }

        private static CopyleaksScansApi _EducationAPIClient { get; set; }
        private static CopyleaksScansApi _BusinessesAPIClient { get; set; }

        static CopyleaksSDKHttpClients()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,

            };
            _HttpClient = new HttpClient(handler);
            _IdentityClient = new CopyleaksIdentityApi(_HttpClient);
            _EducationAPIClient = new CopyleaksScansApi(PRODUCT_EDUCATION, _HttpClient);
            _BusinessesAPIClient = new CopyleaksScansApi(PRODUCT_BUSINESSES, _HttpClient);
        }
        public static CopyleaksScansApi APIClient(string product)
        {
            switch (product)
            {
                case PRODUCT_EDUCATION:
                    return _EducationAPIClient;
                case PRODUCT_BUSINESSES:
                    return _BusinessesAPIClient;
                default:
                    throw new Exception($"Product {product} not supported");
            }
        }


        public static CopyleaksIdentityApi IdentityClient()
        {
            return _IdentityClient;
        }
    }
}
