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
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Copyleaks.SDK.V3.API.Models.Responses;
using Copyleaks.SDK.V3.API.Models.Responses.Download;
using Copyleaks.SDK.V3.API.Models.Responses.Result;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Copyleaks.SDK.V3.API
{
    /// <summary>
    /// This class allows you to connect to Copyleaks API, 
    /// scan for plagiarism and get your scan results. 
    /// </summary>
    public class CopyleaksScansApi : CopyleaksBase
    {
        private static readonly HttpMethod Patch = new HttpMethod("PATCH");

        public string CopyleaksApiServer { get; private set; }

        /// <summary>
        /// Conection to Copyleaks scan API
        /// </summary>
        /// <param name="client">The HttpClient to use, for example one created by IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksScansApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>
        /// Conection to Copyleaks scan API, using the shared HttpClient of the SDK.
        /// </summary>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.</param>
        /// <param name="options">Optional timeout and retry settings</param>
        public CopyleaksScansApi(X509Certificate2 clientCertificate = null, CopyleaksClientOptions options = null) : base(clientCertificate, options)
        {
            SetUpService();
        }

        private void SetUpService()
        {
            this.CopyleaksApiServer = ResolveApiEndpoint();
        }

        private static void RequireToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is mandatory", nameof(token));
        }

        /// <summary>
        /// Get your current credit balance
        /// </summary>
        /// <param name="token">Login Token, aquire a login token by invoking by using `CopyleaksIdentityApi.LoginAsync`</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>Current credit balance</returns>
        public async Task<uint> CreditBalanceAsync(string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/credits";
            var credits = await SendForJsonAsync<CountCreditsResponse>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
            return credits.Amount;
        }

        private async Task SubmitAsync(Document model, string requestUri, string token, CancellationToken cancellationToken)
        {
            RequireToken(token);

            // A submit to a fixed scan id is safe to repeat: a duplicate is rejected with 409 Conflict and is not charged twice.
            // CopyleaksHttpException.Attempt tells the caller when that 409 answered a retry.
            await SendJsonAndEnsureSuccessAsync(
                HttpMethod.Put, requestUri, token, model, gzip: true, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Submitting URL to plagiarism scan
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The url and scan properties</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>A task that represents the asynchronous submit operation.</returns>
        public async Task SubmitUrlAsync(string scanId, UrlDocument documentModel, string token, CancellationToken cancellationToken = default)
        {
            RequireScanId(scanId);
            RequireToken(token);
            if (documentModel == null)
                throw new ArgumentNullException(nameof(documentModel));
            if (documentModel.Url == null)
                throw new ArgumentException("Url is mandatory.", nameof(documentModel.Url));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/submit/url/{PathSegment(scanId)}";
            await SubmitAsync(documentModel, requestUri, token, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Submitting local file or free text to plagiarism scan
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The file or free text encoded in base64 and scan properties</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>A task that represents the asynchronous submit operation.</returns>
        public async Task SubmitFileAsync(string scanId, FileDocument documentModel, string token, CancellationToken cancellationToken = default)
        {
            RequireScanId(scanId);
            if (documentModel == null)
                throw new ArgumentNullException(nameof(documentModel));
            if (documentModel.Base64 == null)
                throw new ArgumentException("Base64 is mandatory.", nameof(documentModel.Base64));
            else if (documentModel.Filename == null)
                throw new ArgumentException("Filename is mandatory.", nameof(documentModel.Filename));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/submit/file/{PathSegment(scanId)}";
            await SubmitAsync(documentModel, requestUri, token, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Submit a file to plagiarism scan straight from a stream. The file is base64-encoded while it is read,
        /// so the SDK never holds a base64 string of the file in memory. Use this for large files.
        /// The stream is read asynchronously to its end (so a request body stream works too) and is not disposed.
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="file">The file content</param>
        /// <param name="filename">The original filename including its extension, for example "file.pdf"</param>
        /// <param name="properties">The scan properties</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        public async Task SubmitFileAsync(string scanId, Stream file, string filename, ScanProperties properties, string token, CancellationToken cancellationToken = default)
        {
            RequireScanId(scanId);
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (filename == null)
                throw new ArgumentException("Filename is mandatory.", nameof(filename));

            var rest = new FileDocument { Filename = filename, PropertiesSection = properties };
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/submit/file/{PathSegment(scanId)}";
            await SubmitStreamAsync(file, rest, requestUri, token, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Submitting an image containing textual content to plagiarism scan
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The image file encoded in base64 and scan properties</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>A task that represents the asynchronous submit operation.</returns>
        public async Task SubmitImageOCRAsync(string scanId, FileOcrDocument documentModel, string token, CancellationToken cancellationToken = default)
        {
            RequireScanId(scanId);
            if (documentModel == null)
                throw new ArgumentNullException(nameof(documentModel));
            if (documentModel.Base64 == null)
                throw new ArgumentException("Base64 is mandatory.", nameof(documentModel.Base64));
            else if (documentModel.Filename == null)
                throw new ArgumentException("Filename is mandatory.", nameof(documentModel.Filename));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/submit/ocr/{PathSegment(scanId)}";
            await SubmitAsync(documentModel, requestUri, token, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Submit an image with text to a plagiarism scan (OCR) straight from a stream. The image is base64-encoded while it is read.
        /// The stream is read asynchronously to its end and is not disposed.
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="image">The image content</param>
        /// <param name="filename">The image filename including its extension</param>
        /// <param name="language">The language code of the text in the image</param>
        /// <param name="properties">The scan properties</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        public async Task SubmitImageOCRAsync(string scanId, Stream image, string filename, string language, ScanProperties properties, string token, CancellationToken cancellationToken = default)
        {
            RequireScanId(scanId);
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (filename == null)
                throw new ArgumentException("Filename is mandatory.", nameof(filename));

            var rest = new FileOcrDocument { Filename = filename, Language = language, PropertiesSection = properties };
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/submit/ocr/{PathSegment(scanId)}";
            await SubmitStreamAsync(image, rest, requestUri, token, cancellationToken).ConfigureAwait(false);
        }

        private async Task SubmitStreamAsync(Stream source, FileDocument rest, string requestUri, string token, CancellationToken cancellationToken)
        {
            RequireToken(token);

            JObject restJson = JObject.FromObject(rest, JsonSerializer.CreateDefault());
            restJson.Remove("base64");
            using (var payload = await BufferedBody.JsonWithStreamedBase64Async(source, restJson, gzip: true, cancellationToken).ConfigureAwait(false))
            {
                await SendAndEnsureSuccessAsync(
                    HttpMethod.Put, requestUri, token, payload.CreateContent, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get the perecent progress of the scan
        /// </summary>
        /// <param name="scanId">The scan Id</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the perecent progress of the scan </returns>
        public async Task<uint> ProgressAsync(string scanId, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            RequireScanId(scanId);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/{PathSegment(scanId)}/progress";
            var progress = await SendForJsonAsync<ProgressResponse>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
            return progress.Percents;
        }

        /// <summary>
        /// Deletes the process once it has finished running 
        /// The delete process might take few minutes to complete
        /// </summary>
        /// <param name="model">The scans to delete, and an optional completion webhook</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(DeleteRequest model, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}.1/scans/delete";
            await SendJsonAndEnsureSuccessAsync(
                Patch, requestUri, token, model, gzip: true, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Start processes which are in 'price checked' status
        /// </summary>
        /// <param name="model">A model with the list of scan id's to start and error handeling defenition in case one or more scans have a starting error</param>
        /// <param name="token">Login Token, aquire a login token by invoking by using `CopyleaksIdentityApi.LoginAsync`</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of successfully started scans and a list of scans that have failed starting</returns>
        public async Task<StartResponse> StartAsync(StartRequest model, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (model.Trigger == null)
                throw new ArgumentException("Trigger is mandatory.", nameof(model.Trigger));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/start";
            return await SendJsonForJsonAsync<StartResponse>(
                Patch, requestUri, token, model, gzip: true, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Starts a batch scan for a list of 'proceChecked' scans 
        /// </summary>
        /// <param name="model">A model with the list of scan id's to start and error handeling defenition in case one or more scans have a starting error.
        /// The Model also contains an include list of completed scans that will be compared against</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of successfully started scans and a list of scans that have failed starting </returns>
        public async Task<StartResponse> StartBatchAsync(StartBatchRequest model, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (model.Trigger == null)
                throw new ArgumentException("Trigger is mandatory.", nameof(model.Trigger));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/batch/start";
            return await SendJsonForJsonAsync<StartResponse>(
                Patch, requestUri, token, model, gzip: true, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the scan results from Copyleaks servers.
        /// </summary>
        /// <param name="scanId">A completed scan Id</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a model of the scan result</returns>
        public async Task<Result> ResultAsync(string scanId, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            RequireScanId(scanId);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/{PathSegment(scanId)}/result";
            return await SendForJsonAsync<Result>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a suspected result comparison report
        /// </summary>
        /// <param name="scanId">The scan id</param>
        /// <param name="resultId">The result id (can be taken from ResultAsync method)</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a model of the result comparison report</returns>
        public async Task<DownloadResultResponse> DownloadResultAsync(string scanId, string resultId, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            RequireScanId(scanId);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/downloads/{PathSegment(scanId)}/results/{PathSegment(resultId)}";
            return await SendForJsonAsync<DownloadResultResponse>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get user usage as a stream.
        /// The CSV is copied into <paramref name="stream"/> as it arrives, and <see cref="CopyleaksClientOptions.Timeout"/> covers the whole download.
        /// </summary>
        /// <param name="startDate">The start date of wanted usage history.</param>
        /// <param name="endDate">The end date of wanted usage history.</param>
        /// <param name="stream">The CSV is written to this stream</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <exception cref="CopyleaksHttpException">In case of reject from the server.</exception>
        /// <returns>Returns user usage data as a stream.</returns>
        public async Task GetUserUsageAsync(DateTime startDate, DateTime endDate, Stream stream, string token, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Mandatory", nameof(token));

            string requestUri = $"{this.CopyleaksApiServer}v3/scans/usages/history" +
                $"?start={startDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)}" +
                $"&end={endDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)}";

            // Streamed straight into the caller's stream, without buffering the whole CSV first.
            await DownloadAsync(requestUri, token,
                (body, length, ct) => body.CopyToAsync(stream, 81920, ct), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the your source content comparison report 
        /// </summary>
        /// <param name="scanId">The scan id</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a model of your source content comparison report</returns>
        public async Task<DownloadSourceResponse> DownloadSourceReportAsync(string scanId, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            RequireScanId(scanId);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/downloads/{PathSegment(scanId)}";
            return await SendForJsonAsync<DownloadSourceResponse>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the pdf report for your scan request.
        /// <see cref="CopyleaksClientOptions.Timeout"/> covers the whole download.
        /// </summary>
        /// <param name="scanId">The scan id</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a stream of the pdf report, positioned at its start</returns>
        public async Task<Stream> DownloadPdfReportAsync(string scanId, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            RequireScanId(scanId);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/downloads/{PathSegment(scanId)}/report.pdf";
            MemoryStream pdf = null;
            await DownloadAsync(requestUri, token, async (body, length, ct) =>
            {
                pdf = new MemoryStream(length > 0 && length <= Array.MaxLength ? (int)length.Value : 0);
                await body.CopyToAsync(pdf, 81920, ct).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);

            // Hand out the downloaded bytes without copying them again, read-only as before.
            return new MemoryStream(pdf.GetBuffer(), 0, (int)pdf.Length, writable: false);
        }

        /// <summary>
        /// Exporting multiple Copyleaks data items in single call.
        /// </summary>
        /// <param name="scanId">Scan identified to be exporter</param>
        /// <param name="exportId">Export identifier for tracking</param>
        /// <param name="request">The export targets (results, crawled version, PDF report, AI detection, Grammar Checker and overview)</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <exception cref="CopyleaksHttpException">In case of reject from the server.</exception>
        /// <returns></returns>
        public async Task ExportAsync(string scanId, string exportId, ExportRequest request, string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(scanId))
                throw new ArgumentException("Mandatory", nameof(scanId));
            if (string.IsNullOrEmpty(exportId))
                throw new ArgumentException("Mandatory", nameof(exportId));
            if (request == null)
                throw new ArgumentException("Mandatory", nameof(request));
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Mandatory", nameof(token));

            string requestUri = $"{this.CopyleaksApiServer}v3/downloads/{PathSegment(scanId)}/export/{PathSegment(exportId)}";
            await SendJsonAndEnsureSuccessAsync(
                HttpMethod.Post, requestUri, token, request, gzip: true, eRetryMode.RateLimitOnly, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Send the webhook of a finished scan again. Valid for scans that completed, failed, were indexed or were price-checked.
        /// The webhook is sent to the status webhook of the scan shortly after the request is accepted.
        /// </summary>
        /// <param name="scanId">The scan id</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <exception cref="CopyleaksHttpException">The scan is still running (400) or does not exist (404).</exception>
        public async Task ResendWebhookAsync(string scanId, string token, CancellationToken cancellationToken = default)
        {
            RequireScanId(scanId);
            RequireToken(token);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/{PathSegment(scanId)}/webhooks/resend";
            await SendAndEnsureSuccessAsync(
                HttpMethod.Post, requestUri, token, null, eRetryMode.RateLimitOnly, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a list of the supported file types.
        /// </summary>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>List of textal file types supported and list of file types supported using OCR </returns>
        public async Task<SupportedTypesResult> GetSupportedFileTypesAsync(CancellationToken cancellationToken = default)
        {
            string requestUri = $"{this.CopyleaksApiServer}v3/miscellaneous/supported-plagiarism-file-types";
            return await SendForJsonAsync<SupportedTypesResult>(
                HttpMethod.Get, requestUri, null, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get OCR Supported Langauges
        /// </summary>
        /// <param name="cancellationToken">Cancels the request</param>
        /// <returns>List of OCR Supported Langauges</returns>
        public async Task<string[]> GetOcrLanguageListAsync(CancellationToken cancellationToken = default)
        {
            string requestUri = $"{this.CopyleaksApiServer}v3/miscellaneous/ocr-languages-list";
            return await SendForJsonAsync<string[]>(
                HttpMethod.Get, requestUri, null, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the languages supported by cross-language plagiarism detection. No login is needed.
        /// Use the result language codes in <c>CrossLanguages.Languages</c>.
        /// </summary>
        /// <param name="cancellationToken">Cancels the request</param>
        public async Task<AllowedCrossLanguagesResult> GetAllowedCrossLanguagesAsync(CancellationToken cancellationToken = default)
        {
            string requestUri = $"{this.CopyleaksApiServer}v3/miscellaneous/allowed-cross-languages";
            return await SendForJsonAsync<AllowedCrossLanguagesResult>(
                HttpMethod.Get, requestUri, null, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Private Cloud Hub repository information: name, status, credits and masking policy.
        /// Requires the "Admin" or "Super Admin" role in the repository.
        /// </summary>
        /// <param name="repositoryId">The repository id, as shown in the Copyleaks Admin Dashboard</param>
        /// <param name="token">Login Token</param>
        /// <param name="cancellationToken">Cancels the request</param>
        public async Task<RepositoryInfoResponse> GetRepositoryInfoAsync(string repositoryId, string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(repositoryId))
                throw new ArgumentException("RepositoryId is mandatory", nameof(repositoryId));
            RequireToken(token);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/repositories/repository/{PathSegment(repositoryId)}/info";
            return await SendForJsonAsync<RepositoryInfoResponse>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the Copyleaks API release notes. No login is needed.
        /// </summary>
        /// <param name="cancellationToken">Cancels the request</param>
        public async Task<ReleaseNotesResponse> GetReleaseNotesAsync(CancellationToken cancellationToken = default)
        {
            string requestUri = $"{this.CopyleaksApiServer}v3/release-logs.json";
            return await SendForJsonAsync<ReleaseNotesResponse>(
                HttpMethod.Get, requestUri, null, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }
    }
}
