using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.HttpCustomSend;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API.Extensions
{
    public static class HttpRequestMessageExtensions
    {                        
        public static void SetupHeaders(this HttpRequestMessage msg , string token = null)
        {
            msg.Headers.Clear();
            msg.Headers.Add("Connection", "keep-alive");
            msg.Headers.Add("Keep-Alive", "600");

            msg.Headers.Accept.Clear();
            msg.Headers.UserAgent.ParseAdd($"copyleaks-core-sdk/{AssemblyHelper.GetVersion()}");

            if (!string.IsNullOrEmpty(token))
                msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
