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
        private static HttpClient _HttpClient { get; set; }
        private static CopyleaksIdentityApi _IdentityClient { get; set; }

        private static CopyleaksScansApi _APIClient { get; set; }

        static CopyleaksSDKHttpClients()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,

            };
            _HttpClient = new HttpClient(handler);
            _IdentityClient = new CopyleaksIdentityApi(_HttpClient);
			_APIClient = new CopyleaksScansApi(_HttpClient);
        }
        public static CopyleaksScansApi APIClient()
        {
			return _APIClient;
		}


        public static CopyleaksIdentityApi IdentityClient()
        {
            return _IdentityClient;
        }
    }
}
