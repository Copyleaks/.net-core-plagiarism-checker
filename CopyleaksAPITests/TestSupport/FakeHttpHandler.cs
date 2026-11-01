using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CopyleaksAPITests.TestSupport
{
    /// <summary>One request as the server would see it: method, URL, headers and the (decompressed) body.</summary>
    public sealed class RecordedRequest
    {
        public HttpMethod Method { get; set; }
        public Uri Uri { get; set; }
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> ContentHeaders { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public long? ContentLength { get; set; }
        public byte[] RawBody { get; set; }

        /// <summary>The body after gzip decoding, when the request was gzip-encoded.</summary>
        public byte[] Body { get; set; }

        public string BodyText => Body == null ? null : Encoding.UTF8.GetString(Body);

        public JObject Json => JObject.Parse(BodyText);

        public bool IsGzip => ContentHeaders.TryGetValue("Content-Encoding", out var encoding) && encoding.Contains("gzip");

        public string Header(string name) => Headers.TryGetValue(name, out var value) ? value : null;
    }

    /// <summary>
    /// A fake HTTP server for offline tests. It records every request and answers with the next queued responder.
    /// When the queue runs out, the last responder answers every further request.
    /// </summary>
    public sealed class FakeHttpHandler : HttpMessageHandler
    {
        private readonly object _lock = new object();
        private readonly List<Func<RecordedRequest, CancellationToken, Task<HttpResponseMessage>>> _responders =
            new List<Func<RecordedRequest, CancellationToken, Task<HttpResponseMessage>>>();
        private int _next;

        public ConcurrentQueue<RecordedRequest> Requests { get; } = new ConcurrentQueue<RecordedRequest>();

        public List<RecordedRequest> RequestList => Requests.ToList();

        public RecordedRequest Single
        {
            get
            {
                var list = RequestList;
                if (list.Count != 1)
                    throw new InvalidOperationException($"Expected exactly 1 request, got {list.Count}.");
                return list[0];
            }
        }

        public FakeHttpHandler Respond(Func<RecordedRequest, CancellationToken, Task<HttpResponseMessage>> responder)
        {
            lock (_lock)
                _responders.Add(responder);
            return this;
        }

        public FakeHttpHandler Respond(HttpStatusCode status, string json = null, Action<HttpResponseMessage> configure = null)
        {
            return Respond((request, ct) =>
            {
                var response = new HttpResponseMessage(status);
                if (json != null)
                    response.Content = new StringContent(json, Encoding.UTF8, "application/json");
                configure?.Invoke(response);
                return Task.FromResult(response);
            });
        }

        public FakeHttpHandler RespondBytes(HttpStatusCode status, byte[] bytes, string contentType)
        {
            return Respond((request, ct) =>
            {
                var response = new HttpResponseMessage(status) { Content = new ByteArrayContent(bytes) };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                return Task.FromResult(response);
            });
        }

        public FakeHttpHandler Throw(Exception exception)
        {
            return Respond((request, ct) => Task.FromException<HttpResponseMessage>(exception));
        }

        /// <summary>Never answers until the request is cancelled.</summary>
        public FakeHttpHandler Hang()
        {
            return Respond(async (request, ct) =>
            {
                await Task.Delay(Timeout.Infinite, ct).ConfigureAwait(false);
                throw new InvalidOperationException("unreachable");
            });
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var recorded = new RecordedRequest { Method = request.Method, Uri = request.RequestUri };
            foreach (var header in request.Headers)
                recorded.Headers[header.Key] = string.Join(", ", header.Value);

            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                    recorded.ContentHeaders[header.Key] = string.Join(", ", header.Value);
                recorded.ContentLength = request.Content.Headers.ContentLength;
                recorded.RawBody = await request.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                recorded.Body = recorded.IsGzip ? Gunzip(recorded.RawBody) : recorded.RawBody;
            }
            Requests.Enqueue(recorded);

            Func<RecordedRequest, CancellationToken, Task<HttpResponseMessage>> responder;
            lock (_lock)
            {
                if (_responders.Count == 0)
                    throw new InvalidOperationException("No response configured.");
                responder = _responders[Math.Min(_next, _responders.Count - 1)];
                _next++;
            }
            var response = await responder(recorded, cancellationToken).ConfigureAwait(false);
            response.RequestMessage = request;
            return response;
        }

        public static byte[] Gunzip(byte[] data)
        {
            using (var input = new MemoryStream(data))
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                gzip.CopyTo(output);
                return output.ToArray();
            }
        }
    }

    /// <summary>Parses a multipart/form-data body into its parts.</summary>
    public sealed class FormPart
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
        public string Text => Encoding.UTF8.GetString(Data);
    }

    public static class Multipart
    {
        public static Dictionary<string, FormPart> Parse(RecordedRequest request)
        {
            string contentType = request.ContentHeaders["Content-Type"];
            string boundary = contentType.Split(';').Select(p => p.Trim())
                .First(p => p.StartsWith("boundary=", StringComparison.OrdinalIgnoreCase))
                .Substring("boundary=".Length).Trim('"');

            var parts = new Dictionary<string, FormPart>(StringComparer.Ordinal);
            byte[] body = request.Body;
            byte[] delimiter = Encoding.ASCII.GetBytes("--" + boundary);
            var positions = new List<int>();
            for (int i = 0; i <= body.Length - delimiter.Length; i++)
            {
                if (body.AsSpan(i, delimiter.Length).SequenceEqual(delimiter))
                    positions.Add(i);
            }

            for (int p = 0; p < positions.Count - 1; p++)
            {
                int start = positions[p] + delimiter.Length + 2; // skip CRLF
                int end = positions[p + 1] - 2;                  // drop CRLF before the next delimiter
                var section = body.AsSpan(start, end - start);
                int headerEnd = IndexOf(section, Encoding.ASCII.GetBytes("\r\n\r\n"));
                string headers = Encoding.UTF8.GetString(section.Slice(0, headerEnd));
                byte[] data = section.Slice(headerEnd + 4).ToArray();

                var part = new FormPart { Data = data };
                foreach (string line in headers.Split("\r\n"))
                {
                    if (line.StartsWith("Content-Disposition:", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (string item in line.Substring("Content-Disposition:".Length).Split(';').Select(x => x.Trim()))
                        {
                            if (item.StartsWith("name=")) part.Name = item.Substring(5).Trim('"');
                            if (item.StartsWith("filename=")) part.FileName = item.Substring(9).Trim('"');
                        }
                    }
                    else if (line.StartsWith("Content-Type:", StringComparison.OrdinalIgnoreCase))
                    {
                        part.ContentType = line.Substring("Content-Type:".Length).Trim();
                    }
                }
                parts[part.Name] = part;
            }
            return parts;
        }

        private static int IndexOf(ReadOnlySpan<byte> haystack, byte[] needle)
        {
            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                if (haystack.Slice(i, needle.Length).SequenceEqual(needle))
                    return i;
            }
            return -1;
        }
    }
}
