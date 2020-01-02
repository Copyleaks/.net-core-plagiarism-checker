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
using Copyleaks.SDK.V3.API.Extensions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.HttpCustomSend;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Responses;
using Copyleaks.SDK.V3.API.Models.Responses.Download;
using Copyleaks.SDK.V3.API.Models.Responses.Result;
using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;
using Polly.Retry;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API
{
    /// <summary>
    /// This class allows you to connect to Copyleaks API, 
    /// scan for plagiarism and get your scan results. 
    /// </summary>
    public class CopyleaksScansApi : CopyleaksBase
    {
        /// <summary>
        /// The product for scanning the documents
        /// </summary>
        public string Product { get; private set; }

        public string CopyleaksApiServer { get; private set; }

        /// <summary>
        /// Conection to Copyleaks scan API
        /// </summary>
        /// <param name="product">The product for scanning the documents</param>
        /// <param name="token">Login Token, aquire a login token by invoking by using `CopyleaksIdentityApi.LoginAsync`</param>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.
        /// Configure you client's certificate at https://copyleaks.com/Manage </param>
        public CopyleaksScansApi(eProduct product, X509Certificate2 clientCertificate = null) : this(product.ToString().ToLower(), clientCertificate) { }

        /// <summary>
        /// Conection to Copyleaks scan API
        /// </summary>
        /// <param name="product">The product for scanning the documents</param>
        /// <param name="token">Login Token, aquire a login token by invoking by using `Copyle
        /// <param name="client">Override the underlying http client with custom settings</param>
        public CopyleaksScansApi(string product, HttpClient client) : base(client)
        {
            SetUpService(product);
        }

        /// <summary>
        /// Conection to Copyleaks scan API
        /// </summary>
        /// <param name="product">The product for scanning the documents</param>
        /// <param name="token">Login Token, aquire a login token by invoking by using `CopyleaksIdentityApi.LoginAsync`</param>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.
        public CopyleaksScansApi(string product, X509Certificate2 clientCertificate = null) : base(clientCertificate)
        {
            SetUpService(product);
        }

        private void SetUpService(string product)
        {
            this.CopyleaksApiServer = ConfigurationManager.Configuration["apiEndPoint"];
            this.Product = product;
        }

        /// <summary>
        /// Get your current credit balance
        /// </summary>
        /// <param name="token">Login Token, aquire a login token by invoking by using `CopyleaksIdentityApi.LoginAsync`</param>
        /// <returns>Current credit balance</returns>
        public async Task<uint> CreditBalanceAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("mandatory", nameof(token));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/credits";
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, requestUri);
            msg.SetupHeaders(token);

            using (var stream = new MemoryStream())
            {
                var task = Task.Run(async () =>
                {
                    using (var streamContent = new StreamContent(stream))
                        return await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false));
                });

                using (var response = await RetryPolicy.ExecuteAsync(() => task).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new CopyleaksHttpException(response);
                    }

                    var credits = await response.ExtractJsonResultsAsync<CountCreditsResponse>().ConfigureAwait(false);
                    return credits.Amount;
                }
            }
        }

        private async Task SubmitAsync(Document model, string requestUri, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("mandatory", nameof(token));

            var msg = new HttpRequestMessage(HttpMethod.Put, requestUri);
            msg.SetupHeaders(token);

            msg.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            using (var stream = new MemoryStream())
            using (var streamContent = new StreamContent(stream))
            using (var response = await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new CopyleaksHttpException(response);
                }
            }
        }

        /// <summary>
        /// Submitting URL to plagiarism scan
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The url and scan properties</param>
        /// <returns>A task that represents the asynchronous submit operation.</returns>
        public async Task SubmitUrlAsync(string scanId, UrlDocument documentModel, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));

            if (documentModel.Url == null)
                throw new ArgumentException("Url is mandatory.", nameof(documentModel.Url));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/submit/url/{scanId}";
            await SubmitAsync(documentModel, requestUri, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Submitting local file or free text to plagiarism scan
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The file or free text encoded in base64 and scan properties</param>
        /// <returns>A task that represents the asynchronous submit operation.</returns>
        public async Task SubmitFileAsync(string scanId, FileDocument documentModel, string token)
        {
            if (documentModel.Base64 == null)
                throw new ArgumentException("Base64 is mandatory.", nameof(documentModel.Base64));
            else if (documentModel.Filename == null)
                throw new ArgumentException("Filename is mandatory.", nameof(documentModel.Filename));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/submit/file/{scanId}";
            await SubmitAsync(documentModel, requestUri, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Submitting an image containing textual content to plagiarism scan
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The image file encoded in base64 and scan properties</param>
        /// <returns>A task that represents the asynchronous submit operation.</returns>
        public async Task SubmitImageOCRAsync(string scanId, FileOcrDocument documentModel, string token)
        {
            if (documentModel.Base64 == null)
                throw new ArgumentException("Base64 is mandatory.", nameof(documentModel.Base64));
            else if (documentModel.Filename == null)
                throw new ArgumentException("Filename is mandatory.", nameof(documentModel.Filename));
            else if (documentModel.Language == null)
                throw new ArgumentException("Language is mandatory.", nameof(documentModel.Language));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/submit/ocr/{scanId}";
            await SubmitAsync(documentModel, requestUri, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the perecent progress of the scan
        /// </summary>
        /// <param name="scanId">The scan Id</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the perecent progress of the scan </returns>
        public async Task<uint> ProgressAsync(string scanId, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("mandatory", nameof(token));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/{scanId}/progress";
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, requestUri);
            msg.SetupHeaders(token);


            using (var stream = new MemoryStream())
            {
                var task = Task.Run(async () =>
                {
                    using (var streamContent = new StreamContent(stream))
                        return await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false));
                });
                using (var response = await RetryPolicy.ExecuteAsync(() => task).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new CopyleaksHttpException(response);
                    }
                    var progress = await response.ExtractJsonResultsAsync<ProgressResponse>().ConfigureAwait(false);
                    return progress.Percents;
                }
            }
        }

        /// <summary>
        /// Deletes the process once it has finished running 
        /// The delete process might take few minutes to complete
        /// </summary>
        /// <param name="scanIds">A list of completed scan id's to be deleted</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(DeleteRequest model, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("mandatory", nameof(token));

            var method = new HttpMethod("PATCH");
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}.1/{this.Product}/delete";
            HttpRequestMessage msg = new HttpRequestMessage(method, requestUri);
            msg.SetupHeaders(token);
            msg.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            using (var stream = new MemoryStream())
            {
                var task = Task.Run(async () =>
                {
                    using (var streamContent = new StreamContent(stream))
                        return await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false));
                });
                using (var response = await RetryPolicy.ExecuteAsync(() => task).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new CopyleaksHttpException(response);
                    }
                }
            }
        }

        /// <summary>
        /// Start processes which are in 'price checked' status
        /// </summary>
        /// <param name="token">Login Token, aquire a login token by invoking by using `CopyleaksIdentityApi.LoginAsync`</param>
        /// <param name="model">A model with the list of scan id's to start and error handeling defenition in case one or more scans have a starting error</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of successfully started scans and a list of scans that have failed starting</returns>
        public async Task<StartResponse> StartAsync(StartRequest model, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));

            if (model.Trigger == null)
                throw new ArgumentException("Trigger is mandatory.", nameof(model.Trigger));

            var method = new HttpMethod("PATCH");
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/start";
            HttpRequestMessage msg = new HttpRequestMessage(method, requestUri);
            msg.SetupHeaders(token);
            msg.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            using (var stream = new MemoryStream())
            using (var streamContent = new StreamContent(stream))
            using (var response = await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new CopyleaksHttpException(response);
                }
                return await response.ExtractJsonResultsAsync<StartResponse>().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Starts a batch scan for a list of 'proceChecked' scans 
        /// </summary>
        /// <param name="model">A model with the list of scan id's to start and error handeling defenition in case one or more scans have a starting error.
        /// The Model also contains an include list of completed scans that will be compared against</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of successfully started scans and a list of scans that have failed starting </returns>
        public async Task<StartResponse> StartBatchAsync(StartBatchRequest model, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));

            if (model.Trigger == null)
                throw new ArgumentException("Trigger is mandatory.", nameof(model.Trigger));

            var method = new HttpMethod("PATCH");
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/batch/start";
            var msg = new HttpRequestMessage(method, requestUri);
            msg.SetupHeaders(token);
            msg.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            using (var stream = new MemoryStream())
            using (var streamContent = new StreamContent(stream))
            using (var response = await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new CopyleaksHttpException(response);
                }
                return await response.ExtractJsonResultsAsync<StartResponse>().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get the scan results from Copyleaks servers.
        /// </summary>
        /// <param name="scanId">A completed scan Id</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a model of the scan result</returns>
        public async Task<Result> ResultAsync(string scanId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token is mandatory", nameof(token));
            }

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/{scanId}/result";
            var msg = new HttpRequestMessage(HttpMethod.Get, requestUri);
            msg.SetupHeaders(token);

            using (var stream = new MemoryStream())
            {
                var task = Task.Run(async () =>
                {
                    using (var streamContent = new StreamContent(stream))
                        return await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false));
                });
                using (var response = await RetryPolicy.ExecuteAsync(() => task).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new CopyleaksHttpException(response);
                    }
                    return await response.ExtractJsonResultsAsync<Result>().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Get a suspected result comparison report
        /// </summary>
        /// <param name="scanId">The scan id</param>
        /// <param name="resultId">The result id (can be taken from ResultAsync method)</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a model of the result comparison report</returns>
        public async Task<DownloadResultResponse> DownloadResultAsync(string scanId, string resultId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token is mandatory", nameof(token));
            }

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/downloads/{scanId}/results/{resultId}";
            var msg = new HttpRequestMessage(HttpMethod.Get, requestUri);
            msg.SetupHeaders(token);

            using (var stream = new MemoryStream())
            {
                var task = Task.Run(async () =>
                {
                    using (var streamContent = new StreamContent(stream))
                        return await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false));
                });
                using (var response = await RetryPolicy.ExecuteAsync(() => task).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new CopyleaksHttpException(response);
                    }
                    return await response.ExtractJsonResultsAsync<DownloadResultResponse>().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Get the your source content comparison report 
        /// </summary>
        /// <param name="scanId">The scan id</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a model of your source content comparison report</returns>
        public async Task<DownloadSourceResponse> DownloadSourceReportAsync(string scanId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token is mandatory", nameof(token));
            }

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/downloads/{scanId}";
            var msg = new HttpRequestMessage(HttpMethod.Get, requestUri);
            msg.SetupHeaders(token);

            using (var stream = new MemoryStream())
            {
                var task = Task.Run(async () =>
                {
                    using (var streamContent = new StreamContent(stream))
                        return await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false));
                });
                using (var response = await RetryPolicy.ExecuteAsync(() => task).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new CopyleaksHttpException(response);
                    }
                    return await response.ExtractJsonResultsAsync<DownloadSourceResponse>().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Get the pdf report for your scan request
        /// </summary>
        /// <param name="scanId">The scan id</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a stream of the pdf report</returns>
        public async Task<Stream> DownloadPdfReportAsync(string scanId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token is mandatory", nameof(token));
            }

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/downloads/{scanId}/report.pdf";
            var msg = new HttpRequestMessage(HttpMethod.Get, requestUri);
            msg.SetupHeaders(token);

            using (var stream = new MemoryStream())
            {
                var task = Task.Run(async () =>
                {
                    using (var streamContent = new StreamContent(stream))
                        return await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false));
                });
                using (var response = await RetryPolicy.ExecuteAsync(() => task).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new CopyleaksHttpException(response);
                    }
                    var reportStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    reportStream.Seek(0, SeekOrigin.Begin);
                    return reportStream;
                }
            }
        }

        /// <summary>
        /// Exporting multiple Copyleaks data items in single call.
        /// </summary>
        /// <param name="scanId">Scan identified to be exporter</param>
        /// <param name="exportId">Export identifier for tracking</param>
        /// <param name="request">The export items (results, crawled-version and pdf report file)</param>
        /// <exception cref="HttpRequestException">In case of reject from the server.</exception>
        /// <returns></returns>
        public async Task ExportAsync(string scanId, string exportId, ExportRequest request, string token)
        {
            if (string.IsNullOrEmpty(scanId))
                throw new ArgumentException("Mandatory", nameof(scanId));
            if (string.IsNullOrEmpty(exportId))
                throw new ArgumentException("Mandatory", nameof(exportId));
            if (request == null)
                throw new ArgumentException("Mandatory", nameof(request));
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Mandatory", nameof(token));
            }

            Uri url = new Uri($"{this.CopyleaksApiServer}v3/downloads/{scanId}/export/{exportId}");
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, url);
            msg.SetupHeaders(token);
            msg.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            using (var stream = new MemoryStream())
            using (var streamContent = new StreamContent(stream))
            using (var response = await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new CopyleaksHttpException(response);
                }
            }
        }

        /// <summary>
        /// Get a list of the supported file types.
        /// </summary>
        /// <returns>List of textal file types supported and list of file types supported using OCR </returns>
        public async Task<SupportedTypesResult> GetSupportedFileTypesAsync()
        {
            string requestUri = $"{this.CopyleaksApiServer}v1/miscellaneous/supported-file-types";
            var msg = new HttpRequestMessage(HttpMethod.Get, requestUri);
            msg.SetupHeaders();

            using (var stream = new MemoryStream())
            {
                var task = Task.Run(async () =>
                {
                    using (var streamContent = new StreamContent(stream))
                        return await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false));
                });
                using (var response = await RetryPolicy.ExecuteAsync(() => task).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new CopyleaksHttpException(response);
                    }

                    return await response.ExtractJsonResultsAsync<SupportedTypesResult>().ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Get OCR Supported Langauges
        /// </summary>
        /// <returns>List of OCR Supported Langauges</returns>
        public async Task<string[]> GetOcrLanguageListAsync()
        {
            string requestUri = $"{this.CopyleaksApiServer}v1/miscellaneous/ocr-languages-list";
            var msg = new HttpRequestMessage(HttpMethod.Get, requestUri);
            msg.SetupHeaders();

            using (var stream = new MemoryStream())
            {
                var task = Task.Run(async () =>
                {
                    using (var streamContent = new StreamContent(stream))
                        return await Client.SendAsync(await msg.CloneAsync(stream, streamContent).ConfigureAwait(false));
                });
                using (var response = await RetryPolicy.ExecuteAsync(() => task).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new CopyleaksHttpException(response);
                    }

                    return await response.ExtractJsonResultsAsync<string[]>().ConfigureAwait(false);
                }
            }
        }
    }
}