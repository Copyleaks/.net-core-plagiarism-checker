using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.AIDetection;
using Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection;
using Copyleaks.SDK.V3.API.Models.Requests.AiVideoDetection;
using Copyleaks.SDK.V3.API.Models.Requests.ImagePlagiarism;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Copyleaks.SDK.V3.API.Models.Requests.Shared;
using Copyleaks.SDK.V3.API.Models.Requests.TextModeration;
using Copyleaks.SDK.V3.API.Models.Requests.WritingAssistant;
using Copyleaks.SDK.V3.API.Models.Types;
using CopyleaksAPITests.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace CopyleaksAPITests
{
    /// <summary>Offline tests of every endpoint: the exact method, URL, headers and body the SDK sends.</summary>
    [TestClass]
    public class EndpointTests
    {
        private const string Api = "https://api.copyleaks.com";
        private const string Webhook = "https://example.com/webhook/{STATUS}";

        private static (FakeHttpHandler Handler, HttpClient Client) Server(HttpStatusCode status, string json = null)
        {
            var handler = new FakeHttpHandler().Respond(status, json);
            return (handler, Fakes.Client(handler));
        }

        private static void AssertRequest(RecordedRequest request, HttpMethod method, string url, bool authorized = true)
        {
            Assert.AreEqual(method, request.Method);
            Assert.AreEqual(url, request.Uri.ToString());
            if (authorized)
                Assert.AreEqual($"Bearer {Fakes.Token}", request.Header("Authorization"));
            else
                Assert.IsNull(request.Header("Authorization"));
            StringAssert.StartsWith(request.Header("User-Agent"), "copyleaks-core-sdk/");
        }

        private static void AssertJsonBody(RecordedRequest request, string expectedJson, bool gzip)
        {
            Assert.AreEqual(gzip, request.IsGzip, "Content-Encoding gzip");
            StringAssert.StartsWith(request.ContentHeaders["Content-Type"], "application/json");
            Assert.AreEqual(request.RawBody.Length, request.ContentLength, "Content-Length");
            var expected = JToken.Parse(expectedJson);
            Assert.IsTrue(JToken.DeepEquals(expected, request.Json), $"Body was {request.BodyText}");
        }

        // Identity
        [TestMethod]
        public async Task Login()
        {
            var (handler, client) = Server(HttpStatusCode.OK, "{\"access_token\":\"abc\",\".issued\":\"2026-01-01T00:00:00Z\",\".expires\":\"2026-01-03T00:00:00Z\"}");

            var login = await new CopyleaksIdentityApi(client).LoginAsync("me@example.com", "key-1");

            Assert.AreEqual("abc", login.Token);
            var request = handler.Single;
            AssertRequest(request, HttpMethod.Post, "https://id.copyleaks.com/v3/account/login/api", authorized: false);
            AssertJsonBody(request, "{\"email\":\"me@example.com\",\"key\":\"key-1\"}", gzip: true);
        }

        // Scans
        [TestMethod]
        public async Task Credit_balance()
        {
            var (handler, client) = Server(HttpStatusCode.OK, "{\"Amount\":7}");
            Assert.AreEqual(7u, await new CopyleaksScansApi(client).CreditBalanceAsync(Fakes.Token));
            AssertRequest(handler.Single, HttpMethod.Get, $"{Api}/v3/scans/credits");
            Assert.IsNull(handler.Single.Body);
        }

        [TestMethod]
        public async Task Submit_url_with_verb_and_headers()
        {
            var (handler, client) = Server(HttpStatusCode.Created);
            var document = new UrlDocument
            {
                Url = new Uri("https://example.com/page"),
                Verb = "POST",
                Headers = new string[,] { { "X-Key", "value" } },
                PropertiesSection = new ClientScanProperties { Sandbox = true, Webhooks = new Webhooks { Status = new Uri(Webhook) } },
            };

            await new CopyleaksScansApi(client).SubmitUrlAsync("url1", document, Fakes.Token);

            var request = handler.Single;
            AssertRequest(request, HttpMethod.Put, $"{Api}/v3/scans/submit/url/url1");
            Assert.IsTrue(request.IsGzip);
            Assert.AreEqual("https://example.com/page", request.Json["url"].Value<string>());
            Assert.AreEqual("POST", request.Json["verb"].Value<string>());
            Assert.AreEqual("[[\"X-Key\",\"value\"]]", request.Json["headers"].ToString(Newtonsoft.Json.Formatting.None));
            Assert.AreEqual(Webhook, request.Json["properties"]["webhooks"]["status"].Value<string>());
        }

        [TestMethod]
        public async Task Submit_url_omits_verb_when_unset()
        {
            var (handler, client) = Server(HttpStatusCode.Created);
            await new CopyleaksScansApi(client).SubmitUrlAsync("url2", new UrlDocument { Url = new Uri("https://example.com/"), PropertiesSection = new ClientScanProperties() }, Fakes.Token);
            Assert.IsNull(handler.Single.Json["verb"]);
        }

        [TestMethod]
        public async Task Submit_file()
        {
            var (handler, client) = Server(HttpStatusCode.Created);
            var document = new FileDocument { Base64 = "SGVsbG8=", Filename = "hello.txt", PropertiesSection = new ClientScanProperties { Sandbox = true } };

            await new CopyleaksScansApi(client).SubmitFileAsync("file1", document, Fakes.Token);

            var request = handler.Single;
            AssertRequest(request, HttpMethod.Put, $"{Api}/v3/scans/submit/file/file1");
            Assert.AreEqual("SGVsbG8=", request.Json["base64"].Value<string>());
            Assert.AreEqual("hello.txt", request.Json["filename"].Value<string>());
            Assert.IsTrue(request.Json["properties"]["sandbox"].Value<bool>());
        }

        [TestMethod]
        public async Task Submit_ocr()
        {
            var (handler, client) = Server(HttpStatusCode.Created);
            var document = new FileOcrDocument { Base64 = "iVBORw0K", Filename = "page.png", Language = "en", PropertiesSection = new ClientScanProperties { Sandbox = true } };

            await new CopyleaksScansApi(client).SubmitImageOCRAsync("ocr1", document, Fakes.Token);

            var request = handler.Single;
            AssertRequest(request, HttpMethod.Put, $"{Api}/v3/scans/submit/ocr/ocr1");
            Assert.AreEqual("en", request.Json["langCode"].Value<string>());
            Assert.AreEqual("iVBORw0K", request.Json["base64"].Value<string>());
        }

        [TestMethod]
        public async Task Progress_result_and_downloads()
        {
            var handler = new FakeHttpHandler()
                .Respond(HttpStatusCode.OK, "{\"percents\":55}")
                .Respond(HttpStatusCode.OK, "{}")
                .Respond(HttpStatusCode.OK, "{}")
                .Respond(HttpStatusCode.OK, "{\"version\":3}")
                .RespondBytes(HttpStatusCode.OK, Encoding.ASCII.GetBytes("%PDF-1.7"), "application/pdf");
            var api = new CopyleaksScansApi(Fakes.Client(handler));

            Assert.AreEqual(55u, await api.ProgressAsync("s1", Fakes.Token));
            await api.ResultAsync("s1", Fakes.Token);
            await api.DownloadResultAsync("s1", "r1", Fakes.Token);
            Assert.AreEqual(3, (await api.DownloadSourceReportAsync("s1", Fakes.Token)).Version);
            using (var pdf = await api.DownloadPdfReportAsync("s1", Fakes.Token))
            using (var reader = new StreamReader(pdf))
                Assert.AreEqual("%PDF-1.7", reader.ReadToEnd());

            var requests = handler.RequestList;
            AssertRequest(requests[0], HttpMethod.Get, $"{Api}/v3/scans/s1/progress");
            AssertRequest(requests[1], HttpMethod.Get, $"{Api}/v3/scans/s1/result");
            AssertRequest(requests[2], HttpMethod.Get, $"{Api}/v3/downloads/s1/results/r1");
            AssertRequest(requests[3], HttpMethod.Get, $"{Api}/v3/downloads/s1");
            AssertRequest(requests[4], HttpMethod.Get, $"{Api}/v3/downloads/s1/report.pdf");
        }

        [TestMethod]
        public async Task Usage_history_uses_invariant_dates_and_copies_the_csv()
        {
            var handler = new FakeHttpHandler().RespondBytes(HttpStatusCode.OK, Encoding.UTF8.GetBytes("a,b\n1,2\n"), "text/csv");
            var api = new CopyleaksScansApi(Fakes.Client(handler));
            var previous = System.Globalization.CultureInfo.CurrentCulture;
            try
            {
                // A culture with a non-Gregorian default calendar must not change the dates.
                System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("th-TH");
                using (var csv = new MemoryStream())
                {
                    await api.GetUserUsageAsync(new DateTime(2026, 1, 2), new DateTime(2026, 1, 9), csv, Fakes.Token);
                    Assert.AreEqual("a,b\n1,2\n", Encoding.UTF8.GetString(csv.ToArray()));
                }
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentCulture = previous;
            }
            AssertRequest(handler.Single, HttpMethod.Get, $"{Api}/v3/scans/usages/history?start=02-01-2026&end=09-01-2026");
        }

        [TestMethod]
        public async Task Delete_with_completion_webhook_and_headers()
        {
            var (handler, client) = Server(HttpStatusCode.Accepted);
            await new CopyleaksScansApi(client).DeleteAsync(new DeleteRequest
            {
                Scans = new[] { new DeleteScanItem { Id = "s1" }, new DeleteScanItem { Id = "s2" } },
                Purge = true,
                CompletionWebhook = new Uri("https://example.com/deleted"),
                Headers = new string[,] { { "key", "value" } },
            }, Fakes.Token);

            var request = handler.Single;
            AssertRequest(request, new HttpMethod("PATCH"), $"{Api}/v3.1/scans/delete");
            AssertJsonBody(request,
                "{\"scans\":[{\"id\":\"s1\"},{\"id\":\"s2\"}],\"purge\":true,\"completionWebhook\":\"https://example.com/deleted\",\"headers\":[[\"key\",\"value\"]]}",
                gzip: true);
        }

        [TestMethod]
        public async Task Delete_without_webhook_sends_only_scans_and_purge()
        {
            var (handler, client) = Server(HttpStatusCode.Accepted);
            await new CopyleaksScansApi(client).DeleteAsync(new DeleteRequest { Scans = new[] { new DeleteScanItem { Id = "s1" } } }, Fakes.Token);
            AssertJsonBody(handler.Single, "{\"scans\":[{\"id\":\"s1\"}],\"purge\":false}", gzip: true);
        }

        [TestMethod]
        public async Task Start_and_batch_start()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.OK, "{\"success\":[{\"id\":\"s1\"}],\"failed\":[]}");
            var api = new CopyleaksScansApi(Fakes.Client(handler));

            var started = await api.StartAsync(new StartRequest { Trigger = new[] { "s1" }, ErrorHandlingMode = eErrorHandling.Ignore }, Fakes.Token);
            await api.StartBatchAsync(new StartBatchRequest { Trigger = new[] { "s1" }, Include = new[] { "old1" } }, Fakes.Token);

            Assert.AreEqual("s1", started.Success[0].Id);
            var requests = handler.RequestList;
            AssertRequest(requests[0], new HttpMethod("PATCH"), $"{Api}/v3/scans/start");
            AssertJsonBody(requests[0], "{\"trigger\":[\"s1\"],\"errorHandling\":1}", gzip: true);
            AssertRequest(requests[1], new HttpMethod("PATCH"), $"{Api}/v3/scans/batch/start");
            AssertJsonBody(requests[1], "{\"include\":[\"old1\"],\"trigger\":[\"s1\"],\"errorHandling\":0}", gzip: true);
        }

        [TestMethod]
        public async Task Export_with_new_targets_and_no_null_fields()
        {
            var (handler, client) = Server(HttpStatusCode.NoContent);
            ExportTask Target(string path) => new ExportTask { Verb = "PUT", Endpoint = new Uri($"https://example.com/export/{path}") };

            await new CopyleaksScansApi(client).ExportAsync("scan1", "exp1", new ExportRequest
            {
                CompletionWebhook = new Uri("https://example.com/export/done"),
                CompletionWebhookHeaders = new string[,] { { "Authorization", "secret" } },
                AIDetection = Target("ai"),
                WritingFeedback = Target("grammar"),
                Overview = Target("overview"),
            }, Fakes.Token);

            var request = handler.Single;
            AssertRequest(request, HttpMethod.Post, $"{Api}/v3/downloads/scan1/export/exp1");
            AssertJsonBody(request, @"{
                ""completionWebhook"":""https://example.com/export/done"",
                ""completionWebhookHeaders"":[[""Authorization"",""secret""]],
                ""aiDetection"":{""verb"":""PUT"",""endpoint"":""https://example.com/export/ai""},
                ""writingFeedback"":{""verb"":""PUT"",""endpoint"":""https://example.com/export/grammar""},
                ""overview"":{""verb"":""PUT"",""endpoint"":""https://example.com/export/overview""}
            }", gzip: true);
        }

        [TestMethod]
        public async Task Export_results_crawled_version_and_pdf()
        {
            var (handler, client) = Server(HttpStatusCode.NoContent);
            await new CopyleaksScansApi(client).ExportAsync("scan1", "exp2", new ExportRequest
            {
                CompletionWebhook = new Uri("https://example.com/done"),
                MaxRetries = 4,
                DeveloperPayload = "dp",
                Results = new[] { new ResultExportTask { ResultId = "r1", Verb = "POST", Endpoint = new Uri("https://example.com/r1"), Headers = new string[,] { { "a", "b" } } } },
                CrawledVersion = new ExportTask { Verb = "POST", Endpoint = new Uri("https://example.com/crawled") },
                PdfReport = new ExportTask { Verb = "POST", Endpoint = new Uri("https://example.com/pdf") },
            }, Fakes.Token);

            AssertJsonBody(handler.Single, @"{
                ""completionWebhook"":""https://example.com/done"",
                ""maxRetries"":4,
                ""developerPayload"":""dp"",
                ""results"":[{""id"":""r1"",""verb"":""POST"",""endpoint"":""https://example.com/r1"",""headers"":[[""a"",""b""]]}],
                ""crawledVersion"":{""verb"":""POST"",""endpoint"":""https://example.com/crawled""},
                ""pdfReport"":{""verb"":""POST"",""endpoint"":""https://example.com/pdf""}
            }", gzip: true);
        }

        [TestMethod]
        public async Task Resend_webhook()
        {
            var (handler, client) = Server(HttpStatusCode.Accepted, "{}");

            await new CopyleaksScansApi(client).ResendWebhookAsync("scan-9", Fakes.Token);

            var request = handler.Single;
            AssertRequest(request, HttpMethod.Post, $"{Api}/v3/scans/scan-9/webhooks/resend");
            Assert.IsTrue(request.RawBody == null || request.RawBody.Length == 0, "resend has no body");
        }

        [TestMethod]
        public async Task Resend_webhook_requires_scan_id_and_token()
        {
            var api = new CopyleaksScansApi(Fakes.Client(new FakeHttpHandler()));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => api.ResendWebhookAsync("", Fakes.Token));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => api.ResendWebhookAsync("scan1", ""));
        }

        [TestMethod]
        public async Task Submits_require_a_scan_id()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.Created, "{}");
            var client = Fakes.Client(handler);
            var scans = new CopyleaksScansApi(client);
            var aiText = new CopyleaksAIDetectionApi(client);
            var grammar = new CopyleaksWritingAssistantApi(client);
            var properties = new ClientScanProperties { Sandbox = true };

            foreach (string scanId in new[] { null, "" })
            {
                await Assert.ThrowsExceptionAsync<ArgumentException>(() => scans.SubmitUrlAsync(scanId,
                    new UrlDocument { Url = new Uri("https://example.com/"), PropertiesSection = properties }, Fakes.Token));
                await Assert.ThrowsExceptionAsync<ArgumentException>(() => scans.SubmitFileAsync(scanId,
                    new FileDocument { Base64 = "aGk=", Filename = "a.txt", PropertiesSection = properties }, Fakes.Token));
                await Assert.ThrowsExceptionAsync<ArgumentException>(() => scans.SubmitFileAsync(scanId,
                    new MemoryStream(new byte[] { 1 }), "a.txt", properties, Fakes.Token));
                await Assert.ThrowsExceptionAsync<ArgumentException>(() => scans.SubmitImageOCRAsync(scanId,
                    new FileOcrDocument { Base64 = "aGk=", Filename = "a.png", Language = "en", PropertiesSection = properties }, Fakes.Token));
                await Assert.ThrowsExceptionAsync<ArgumentException>(() => scans.SubmitImageOCRAsync(scanId,
                    new MemoryStream(new byte[] { 1 }), "a.png", "en", properties, Fakes.Token));
                await Assert.ThrowsExceptionAsync<ArgumentException>(() => aiText.SubmitNaturalLanguageAsync(scanId,
                    new NaturalLanguageDocument { Text = new string('a', 300), Sandbox = true }, Fakes.Token));
                await Assert.ThrowsExceptionAsync<ArgumentException>(() => grammar.SubmitTextAsync(scanId,
                    new WritingAssistantDocument { Text = "Some text", Sandbox = true }, Fakes.Token));
            }

            Assert.AreEqual(0, handler.RequestList.Count, "an empty scan id must never reach the server");
        }

        [TestMethod]
        public async Task Supported_file_types_and_ocr_languages_use_the_documented_v3_paths()
        {
            var handler = new FakeHttpHandler()
                .Respond(HttpStatusCode.OK, "{\"textual\":[\"pdf\",\"docx\"],\"ocr\":[\"png\"]}")
                .Respond(HttpStatusCode.OK, "[\"en\",\"fr\"]");
            var api = new CopyleaksScansApi(Fakes.Client(handler));

            var types = await api.GetSupportedFileTypesAsync();
            var languages = await api.GetOcrLanguageListAsync();

            CollectionAssert.AreEqual(new[] { "pdf", "docx" }, types.Textual);
            CollectionAssert.AreEqual(new[] { "en", "fr" }, languages);
            AssertRequest(handler.RequestList[0], HttpMethod.Get, $"{Api}/v3/miscellaneous/supported-plagiarism-file-types", authorized: false);
            AssertRequest(handler.RequestList[1], HttpMethod.Get, $"{Api}/v3/miscellaneous/ocr-languages-list", authorized: false);
        }

        [TestMethod]
        public async Task Allowed_cross_languages_reads_the_server_object_shape()
        {
            var (handler, client) = Server(HttpStatusCode.OK,
                "{\"documentLanguages\":{\"english\":\"en\",\"spanish\":\"es\"},\"resultLanguages\":{\"albanian\":\"sq\",\"english\":\"en\",\"vietnamese\":\"vi\"}}");

            var languages = await new CopyleaksScansApi(client).GetAllowedCrossLanguagesAsync();

            AssertRequest(handler.Single, HttpMethod.Get, $"{Api}/v3/miscellaneous/allowed-cross-languages", authorized: false);
            Assert.AreEqual("en", languages.DocumentLanguages["english"]);
            CollectionAssert.AreEquivalent(new[] { "en", "es" }, languages.DocumentLanguageCodes);
            CollectionAssert.AreEquivalent(new[] { "sq", "en", "vi" }, languages.ResultLanguageCodes);
        }

        [TestMethod]
        public async Task Allowed_cross_languages_also_reads_an_array_of_codes()
        {
            var (_, client) = Server(HttpStatusCode.OK, "{\"documentLanguages\":[\"da\",\"en\"],\"resultLanguages\":[\"sq\"]}");

            var languages = await new CopyleaksScansApi(client).GetAllowedCrossLanguagesAsync();

            CollectionAssert.AreEquivalent(new[] { "da", "en" }, languages.DocumentLanguageCodes);
            CollectionAssert.AreEquivalent(new[] { "sq" }, languages.ResultLanguageCodes);
        }

        [TestMethod]
        public async Task Repository_info()
        {
            var (handler, client) = Server(HttpStatusCode.OK,
                "{\"id\":\"repo-1\",\"name\":\"Hub\",\"description\":\"d\",\"permission\":4,\"status\":0,\"maxCredits\":1000,\"currentCredits\":900,\"maskingPolicy\":2,\"creationTime\":\"2024-09-09T10:43:52\"}");

            var info = await new CopyleaksScansApi(client).GetRepositoryInfoAsync("repo-1", Fakes.Token);

            AssertRequest(handler.Single, HttpMethod.Get, $"{Api}/v3/repositories/repository/repo-1/info");
            Assert.AreEqual("Hub", info.Name);
            Assert.AreEqual(4, info.Permission);
            Assert.AreEqual(0, info.Status);
            Assert.AreEqual(1000, info.MaxCredits);
            Assert.AreEqual(900, info.CurrentCredits);
            Assert.AreEqual(eMaskingPolicy.MaskAllFiles, info.MaskingPolicy);
            Assert.AreEqual(new DateTime(2024, 9, 9, 10, 43, 52), info.CreationTime);
        }

        [TestMethod]
        public async Task Release_notes()
        {
            var (handler, client) = Server(HttpStatusCode.OK,
                "{\"updates\":[{\"msgType\":\"NEW\",\"date\":\"2023-12-07T09:00:00Z\",\"htmlDescription\":\"<b>New</b>\"}]}");

            var notes = await new CopyleaksScansApi(client).GetReleaseNotesAsync();

            AssertRequest(handler.Single, HttpMethod.Get, $"{Api}/v3/release-logs.json", authorized: false);
            Assert.AreEqual(1, notes.Updates.Length);
            Assert.AreEqual("NEW", notes.Updates[0].MsgType);
            Assert.AreEqual(new DateTimeOffset(2023, 12, 7, 9, 0, 0, TimeSpan.Zero), notes.Updates[0].Date);
            Assert.AreEqual("<b>New</b>", notes.Updates[0].HtmlDescription);
        }

        // AI text detection
        [TestMethod]
        public async Task Ai_text_detection_with_explain_and_sensitivity()
        {
            var (handler, client) = Server(HttpStatusCode.OK, "{\"modelVersion\":\"v11\",\"summary\":{\"human\":0.1,\"ai\":0.9},\"explain\":{\"patterns\":{\"statistics\":{\"aiCount\":[1.5]}}}}");
            var document = new NaturalLanguageDocument { Text = "text", Sandbox = true, Language = "en", Explain = true, Sensitivity = 3 };

            var result = await new CopyleaksAIDetectionApi(client).SubmitNaturalLanguageAsync("ai1", document, Fakes.Token);

            AssertRequest(handler.Single, HttpMethod.Post, $"{Api}/v2/writer-detector/ai1/check");
            AssertJsonBody(handler.Single, "{\"text\":\"text\",\"sandbox\":true,\"language\":\"en\",\"explain\":true,\"sensitivity\":3}", gzip: true);
            Assert.AreEqual(1.5, result.Explain.Patterns.Statistics.AICount[0]);
        }

        [TestMethod]
        public async Task Ai_text_detection_omits_explain_and_sensitivity_when_unset()
        {
            var (handler, client) = Server(HttpStatusCode.OK, "{\"summary\":{\"human\":1,\"ai\":0}}");
            await new CopyleaksAIDetectionApi(client).SubmitNaturalLanguageAsync("ai2", new NaturalLanguageDocument { Text = "t" }, Fakes.Token);
            AssertJsonBody(handler.Single, "{\"text\":\"t\",\"sandbox\":false,\"language\":null}", gzip: true);
        }

        [TestMethod]
        public async Task Ai_text_credits()
        {
            var (handler, client) = Server(HttpStatusCode.OK, "{\"credits\":382}");
            Assert.AreEqual(382u, await new CopyleaksAIDetectionApi(client).CreditBalanceAsync(Fakes.Token));
            AssertRequest(handler.Single, HttpMethod.Get, $"{Api}/v2/writer-detector/credits");
        }

        [TestMethod]
        public async Task Ai_text_supported_file_types()
        {
            var (handler, client) = Server(HttpStatusCode.OK, "{\"supportedAiFileTypes\":[\"pdf\",\"docx\",\"LaTeX\"]}");

            var types = await new CopyleaksAIDetectionApi(client).GetSupportedFileTypesAsync();

            AssertRequest(handler.Single, HttpMethod.Get, $"{Api}/v3/miscellaneous/supported-ai-text-detection-file-types", authorized: false);
            CollectionAssert.AreEqual(new[] { "pdf", "docx", "LaTeX" }, types.SupportedAiFileTypes);
        }

        // Grammar Checker
        [TestMethod]
        public async Task Grammar_checker_sends_default_weights_of_one()
        {
            var (handler, client) = Server(HttpStatusCode.OK, "{\"scanType\":\"sandbox\",\"score\":{\"corrections\":{\"overallScore\":90}}}");
            var document = new WritingAssistantDocument { Text = "Some text", Sandbox = true, Score = new Score { GrammarScoreWeight = 0.5 } };

            var result = await new CopyleaksWritingAssistantApi(client).SubmitTextAsync("wa1", document, Fakes.Token);

            Assert.AreEqual(90, result.Score.Corrections.OverallScore);
            AssertRequest(handler.Single, HttpMethod.Post, $"{Api}/v1/writing-feedback/wa1/check");
            AssertJsonBody(handler.Single,
                "{\"text\":\"Some text\",\"sandbox\":true,\"score\":{\"grammarScoreWeight\":0.5,\"mechanicsScoreWeight\":1.0,\"sentenceStructureScoreWeight\":1.0,\"wordChoiceScoreWeight\":1.0}}",
                gzip: true);
        }

        [TestMethod]
        public async Task Correction_types_are_public()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.OK, "{\"correctionTypes\":[{\"id\":1,\"title\":\"General\",\"category\":2}]}");
            var api = new CopyleaksWritingAssistantApi(Fakes.Client(handler));

            var types = await api.GetCorrectionTypesAsync("en");
            await api.GetCorrectionTypesAsync(Fakes.Token, "en");

            Assert.AreEqual("General", types.CorrectionType[0].Title);
            AssertRequest(handler.RequestList[0], HttpMethod.Get, $"{Api}/v1/writing-feedback/correction-types/en", authorized: false);
            AssertRequest(handler.RequestList[1], HttpMethod.Get, $"{Api}/v1/writing-feedback/correction-types/en", authorized: true);
        }

        // Text Moderation
        [TestMethod]
        public async Task Text_moderation()
        {
            var (handler, client) = Server(HttpStatusCode.OK, "{\"moderations\":{\"text\":{\"chars\":{\"starts\":[0],\"labels\":[1],\"lengths\":[4]}}},\"legend\":[{\"index\":1,\"id\":\"toxic-v1\"}]}");
            var model = new CopyleaksTextModerationRequestModel("bad text", true, CopyleaksTextModerationLanguages.ENGLISH,
                new object[] { new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.TOXIC_V1) });

            var result = await new CopyleaksTextModerationApi(client).SubmitTextAsync("tm1", model, Fakes.Token);

            Assert.AreEqual("toxic-v1", result.Legend[0].Id);
            AssertRequest(handler.Single, HttpMethod.Post, $"{Api}/v1/text-moderation/tm1/check");
            AssertJsonBody(handler.Single, "{\"text\":\"bad text\",\"sandbox\":true,\"language\":\"en\",\"labels\":[{\"id\":\"toxic-v1\"}]}", gzip: false);
        }

        [TestMethod]
        public void Text_moderation_labels_are_required_and_capped_at_16()
        {
            object[] Labels(int count) => Enumerable.Range(0, count).Select(i => (object)new CopyleaksTextModerationLabel($"label-{i}")).ToArray();

            Assert.AreEqual(16, CopyleaksTextModerationRequestModel.MaxLabels);
            Assert.AreEqual(16, new CopyleaksTextModerationRequestModel("t", true, null, Labels(16)).Labels.Length);
            Assert.ThrowsException<ArgumentException>(() => new CopyleaksTextModerationRequestModel("t", true, null, Labels(17)));
            Assert.ThrowsException<ArgumentException>(() => new CopyleaksTextModerationRequestModel("t", true, null, Labels(0)));
            Assert.ThrowsException<ArgumentException>(() => new CopyleaksTextModerationRequestModel("t", true, null, null));
        }

        // AI image detection
        [TestMethod]
        public async Task Ai_image_json_with_mask_type_and_object_detection()
        {
            var (handler, client) = Server(HttpStatusCode.OK, "{\"model\":\"ai-image-1-ultra\",\"summary\":{\"human\":0.3,\"ai\":0.7},\"isAiDetected\":true}");
            var model = new CopyleaksAiImageDetectionRequestModel("aGVsbG8=", "photo.png", CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA, sandbox: true)
            {
                MaskType = CopyleaksAiImageMaskTypes.HEATMAP,
                ObjectDetection = new List<CopyleaksAiImageObjectDetectionItem> { new CopyleaksAiImageObjectDetectionItem("face"), new CopyleaksAiImageObjectDetectionItem("hand") },
            };

            var result = await new CopyleaksAiImageDetectionApi(client).SubmitAsync("img1", model, Fakes.Token);

            Assert.IsTrue(result.IsAiDetected.Value);
            AssertRequest(handler.Single, HttpMethod.Post, $"{Api}/v1/ai-image-detector/img1/check");
            AssertJsonBody(handler.Single,
                "{\"base64\":\"aGVsbG8=\",\"filename\":\"photo.png\",\"model\":\"ai-image-1-ultra\",\"sandbox\":true,\"maskType\":\"heatmap\",\"objectDetection\":[{\"object\":\"face\"},{\"object\":\"hand\"}]}",
                gzip: false);
        }

        [TestMethod]
        public async Task Ai_image_json_omits_optional_fields()
        {
            var (handler, client) = Server(HttpStatusCode.OK, "{\"summary\":{\"human\":1,\"ai\":0}}");
            await new CopyleaksAiImageDetectionApi(client).SubmitAsync("img2",
                new CopyleaksAiImageDetectionRequestModel("aGVsbG8=", "photo.png", CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA), Fakes.Token);
            AssertJsonBody(handler.Single, "{\"base64\":\"aGVsbG8=\",\"filename\":\"photo.png\",\"model\":\"ai-image-1-ultra\",\"sandbox\":false}", gzip: false);
        }

        [TestMethod]
        public async Task Ai_image_multipart_from_stream_and_bytes()
        {
            byte[] image = { 0x89, 0x50, 0x4E, 0x47, 1, 2, 3 };
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.OK, "{\"summary\":{\"human\":1,\"ai\":0}}");
            var api = new CopyleaksAiImageDetectionApi(Fakes.Client(handler));
            var model = new CopyleaksAiImageDetectionRequestModel("photo.png", CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA, sandbox: true)
            {
                MaskType = CopyleaksAiImageMaskTypes.NONE,
                ObjectDetection = new List<CopyleaksAiImageObjectDetectionItem> { new CopyleaksAiImageObjectDetectionItem("face") },
            };

            using (var stream = new MemoryStream(image))
                await api.SubmitFileAsync("img3", stream, model, Fakes.Token);
            await api.SubmitFileAsync("img4", image, model, Fakes.Token);

            foreach (var (request, scanId) in handler.RequestList.Zip(new[] { "img3", "img4" }))
            {
                AssertRequest(request, HttpMethod.Post, $"{Api}/v1/ai-image-detector/{scanId}/check");
                StringAssert.StartsWith(request.ContentHeaders["Content-Type"], "multipart/form-data");
                var parts = Multipart.Parse(request);
                CollectionAssert.AreEqual(image, parts["image"].Data);
                Assert.AreEqual("photo.png", parts["image"].FileName);
                Assert.AreEqual("image/png", parts["image"].ContentType);
                Assert.AreEqual("photo.png", parts["filename"].Text);
                Assert.AreEqual("ai-image-1-ultra", parts["model"].Text);
                Assert.AreEqual("true", parts["sandbox"].Text);
                Assert.AreEqual("none", parts["maskType"].Text);
                Assert.AreEqual("[{\"object\":\"face\"}]", parts["objectDetection"].Text);
                Assert.IsFalse(parts.ContainsKey("base64"));
            }
        }

        [TestMethod]
        public async Task Ai_image_multipart_rejects_base64()
        {
            var api = new CopyleaksAiImageDetectionApi(Fakes.Client(new FakeHttpHandler()));
            var model = new CopyleaksAiImageDetectionRequestModel("aGVsbG8=", "photo.png", CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA);
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => api.SubmitFileAsync("img5", new byte[] { 1 }, model, Fakes.Token));
        }

        [TestMethod]
        public async Task Ai_image_async_submit()
        {
            var (handler, client) = Server(HttpStatusCode.Created, "{}");
            var model = new CopyleaksAiImageDetectionAsyncRequestModel
            {
                Url = new Uri("https://example.com/image1.png"),
                FileName = "image1.png",
                Model = CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA,
                MaskType = CopyleaksAiImageMaskTypes.HEATMAP,
                ObjectDetection = new List<CopyleaksAiImageObjectDetectionItem> { new CopyleaksAiImageObjectDetectionItem("face") },
                Headers = new string[,] { { "X-Custom-Auth", "my-token" } },
                Webhooks = new CopyleaksAsyncWebhooks { Url = new Uri(Webhook), Headers = new string[,] { { "Authorization", "Bearer hook" } } },
                Sandbox = true,
                DeveloperPayload = "order-1",
            };

            await new CopyleaksAiImageDetectionApi(client).SubmitUrlAsync("imgasync1", model, Fakes.Token);

            AssertRequest(handler.Single, HttpMethod.Put, $"{Api}/v1/ai-image-detector-async/imgasync1/submit");
            AssertJsonBody(handler.Single, @"{
                ""url"":""https://example.com/image1.png"",
                ""fileName"":""image1.png"",
                ""model"":""ai-image-1-ultra"",
                ""maskType"":""heatmap"",
                ""objectDetection"":[{""object"":""face""}],
                ""headers"":[[""X-Custom-Auth"",""my-token""]],
                ""webhooks"":{""url"":""https://example.com/webhook/{STATUS}"",""headers"":[[""Authorization"",""Bearer hook""]]},
                ""sandbox"":true,
                ""developerPayload"":""order-1""
            }", gzip: false);
        }

        [TestMethod]
        public async Task Ai_image_async_minimal_body()
        {
            var (handler, client) = Server(HttpStatusCode.Created, "{}");
            await new CopyleaksAiImageDetectionApi(client).SubmitUrlAsync("imgasync2", new CopyleaksAiImageDetectionAsyncRequestModel
            {
                Url = new Uri("https://example.com/a.png"),
                FileName = "a.png",
                Model = "ai-image-1-ultra",
                Webhooks = new CopyleaksAsyncWebhooks { Url = new Uri(Webhook) },
            }, Fakes.Token);
            AssertJsonBody(handler.Single,
                "{\"url\":\"https://example.com/a.png\",\"fileName\":\"a.png\",\"model\":\"ai-image-1-ultra\",\"webhooks\":{\"url\":\"https://example.com/webhook/{STATUS}\"},\"sandbox\":false}",
                gzip: false);
        }

        // AI video detection
        [TestMethod]
        public async Task Ai_video_submit()
        {
            var (handler, client) = Server(HttpStatusCode.Created, "{}");
            var model = new CopyleaksAiVideoDetectionRequestModel
            {
                Url = new Uri("https://example.com/my-video.mp4"),
                Headers = new string[,] { { "X-Custom-Auth", "my-token" } },
                Verb = "GET",
                Filename = "my-video.mp4",
                Model = CopyleaksAiVideoDetectionModels.AI_VIDEO_1_PRO,
                Webhooks = new CopyleaksAsyncWebhooks { Url = new Uri("https://example.com/webhook/receive-results") },
                Sandbox = true,
                DeveloperPayload = "order-id-12345",
            };

            await new CopyleaksAiVideoDetectionApi(client).SubmitAsync("video1", model, Fakes.Token);

            AssertRequest(handler.Single, HttpMethod.Post, $"{Api}/v1/ai-video-detector/video1/submit");
            AssertJsonBody(handler.Single, @"{
                ""url"":""https://example.com/my-video.mp4"",
                ""headers"":[[""X-Custom-Auth"",""my-token""]],
                ""verb"":""GET"",
                ""filename"":""my-video.mp4"",
                ""model"":""ai-video-1-pro"",
                ""webhooks"":{""url"":""https://example.com/webhook/receive-results""},
                ""sandbox"":true,
                ""developerPayload"":""order-id-12345""
            }", gzip: false);
        }

        [TestMethod]
        public async Task Ai_video_requires_url_filename_model_and_webhook()
        {
            var api = new CopyleaksAiVideoDetectionApi(Fakes.Client(new FakeHttpHandler()));
            var model = new CopyleaksAiVideoDetectionRequestModel { Url = new Uri("https://example.com/v.mp4"), Filename = "v.mp4", Model = "ai-video-1-pro" };
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => api.SubmitAsync("video2", model, Fakes.Token));
        }

        // Image Plagiarism Detection
        [TestMethod]
        public async Task Image_plagiarism_multipart_upload()
        {
            const string response = @"{
                ""developerPayload"":""my-custom-data"",
                ""scannedImage"":{""scanId"":""ip1"",""expectedCredits"":1,""actualCredits"":1,""creationTime"":""2026-05-24T10:00:00Z"",""width"":1920,""height"":1080,""filename"":""my-photo.jpg""},
                ""matches"":{
                    ""internet"":[
                        {""url"":""https://example.com/images/photo.jpg"",""matchType"":0,""webPages"":[{""url"":""https://example.com/blog/my-post""}]},
                        {""url"":""https://example.org/gallery/photo-sm.jpg"",""matchType"":1}
                    ],
                    ""score"":{""totalMatches"":2,""fullMatches"":1,""partialMatches"":1}
                }
            }";
            byte[] image = { 0xFF, 0xD8, 0xFF, 0xE0, 9, 8, 7 };
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.OK, response);
            var api = new CopyleaksImagePlagiarismApi(Fakes.Client(handler));

            var result = await api.SubmitAsync("ip1", image, new CopyleaksImagePlagiarismRequestModel("my-photo.jpg", sandbox: true) { DeveloperPayload = "my-custom-data" }, Fakes.Token);

            var request = handler.Single;
            AssertRequest(request, HttpMethod.Post, $"{Api}/v1/image-plagiarism-detector/ip1/check");
            StringAssert.StartsWith(request.ContentHeaders["Content-Type"], "multipart/form-data");
            var parts = Multipart.Parse(request);
            CollectionAssert.AreEqual(image, parts["image"].Data);
            Assert.AreEqual("my-photo.jpg", parts["image"].FileName);
            Assert.AreEqual("image/jpeg", parts["image"].ContentType);
            Assert.AreEqual("my-photo.jpg", parts["filename"].Text);
            Assert.AreEqual("true", parts["sandbox"].Text);
            Assert.AreEqual("my-custom-data", parts["developerPayload"].Text);
            Assert.AreEqual(4, parts.Count);

            Assert.AreEqual("my-custom-data", result.DeveloperPayload);
            Assert.AreEqual(1920, result.ScannedImage.Width);
            Assert.AreEqual(2, result.Matches.Internet.Count);
            Assert.AreEqual(0, result.Matches.Internet[0].MatchType);
            Assert.AreEqual("https://example.com/blog/my-post", result.Matches.Internet[0].WebPages[0].Url);
            Assert.IsNull(result.Matches.Internet[1].WebPages);
            Assert.AreEqual(1, result.Matches.Score.PartialMatches);
        }

        [TestMethod]
        public async Task Image_plagiarism_from_stream_omits_developer_payload_when_unset()
        {
            byte[] image = { 1, 2, 3, 4 };
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.OK, "{\"scannedImage\":{\"scanId\":\"ip2\"},\"matches\":{\"internet\":[],\"score\":{\"totalMatches\":0}}}");
            var api = new CopyleaksImagePlagiarismApi(Fakes.Client(handler));

            using (var stream = new MemoryStream(image))
            {
                stream.Position = 0;
                await api.SubmitAsync("ip2", stream, new CopyleaksImagePlagiarismRequestModel("photo.webp"), Fakes.Token);
            }

            var parts = Multipart.Parse(handler.Single);
            CollectionAssert.AreEqual(image, parts["image"].Data);
            Assert.AreEqual("image/webp", parts["image"].ContentType);
            Assert.AreEqual("false", parts["sandbox"].Text);
            Assert.IsFalse(parts.ContainsKey("developerPayload"));
        }

        [TestMethod]
        public async Task Image_plagiarism_stream_is_resent_after_429()
        {
            byte[] image = { 5, 6, 7 };
            var handler = new FakeHttpHandler()
                .Respond(HttpStatusCode.TooManyRequests, "{}")
                .Respond(HttpStatusCode.OK, "{\"matches\":{\"internet\":[]}}");
            var api = new CopyleaksImagePlagiarismApi(Fakes.Client(handler), Fakes.FastRetries());

            using (var stream = new MemoryStream(image))
                await api.SubmitAsync("ip3", stream, new CopyleaksImagePlagiarismRequestModel("p.png", true), Fakes.Token);

            var requests = handler.RequestList;
            Assert.AreEqual(2, requests.Count);
            CollectionAssert.AreEqual(image, Multipart.Parse(requests[1])["image"].Data, "the retry must resend the whole image");
        }
    }
}
