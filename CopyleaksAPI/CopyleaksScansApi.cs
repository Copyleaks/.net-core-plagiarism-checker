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
    /// Plagiarism scans: submit, start, progress, results, reports, export and delete.
    /// A <c>token</c> argument is the login token from <c>CopyleaksIdentityApi.LoginAsync</c>.
    /// </summary>
    public class CopyleaksScansApi : CopyleaksBase
    {
        private static readonly HttpMethod Patch = new HttpMethod("PATCH");

        public string CopyleaksApiServer { get; private set; }

        /// <summary>Uses the given HttpClient.</summary>
        /// <param name="client">For example one from IHttpClientFactory. The SDK never disposes it.</param>
        /// <param name="options">Optional timeout and retry settings.</param>
        public CopyleaksScansApi(HttpClient client, CopyleaksClientOptions options = null) : base(client, options)
        {
            SetUpService();
        }

        /// <summary>Uses the SDK's shared HttpClient, with an optional client certificate and options.</summary>
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

        /// <summary>Get the current credit balance.</summary>
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

        /// <summary>Submit a URL for a plagiarism scan.</summary>
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

        /// <summary>Submit a file or free text, base64-encoded, for a plagiarism scan.</summary>
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
        /// Submit a file from a stream for a plagiarism scan; use it for large files, as no base64 copy is held in memory.
        /// <paramref name="filename"/> includes the extension, for example "file.pdf". The stream is read asynchronously
        /// to its end (a request body stream works) and is not disposed.
        /// </summary>
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

        /// <summary>Submit an image with text, base64-encoded, for a plagiarism scan (OCR).</summary>
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
        /// Submit an image with text from a stream for a plagiarism scan (OCR), base64-encoded while it is read.
        /// <paramref name="filename"/> includes the extension; <paramref name="language"/> is the language code of the text.
        /// The stream is read asynchronously to its end and is not disposed.
        /// </summary>
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

        /// <summary>Get the scan progress, in percent.</summary>
        public async Task<uint> ProgressAsync(string scanId, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            RequireScanId(scanId);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/{PathSegment(scanId)}/progress";
            var progress = await SendForJsonAsync<ProgressResponse>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
            return progress.Percents;
        }

        /// <summary>Delete finished scans. Deletion can take a few minutes.</summary>
        /// <param name="model">The scans to delete, and an optional completion webhook.</param>
        /// <param name="token">Login token.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        public async Task DeleteAsync(DeleteRequest model, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}.1/scans/delete";
            await SendJsonAndEnsureSuccessAsync(
                Patch, requestUri, token, model, gzip: true, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Start scans in 'price checked' status.</summary>
        /// <returns>The scans that started and the scans that failed to start.</returns>
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

        /// <summary>Start 'price checked' scans as a batch, also compared against its included completed scans.</summary>
        /// <returns>The scans that started and the scans that failed to start.</returns>
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

        /// <summary>Get the results of a completed scan.</summary>
        public async Task<Result> ResultAsync(string scanId, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            RequireScanId(scanId);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/{PathSegment(scanId)}/result";
            return await SendForJsonAsync<Result>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Get the comparison report of one suspected result.</summary>
        /// <param name="scanId">Scan id.</param>
        /// <param name="resultId">Result id, for example from <see cref="ResultAsync"/>.</param>
        /// <param name="token">Login token.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        public async Task<DownloadResultResponse> DownloadResultAsync(string scanId, string resultId, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            RequireScanId(scanId);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/downloads/{PathSegment(scanId)}/results/{PathSegment(resultId)}";
            return await SendForJsonAsync<DownloadResultResponse>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Write the usage history CSV for a date range into <paramref name="stream"/> as it arrives.
        /// <see cref="CopyleaksClientOptions.Timeout"/> covers the whole download.
        /// </summary>
        /// <exception cref="CopyleaksHttpException">The server rejected the request.</exception>
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

        /// <summary>Get the comparison report of your submitted content.</summary>
        public async Task<DownloadSourceResponse> DownloadSourceReportAsync(string scanId, string token, CancellationToken cancellationToken = default)
        {
            RequireToken(token);
            RequireScanId(scanId);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/downloads/{PathSegment(scanId)}";
            return await SendForJsonAsync<DownloadSourceResponse>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Download the PDF report. <see cref="CopyleaksClientOptions.Timeout"/> covers the whole download.</summary>
        /// <returns>The PDF, as a stream positioned at its start.</returns>
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

        /// <summary>Export several items of a scan in one call; <paramref name="exportId"/> is your tracking id.</summary>
        /// <exception cref="CopyleaksHttpException">The server rejected the request.</exception>
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
        /// Resend the webhook of a finished scan (completed, failed, indexed or price-checked) to its status webhook,
        /// shortly after the request is accepted.
        /// </summary>
        /// <exception cref="CopyleaksHttpException">The scan is still running (400) or does not exist (404).</exception>
        public async Task ResendWebhookAsync(string scanId, string token, CancellationToken cancellationToken = default)
        {
            RequireScanId(scanId);
            RequireToken(token);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/scans/{PathSegment(scanId)}/webhooks/resend";
            await SendAndEnsureSuccessAsync(
                HttpMethod.Post, requestUri, token, null, eRetryMode.RateLimitOnly, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Get the supported file types, for text and for OCR.</summary>
        public async Task<SupportedTypesResult> GetSupportedFileTypesAsync(CancellationToken cancellationToken = default)
        {
            string requestUri = $"{this.CopyleaksApiServer}v3/miscellaneous/supported-plagiarism-file-types";
            return await SendForJsonAsync<SupportedTypesResult>(
                HttpMethod.Get, requestUri, null, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Get the languages supported by OCR.</summary>
        public async Task<string[]> GetOcrLanguageListAsync(CancellationToken cancellationToken = default)
        {
            string requestUri = $"{this.CopyleaksApiServer}v3/miscellaneous/ocr-languages-list";
            return await SendForJsonAsync<string[]>(
                HttpMethod.Get, requestUri, null, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Get the languages for <c>CrossLanguages.Languages</c>. No login needed.</summary>
        public async Task<AllowedCrossLanguagesResult> GetAllowedCrossLanguagesAsync(CancellationToken cancellationToken = default)
        {
            string requestUri = $"{this.CopyleaksApiServer}v3/miscellaneous/allowed-cross-languages";
            return await SendForJsonAsync<AllowedCrossLanguagesResult>(
                HttpMethod.Get, requestUri, null, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Private Cloud Hub repository info: name, status, credits and masking policy.
        /// Requires the "Admin" or "Super Admin" role in the repository.
        /// </summary>
        /// <param name="repositoryId">Repository id, as shown in the Copyleaks Admin Dashboard.</param>
        /// <param name="token">Login token.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        public async Task<RepositoryInfoResponse> GetRepositoryInfoAsync(string repositoryId, string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(repositoryId))
                throw new ArgumentException("RepositoryId is mandatory", nameof(repositoryId));
            RequireToken(token);

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/repositories/repository/{PathSegment(repositoryId)}/info";
            return await SendForJsonAsync<RepositoryInfoResponse>(
                HttpMethod.Get, requestUri, token, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Get the Copyleaks API release notes. No login needed.</summary>
        public async Task<ReleaseNotesResponse> GetReleaseNotesAsync(CancellationToken cancellationToken = default)
        {
            string requestUri = $"{this.CopyleaksApiServer}v3/release-logs.json";
            return await SendForJsonAsync<ReleaseNotesResponse>(
                HttpMethod.Get, requestUri, null, null, eRetryMode.Safe, cancellationToken).ConfigureAwait(false);
        }
    }
}
