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
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection;
using Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API
{
    public class CopyleaksAiImageDetectionApi : CopyleaksBase
    {
        public string CopyleaksApiServer { get; private set; }

        private string ImageDetectionApiVersion { get; set; }

        /// <summary>
        /// Conection to Copyleaks AI Image Detection API
        /// </summary>
        /// <param name="client">The HttpClient to use, for example one created by IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksAiImageDetectionApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>
        /// Conection to Copyleaks AI Image Detection API, using the shared HttpClient of the SDK.
        /// </summary>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksAiImageDetectionApi(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null) : base(clientCertificate, options)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ResolveApiEndpoint();
            this.ImageDetectionApiVersion = ConfigurationManager.Configuration[CopyleaksConstants.ImageDetectionApiVersion];
        }

        private static void Validate(string scanId, string token)
        {
            if (string.IsNullOrEmpty(scanId))
                throw new ArgumentNullException(nameof(scanId), "ScanId is mandatory");

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token), "Token is mandatory");
        }

        private string CheckUri(string scanId) =>
            $"{this.CopyleaksApiServer}{this.ImageDetectionApiVersion}/ai-image-detector/{PathSegment(scanId)}/check";

        /// <summary>
        /// Detect AI-generated content in an image sent as base64 JSON (<see cref="CopyleaksAiImageDetectionRequestModel.Base64"/>).
        /// For large images prefer <see cref="SubmitFileAsync(string, Stream, CopyleaksAiImageDetectionRequestModel, string, CancellationToken)"/>,
        /// which uploads the file without base64.
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="requestModel">The image (base64), filename, model and options</param>
        /// <param name="token">A copyleaks access token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task<CopyleaksAiImageDetectionResponseModel> SubmitAsync(string scanId, CopyleaksAiImageDetectionRequestModel requestModel, string token, CancellationToken cancellationToken = default)
        {
            Validate(scanId, token);
            if (requestModel == null)
                throw new ArgumentNullException(nameof(requestModel));

            // Charged per call, so only a rate-limited (429) attempt is retried.
            return await SendJsonForJsonAsync<CopyleaksAiImageDetectionResponseModel>(
                HttpMethod.Post, CheckUri(scanId), token, requestModel, gzip: false, eRetryMode.RateLimitOnly, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Detect AI-generated content in an image uploaded as a file (multipart/form-data).
        /// This avoids the base64 overhead. <see cref="CopyleaksAiImageDetectionRequestModel.Base64"/> must be null.
        /// The stream is read from its current position and is not disposed.
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="image">The image content</param>
        /// <param name="requestModel">The filename, model and options</param>
        /// <param name="token">A copyleaks access token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task<CopyleaksAiImageDetectionResponseModel> SubmitFileAsync(string scanId, Stream image, CopyleaksAiImageDetectionRequestModel requestModel, string token, CancellationToken cancellationToken = default)
        {
            Validate(scanId, token);
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            ValidateFileRequest(requestModel);

            long start = image.CanSeek ? image.Position : 0;
            return await SendForJsonAsync<CopyleaksAiImageDetectionResponseModel>(
                HttpMethod.Post, CheckUri(scanId), token,
                () => BuildForm(new SourceStreamContent(image, start), requestModel),
                image.CanSeek ? eRetryMode.RateLimitOnly : eRetryMode.None, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Detect AI-generated content in an image uploaded as a file (multipart/form-data).
        /// <see cref="CopyleaksAiImageDetectionRequestModel.Base64"/> must be null.
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="image">The image bytes</param>
        /// <param name="requestModel">The filename, model and options</param>
        /// <param name="token">A copyleaks access token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task<CopyleaksAiImageDetectionResponseModel> SubmitFileAsync(string scanId, byte[] image, CopyleaksAiImageDetectionRequestModel requestModel, string token, CancellationToken cancellationToken = default)
        {
            Validate(scanId, token);
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            ValidateFileRequest(requestModel);

            return await SendForJsonAsync<CopyleaksAiImageDetectionResponseModel>(
                HttpMethod.Post, CheckUri(scanId), token,
                () => BuildForm(new ByteArrayContent(image), requestModel),
                eRetryMode.RateLimitOnly, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Submit an image URL for asynchronous AI image detection. The request returns as soon as it is accepted
        /// (201 Created), and the result is posted to <see cref="CopyleaksAiImageDetectionAsyncRequestModel.Webhooks"/>
        /// with the shape of <see cref="CopyleaksAiImageDetectionResponseModel"/>.
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="requestModel">The image URL, filename, model, webhook and options</param>
        /// <param name="token">A copyleaks access token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <exception cref="CopyleaksHttpException"></exception>
        public async Task SubmitUrlAsync(string scanId, CopyleaksAiImageDetectionAsyncRequestModel requestModel, string token, CancellationToken cancellationToken = default)
        {
            Validate(scanId, token);
            if (requestModel == null)
                throw new ArgumentNullException(nameof(requestModel));
            if (requestModel.Url == null)
                throw new ArgumentException("Url is mandatory.", nameof(requestModel.Url));
            if (requestModel.Webhooks?.Url == null)
                throw new ArgumentException("Webhooks.Url is mandatory.", nameof(requestModel.Webhooks));

            string requestUri = $"{this.CopyleaksApiServer}{this.ImageDetectionApiVersion}/ai-image-detector-async/{PathSegment(scanId)}/submit";
            // A submit to a fixed scan id is safe to repeat: a duplicate is rejected with 409 Conflict,
            // and CopyleaksHttpException.Attempt tells the caller when that 409 answered a retry.
            await SendJsonAndEnsureSuccessAsync(
                HttpMethod.Put, requestUri, token, requestModel, gzip: false, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        private static void ValidateFileRequest(CopyleaksAiImageDetectionRequestModel requestModel)
        {
            if (requestModel == null)
                throw new ArgumentNullException(nameof(requestModel));
            if (string.IsNullOrEmpty(requestModel.FileName))
                throw new ArgumentException("FileName is mandatory.", nameof(requestModel.FileName));
            if (string.IsNullOrEmpty(requestModel.Model))
                throw new ArgumentException("Model is mandatory.", nameof(requestModel.Model));
            if (requestModel.Base64 != null)
                throw new ArgumentException("Base64 must be null when the image is uploaded as a file.", nameof(requestModel.Base64));
        }

        private static MultipartFormDataContent BuildForm(HttpContent image, CopyleaksAiImageDetectionRequestModel requestModel)
        {
            var form = new MultipartFormDataContent();
            MultipartHelper.AddFile(form, "image", image, requestModel.FileName);
            MultipartHelper.AddField(form, "filename", requestModel.FileName);
            MultipartHelper.AddField(form, "model", requestModel.Model);
            MultipartHelper.AddField(form, "sandbox", requestModel.Sandbox ? "true" : "false");
            if (requestModel.MaskType != null)
                MultipartHelper.AddField(form, "maskType", requestModel.MaskType);
            if (requestModel.ObjectDetection != null)
                MultipartHelper.AddField(form, "objectDetection", JsonConvert.SerializeObject(requestModel.ObjectDetection));
            return form;
        }
    }
}
