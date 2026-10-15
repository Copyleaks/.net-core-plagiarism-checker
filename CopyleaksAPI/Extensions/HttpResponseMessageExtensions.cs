using Copyleaks.SDK.V3.API.Exceptions;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API.Extensions
{
    /// <summary>HttpResponseMessage helpers.</summary>
    internal static class HttpResponseMessageExtensions
    {
        /// <summary>Throw <see cref="CopyleaksHttpException"/> on a non-success response (1-based attempt).</summary>
        public static async Task EnsureCopyleaksSuccessAsync(this HttpResponseMessage message, int attempt, CancellationToken cancellationToken = default)
        {
            if (!message.IsSuccessStatusCode)
                throw await CopyleaksHttpException.CreateAsync(message, attempt, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Ensure success, then deserialize the JSON body into T.</summary>
        public static async Task<T> ExtractJsonResultsAsync<T>(this HttpResponseMessage message, int attempt, CancellationToken cancellationToken = default)
        {
            await message.EnsureCopyleaksSuccessAsync(attempt, cancellationToken).ConfigureAwait(false);

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
