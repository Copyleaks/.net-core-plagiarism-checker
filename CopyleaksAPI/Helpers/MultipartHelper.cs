using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Copyleaks.SDK.V3.API.Helpers
{
    internal static class MultipartHelper
    {
        /// <summary>
        /// Add a plain form field. Like an HTML form, the part has no Content-Type header.
        /// </summary>
        public static void AddField(MultipartFormDataContent form, string name, string value)
        {
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(value ?? string.Empty));
            form.Add(content, name);
        }

        /// <summary>
        /// Add a file part with a Content-Type that matches the file extension.
        /// </summary>
        public static void AddFile(MultipartFormDataContent form, string name, HttpContent content, string filename)
        {
            content.Headers.ContentType = new MediaTypeHeaderValue(GetImageMediaType(filename));
            form.Add(content, name, filename ?? "file");
        }

        private static string GetImageMediaType(string filename)
        {
            string extension = Path.GetExtension(filename ?? string.Empty).ToLowerInvariant();
            switch (extension)
            {
                case ".png": return "image/png";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".gif": return "image/gif";
                case ".bmp": return "image/bmp";
                case ".webp": return "image/webp";
                case ".ico": return "image/x-icon";
                case ".heic": return "image/heic";
                case ".heif": return "image/heif";
                case ".tif":
                case ".tiff": return "image/tiff";
                default: return "application/octet-stream";
            }
        }
    }
}
