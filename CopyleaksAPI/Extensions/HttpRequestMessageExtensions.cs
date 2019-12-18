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
        public static void SetupHeaders(this HttpRequestMessage msg, string token = null)
        {
            msg.Headers.Clear();
            msg.Headers.Add("Connection", "keep-alive");
            msg.Headers.Add("Keep-Alive", "600");

            msg.Headers.Accept.Clear();
            msg.Headers.UserAgent.ParseAdd($"copyleaks-core-sdk/{AssemblyHelper.GetVersion()}");

            if (!string.IsNullOrEmpty(token))
                msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static async Task<HttpRequestMessage> CloneAsync(this HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = await request.Content.CloneAsync().ConfigureAwait(false),
                Version = request.Version
            };
            foreach (KeyValuePair<string, object> prop in request.Properties)
            {
                clone.Properties.Add(prop);
            }
            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }

        public static async Task<HttpContent> CloneAsync(this HttpContent content)
        {
            if (content == null) return null;

            var ms = new MemoryStream();
            await content.CopyToAsync(ms).ConfigureAwait(false);
            ms.Position = 0;

            var clone = new StreamContent(ms);
            foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
            {
                clone.Headers.Add(header.Key, header.Value);
            }
            return clone;
        }
    }
}
