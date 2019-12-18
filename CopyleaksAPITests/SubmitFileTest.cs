using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Copyleaks.SDK.V3.API.Models.Responses;
using Copyleaks.SDK.V3.API.Models.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CopyleaksAPITests
{
    [TestClass]
    public class SubmitFileTest
    {
        public const string USER_EMAIL = "**Your Email**";
        public const string USER_KEY = "**Your Key**";
        public const string WebhooksHost = "**Webhooks host origin**";

        private HttpClient Client { get; set; }
        private CopyleaksIdentityApi IdentityClient { get; set; }
        private CopyleaksScansApi EducationAPIClient { get; set; }

        public SubmitFileTest()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,

            };

            Client = new HttpClient(handler);
            IdentityClient = new CopyleaksIdentityApi(Client);
            EducationAPIClient = new CopyleaksScansApi(eProduct.Education.ToString().ToLower(), Client);
        }
        [TestMethod]
        public async Task SUBMIT_FILE_TEST()
        {
            var LoginResposne = await IdentityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
            var authToken = LoginResposne.Token;

            var EducationBalance = await EducationAPIClient.CreditBalanceAsync(authToken).ConfigureAwait(false);

            var scanId = await SubmitEducationFileScanAsync(authToken).ConfigureAwait(false);

            var progress = await Policy.HandleResult<uint>((result) => result != 100)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(3))
                .ExecuteAsync(() => EducationAPIClient.ProgressAsync(scanId, authToken)).ConfigureAwait(false);

            Assert.IsTrue(progress == 100, "Scan Progress didn't hit the 100%");

            //downloads
            var pdfReport = await EducationAPIClient.DownloadPdfReportAsync(scanId, authToken).ConfigureAwait(false);
            var sources = await EducationAPIClient.DownloadSourceReportAsync(scanId, authToken).ConfigureAwait(false);
            var results = await EducationAPIClient.ResultAsync(scanId, authToken).ConfigureAwait(false);

            //sandbox scan always have at least one internet result.
            var downloadedResult = await EducationAPIClient.DownloadResultAsync(scanId, results.Results.Internet[0].Id, authToken).ConfigureAwait(false);

            await EducationAPIClient.DeleteAsync(new DeleteRequest
            {
                Scans = new DeleteScanItem[] { new DeleteScanItem { Id = scanId } },
                Purge = true
            }, authToken).ConfigureAwait(false);
        }

        private async Task<string> SubmitEducationFileScanAsync(string authToken)
        {
            // A unique scan ID for the scan
            // In case this scan ID already exists for this user Copyleaks API will return HTTP 409 Conflict result
            string scanId = Guid.NewGuid().ToString();
            string scannedText = "Hellow world";
            // Submit a file for scan in https://api.copyleaks.com
            await EducationAPIClient.SubmitFileAsync(scanId, new FileDocument
            {
                Base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(scannedText)),
                Filename = "text.txt",
                PropertiesSection = GetScanPropertiesByProduct(scanId, eProduct.Education)
            },
            authToken).ConfigureAwait(false);

            return scanId;
        }

        private ScanProperties GetScanPropertiesByProduct(string scanId, eProduct product)
        {
            ScanProperties scanProperties;
            if (product == eProduct.Businesses)
                scanProperties = new BusinessesScanProperties();
            else
            {
                scanProperties = new EducationScanProperties();
                ((EducationScanProperties)scanProperties).ReportSection.Create = true;
            }

            // The action to perform
            // Possible values:
            // 1. checkCredits - return the number of credits that will be consumed by the scan.
            //                   The Result of the request will be returned to the 'Completion' callback
            // 2. Scan - Scan the submitted text
            //           The Result of the request will be returned to the 'Completion' callback
            // 3. Index - Upload the submitted text to Copyleaks internal database to be compared against feture scans
            //            The Result of the request will be returned to the 'Completion' callback
            scanProperties.Action = eSubmitAction.Scan;
            scanProperties.Webhooks = new Webhooks
            {
                // Copyleaks API will POST the scan results to the 'completed' callback
                // See 'CompletedProcess' method for more details
                Status = new Uri($"{WebhooksHost}/{scanId}/{{status}}")
            };
            // Sandbox mode does not take any credits
            scanProperties.Sandbox = true;

            return scanProperties;
        }
    }
}
