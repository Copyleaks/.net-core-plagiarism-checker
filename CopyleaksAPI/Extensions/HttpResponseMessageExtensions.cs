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

using Copyleaks.SDK.V3.API.Exceptions;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<string> ExtractStringResultsAsync(this HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
                throw new CommandFailedException(message);
            return await message.Content.ReadAsStringAsync();
        }

        public static async Task<T> ExtractJsonResultsAsync<T>(this HttpResponseMessage message)
        {
            if(!message.IsSuccessStatusCode)
                throw new CommandFailedException(message);
            string json = await message.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json))
                throw new JsonException("This request could not be processed.");
            var result = JsonConvert.DeserializeObject<T>(json);
            if (result == null)
                throw new JsonException("Unable to process server response.");
            return result;
        }
    }
}
