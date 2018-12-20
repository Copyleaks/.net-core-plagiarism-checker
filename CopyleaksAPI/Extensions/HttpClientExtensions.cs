/********************************************************************************
 The MIT License(MIT)
 
 Copyright(c) 2016 Copyleaks LTD (https://copyleaks.com)
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
********************************************************************************/


using Copyleaks.SDK.V3.API.Helpers;
using System;
using System.Net.Http;

namespace Copyleaks.SDK.V3.API.Extensions
{
	// CR : Documentation is missing. 
	public static class HttpClientExtensions
    {
        static readonly string ASSEMBLY_VERSION = AssemblyHelper.GetVersion();

        public static void AddAuthentication(this HttpClient client, string token)
        {
            if(!client.DefaultRequestHeaders.Contains("Authorization")) // CR : Extract into const.
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public static void SetCopyleaksClient(this HttpClient client)
        {
            client.Timeout = TimeSpan.FromMilliseconds(int.Parse(ConfigurationManager.Configuration["RequestsTimeout"]));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"copyleaks-core-sdk/{ASSEMBLY_VERSION}");
        }
    }
}
