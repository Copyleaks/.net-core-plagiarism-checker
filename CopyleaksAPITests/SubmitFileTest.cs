using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Copyleaks.SDK.V3.API.Models.Responses;
using Copyleaks.SDK.V3.API.Models.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CopyleaksAPITests
{
    /// <summary>Live API tests: set real credentials below, then run dotnet test --filter TestCategory=Live.</summary>
    [TestClass]
    [TestCategory("Live")]
    public class SubmitFileTest
    {
		public const string USER_EMAIL = "<EMAIL>";
		public const string USER_KEY = "<API KEY>";
		public const string WebhooksHost = "<WEBHOOK>";

		private HttpClient Client { get; set; }
        private CopyleaksIdentityApi IdentityClient { get; set; }
        private CopyleaksScansApi APIClient { get; set; }

        public SubmitFileTest()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,

            };

            Client = new HttpClient(handler);
            IdentityClient = new CopyleaksIdentityApi(Client);
            APIClient = new CopyleaksScansApi(Client);
        }

        [TestMethod]
        public async Task LOGIN()
        {
            var LoginResposne = await IdentityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task USER_USAGE_TEST()
        {
            var LoginResposne = await IdentityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
            var authToken = LoginResposne.Token;

            DateTime start = new DateTime(2020, 3, 11);
            start.ToString("dd-MM-yyyy");
            DateTime end = new DateTime(2020, 3, 15);
            end.ToString("dd-MM-yyyy");

            using (var stream = new MemoryStream())
            {
                await APIClient.GetUserUsageAsync(start, end, stream, authToken).ConfigureAwait(false);

                using (var sr = new StreamReader(stream))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var csv = await sr.ReadToEndAsync();
                    Console.WriteLine(csv);
                }
            }
        }


        [TestMethod]
        public async Task SUBMIT_FILE_TEST()
        {
            var LoginResposne = await IdentityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
            var authToken = LoginResposne.Token;

            var Balance = await APIClient.CreditBalanceAsync(authToken).ConfigureAwait(false);

            var scanId = await SubmitFileScanAsync(authToken).ConfigureAwait(false);

            uint progress = await APIClient.ProgressAsync(scanId, authToken).ConfigureAwait(false);
            for (int attempt = 0; attempt < 6 && progress != 100; attempt++)
            {
                await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
                progress = await APIClient.ProgressAsync(scanId, authToken).ConfigureAwait(false);
            }

            Assert.IsTrue(progress == 100, "Scan Progress didn't hit the 100%");

            var pdfReport = await APIClient.DownloadPdfReportAsync(scanId, authToken).ConfigureAwait(false);
            var sources = await APIClient.DownloadSourceReportAsync(scanId, authToken).ConfigureAwait(false);
            var results = await APIClient.ResultAsync(scanId, authToken).ConfigureAwait(false);

            // A sandbox scan always has at least one internet result.
            var downloadedResult = await APIClient.DownloadResultAsync(scanId, results.Results.Internet[0].Id, authToken).ConfigureAwait(false);

            await APIClient.DeleteAsync(new DeleteRequest
            {
                Scans = new DeleteScanItem[] { new DeleteScanItem { Id = scanId } },
                Purge = true
            }, authToken).ConfigureAwait(false);
        }

        private async Task<string> SubmitFileScanAsync(string authToken)
        {
            // Must be unique: a scan id that already exists for this user gets HTTP 409 Conflict.
            string scanId = Guid.NewGuid().ToString();
            string scannedText = "Hellow world";
            await APIClient.SubmitFileAsync(scanId, new FileDocument
            {
                Base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(scannedText)),
                Filename = "text.txt",
                PropertiesSection = GetScanProperties(scanId)
            },
            authToken).ConfigureAwait(false);

            return scanId;
        }

        private ScanProperties GetScanProperties(string scanId)
        {
            ClientScanProperties clientScanProperties = new ClientScanProperties();

			// Scan, checkCredits (only count the credits it would use) or Index (only store it for future scans).
			// Each one reports to the 'Completion' callback.
			clientScanProperties.Action = eSubmitAction.Scan;
			clientScanProperties.Webhooks = new Webhooks
            {
                // Copyleaks API POSTs the scan results to the status webhook.
                Status = new Uri($"{WebhooksHost}/{scanId}/{{status}}")
            };

			// Sandbox mode does not take any credits
			clientScanProperties.Sandbox = true;

			clientScanProperties.ReportSection.Create = true;

			return clientScanProperties;
        }
    }
}
