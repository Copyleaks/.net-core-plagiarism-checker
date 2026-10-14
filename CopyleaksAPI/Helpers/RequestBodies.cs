using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>
    /// A request body that is serialized once, straight into pooled buffers, and can then be sent
    /// as many times as the retry policy needs. There is no intermediate JSON string and no copy of the
    /// whole body in one large array, so large submits use about one compressed copy of the payload.
    /// </summary>
    internal sealed class BufferedBody : IDisposable
    {
        private const int SegmentSize = 64 * 1024;
        private static readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private readonly List<byte[]> _segments = new List<byte[]>();
        private int _lastSegmentUsed = SegmentSize;
        private bool _disposed;

        private BufferedBody(bool gzip)
        {
            Gzip = gzip;
        }

        public bool Gzip { get; }

        public long Length { get; private set; }

        /// <summary>
        /// Serialize <paramref name="model"/> as JSON with the default Newtonsoft settings.
        /// </summary>
        public static BufferedBody Json(object model, bool gzip)
        {
            var body = new BufferedBody(gzip);
            try
            {
                body.Write(writer =>
                {
                    using (var json = new JsonTextWriter(writer) { CloseOutput = false })
                    {
                        JsonSerializer.CreateDefault().Serialize(json, model);
                    }
                });
                return body;
            }
            catch
            {
                body.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Write a JSON object whose "base64" property is the base64 encoding of <paramref name="source"/>,
        /// followed by the other properties of <paramref name="rest"/>. The file is encoded while it is read,
        /// so the base64 string is never held in memory.
        /// </summary>
        public static BufferedBody JsonWithStreamedBase64(Stream source, JObject rest, bool gzip)
        {
            var body = new BufferedBody(gzip);
            try
            {
                body.Write(writer =>
                {
                    writer.Write("{\"base64\":\"");
                    WriteBase64(source, writer);
                    writer.Write('"');
                    foreach (JProperty property in rest.Properties())
                    {
                        writer.Write(',');
                        using (var json = new JsonTextWriter(writer) { CloseOutput = false })
                        {
                            json.WriteValue(property.Name);
                        }
                        writer.Write(':');
                        using (var json = new JsonTextWriter(writer) { CloseOutput = false })
                        {
                            property.Value.WriteTo(json);
                        }
                    }
                    writer.Write('}');
                });
                return body;
            }
            catch
            {
                body.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Create a new HttpContent over the buffered bytes. The content does not own the buffers,
        /// so it can be created once per attempt.
        /// </summary>
        public HttpContent CreateContent()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BufferedBody));

            var content = new SegmentsContent(this);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };
            if (Gzip)
                content.Headers.ContentEncoding.Add("gzip");
            return content;
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            foreach (byte[] segment in _segments)
                ArrayPool<byte>.Shared.Return(segment);
            _segments.Clear();
        }

        private void Write(Action<TextWriter> write)
        {
            using (var sink = new SegmentWriteStream(this))
            {
                Stream target = Gzip ? new GZipStream(sink, CompressionLevel.Optimal, leaveOpen: true) : (Stream)sink;
                try
                {
                    using (var writer = new StreamWriter(target, Utf8, 16 * 1024, leaveOpen: true))
                    {
                        write(writer);
                    }
                }
                finally
                {
                    if (Gzip)
                        target.Dispose();
                }
            }
        }

        private void Append(ReadOnlySpan<byte> data)
        {
            while (!data.IsEmpty)
            {
                if (_lastSegmentUsed == SegmentSize)
                {
                    _segments.Add(ArrayPool<byte>.Shared.Rent(SegmentSize));
                    _lastSegmentUsed = 0;
                }
                byte[] current = _segments[_segments.Count - 1];
                int count = Math.Min(SegmentSize - _lastSegmentUsed, data.Length);
                data.Slice(0, count).CopyTo(current.AsSpan(_lastSegmentUsed));
                _lastSegmentUsed += count;
                Length += count;
                data = data.Slice(count);
            }
        }

        private async Task CopyToAsync(Stream destination, CancellationToken cancellationToken)
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                int count = i == _segments.Count - 1 ? _lastSegmentUsed : SegmentSize;
                await destination.WriteAsync(_segments[i].AsMemory(0, count), cancellationToken).ConfigureAwait(false);
            }
        }

        private static void WriteBase64(Stream source, TextWriter writer)
        {
            // A multiple of 3 bytes, so every chunk encodes without padding until the last one.
            const int chunkBytes = 3 * 16 * 1024;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(chunkBytes);
            char[] chars = ArrayPool<char>.Shared.Rent(4 * 16 * 1024 + 4);
            try
            {
                int filled = 0;
                while (true)
                {
                    int read = source.Read(buffer, filled, chunkBytes - filled);
                    if (read > 0)
                    {
                        filled += read;
                        if (filled < chunkBytes)
                            continue;
                    }

                    if (filled > 0)
                    {
                        int charCount = Convert.ToBase64CharArray(buffer, 0, filled, chars, 0);
                        writer.Write(chars, 0, charCount);
                        filled = 0;
                    }

                    if (read == 0)
                        break;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
                ArrayPool<char>.Shared.Return(chars);
            }
        }

        private sealed class SegmentWriteStream : Stream
        {
            private readonly BufferedBody _owner;

            public SegmentWriteStream(BufferedBody owner)
            {
                _owner = owner;
            }

            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => _owner.Length;
            public override long Position { get => _owner.Length; set => throw new NotSupportedException(); }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => _owner.Append(buffer.AsSpan(offset, count));
            public override void Write(ReadOnlySpan<byte> buffer) => _owner.Append(buffer);
        }

        private sealed class SegmentsContent : HttpContent
        {
            private readonly BufferedBody _body;

            public SegmentsContent(BufferedBody body)
            {
                _body = body;
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
                => _body.CopyToAsync(stream, CancellationToken.None);

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context, CancellationToken cancellationToken)
                => _body.CopyToAsync(stream, cancellationToken);

            protected override bool TryComputeLength(out long length)
            {
                length = _body.Length;
                return true;
            }
        }
    }

    /// <summary>
    /// Streams a caller's Stream as request content without disposing it, starting from the position it had
    /// when the call was made, so a seekable stream can be sent again on a retry.
    /// </summary>
    internal sealed class SourceStreamContent : HttpContent
    {
        private readonly Stream _source;
        private readonly long _start;

        public SourceStreamContent(Stream source, long start)
        {
            _source = source;
            _start = start;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            => SerializeToStreamAsync(stream, context, CancellationToken.None);

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context, CancellationToken cancellationToken)
        {
            if (_source.CanSeek)
                _source.Position = _start;
            await _source.CopyToAsync(stream, 81920, cancellationToken).ConfigureAwait(false);
        }

        protected override bool TryComputeLength(out long length)
        {
            if (_source.CanSeek)
            {
                length = Math.Max(0, _source.Length - _start);
                return true;
            }
            length = 0;
            return false;
        }
    }
}
