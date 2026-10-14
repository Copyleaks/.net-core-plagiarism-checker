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
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API.Extensions
{
    /// <summary>
    /// Extensions for HttpResponseMessage class
    /// </summary>
    internal static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Throw a <see cref="CopyleaksHttpException"/> when the response is not a success.
        /// </summary>
        public static async Task EnsureCopyleaksSuccessAsync(this HttpResponseMessage message, CancellationToken cancellationToken = default)
        {
            if (!message.IsSuccessStatusCode)
                throw await CopyleaksHttpException.CreateAsync(message, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Extract type T from the response body
        /// </summary>
        /// <typeparam name="T">The expected result type</typeparam>
        /// <param name="message">The response</param>
        /// <param name="cancellationToken">Cancels reading the response</param>
        /// <returns>A deserialized object from the response</returns>
        public static async Task<T> ExtractJsonResultsAsync<T>(this HttpResponseMessage message, CancellationToken cancellationToken = default)
        {
            await message.EnsureCopyleaksSuccessAsync(cancellationToken).ConfigureAwait(false);

            if (message.Content == null || message.Content.Headers.ContentLength == 0)
                throw new JsonException("This request could not be processed.");

            // Deserialize straight from the response stream, without an intermediate string.
            using (Stream stream = await message.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
            using (var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
            using (var json = new JsonTextReader(reader))
            {
                T result = JsonSerializer.CreateDefault().Deserialize<T>(json);
                if (result == null)
                    throw new JsonException("Unable to process server response.");
                return result;
            }
        }
    }
}
