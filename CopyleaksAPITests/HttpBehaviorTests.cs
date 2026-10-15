using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.AIDetection;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Copyleaks.SDK.V3.API.Models.Responses;
using CopyleaksAPITests.TestSupport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CopyleaksAPITests
{
    /// <summary>
    /// Offline tests of the HTTP layer: headers, retries, timeouts, cancellation, HttpClient ownership and request bodies.
    /// </summary>
    [TestClass]
    public class HttpBehaviorTests
    {
        private const string CreditsJson = "{\"Amount\":42}";
        private const string AiTextJson = "{\"scanType\":\"sandbox\",\"summary\":{\"human\":0.25,\"ai\":0.75}}";

        private static NaturalLanguageDocument AiDocument() => new NaturalLanguageDocument { Text = new string('a', 300), Sandbox = true };

        [TestMethod]
        public async Task Requests_carry_cached_user_agent_and_bearer_token_and_no_hop_by_hop_headers()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.OK, CreditsJson);
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries());

            Assert.AreEqual(42u, await api.CreditBalanceAsync(Fakes.Token));

            var request = handler.Single;
            string fileVersion = typeof(CopyleaksScansApi).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            Assert.AreEqual("5.0.0", fileVersion);
            Assert.AreEqual($"copyleaks-core-sdk/{fileVersion}", request.Header("User-Agent"));
            Assert.AreEqual($"Bearer {Fakes.Token}", request.Header("Authorization"));
            Assert.IsNull(request.Header("Keep-Alive"));
            Assert.IsNull(request.Header("Connection"));
        }

        [TestMethod]
        public async Task Public_endpoints_send_no_authorization_header()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.OK, "[\"en\"]");
            var api = new CopyleaksScansApi(Fakes.Client(handler));

            await api.GetOcrLanguageListAsync();

            Assert.IsNull(handler.Single.Header("Authorization"));
        }

        [TestMethod]
        public async Task Retried_get_sends_the_request_again()
        {
            var handler = new FakeHttpHandler()
                .Respond(HttpStatusCode.ServiceUnavailable, "{}")
                .Respond(HttpStatusCode.BadGateway, "{}")
                .Respond(HttpStatusCode.OK, CreditsJson);
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries());

            Assert.AreEqual(42u, await api.CreditBalanceAsync(Fakes.Token));

            var requests = handler.RequestList;
            Assert.AreEqual(3, requests.Count, "every retry must reach the server");
            Assert.IsTrue(requests.All(r => r.Method == HttpMethod.Get && r.Uri.AbsolutePath == "/v3/scans/credits"));
            Assert.IsTrue(requests.All(r => r.Header("Authorization") == $"Bearer {Fakes.Token}"));
        }

        [TestMethod]
        public async Task Retried_submit_resends_the_full_body()
        {
            var handler = new FakeHttpHandler()
                .Respond(HttpStatusCode.InternalServerError)
                .Respond(HttpStatusCode.Created);
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries());
            var document = new FileDocument
            {
                Base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello world")),
                Filename = "file.txt",
                PropertiesSection = new ClientScanProperties { Sandbox = true, Webhooks = new Webhooks { Status = new Uri("https://example.com/webhook/{STATUS}") } },
            };

            await api.SubmitFileAsync("scan1", document, Fakes.Token);

            var requests = handler.RequestList;
            Assert.AreEqual(2, requests.Count);
            Assert.IsTrue(requests.All(r => r.Method == HttpMethod.Put && r.Uri.AbsolutePath == "/v3/scans/submit/file/scan1"));
            Assert.IsTrue(requests.All(r => r.IsGzip));
            Assert.IsTrue(requests.All(r => r.ContentLength == r.RawBody.Length));
            Assert.AreEqual(requests[0].BodyText, requests[1].BodyText);
            Assert.AreEqual(JsonConvert.SerializeObject(document), requests[1].BodyText, "the retried body must be the complete request");
        }

        [TestMethod]
        public async Task Retry_on_network_error_for_safe_calls()
        {
            var handler = new FakeHttpHandler()
                .Throw(new HttpRequestException("connection reset"))
                .Respond(HttpStatusCode.OK, CreditsJson);
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries());

            Assert.AreEqual(42u, await api.CreditBalanceAsync(Fakes.Token));
            Assert.AreEqual(2, handler.RequestList.Count);
        }

        [TestMethod]
        public async Task Retries_stop_after_MaxRetries_and_return_the_last_error()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.ServiceUnavailable, "{\"error\":\"down\"}");
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries(maxRetries: 2));

            var ex = await Assert.ThrowsExceptionAsync<CopyleaksHttpException>(() => api.CreditBalanceAsync(Fakes.Token));

            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, ex.HttpErrorCode);
            Assert.AreEqual(3, handler.RequestList.Count, "1 attempt + 2 retries");
            Assert.AreEqual(3, ex.Attempt, "the error came from the third attempt");
        }

        [TestMethod]
        public async Task MaxRetries_zero_turns_retries_off()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.ServiceUnavailable, "{}");
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries(maxRetries: 0));

            await Assert.ThrowsExceptionAsync<CopyleaksHttpException>(() => api.CreditBalanceAsync(Fakes.Token));
            Assert.AreEqual(1, handler.RequestList.Count);
        }

        [TestMethod]
        public async Task Charged_calls_are_not_retried_on_server_errors_or_network_errors()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.ServiceUnavailable, "{}");
            var api = new CopyleaksAIDetectionApi(Fakes.Client(handler), Fakes.FastRetries());
            await Assert.ThrowsExceptionAsync<CopyleaksHttpException>(() => api.SubmitNaturalLanguageAsync("scan1", AiDocument(), Fakes.Token));
            Assert.AreEqual(1, handler.RequestList.Count, "a 503 on a charged call must not be repeated");

            var failing = new FakeHttpHandler().Throw(new HttpRequestException("reset"));
            var api2 = new CopyleaksAIDetectionApi(Fakes.Client(failing), Fakes.FastRetries());
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => api2.SubmitNaturalLanguageAsync("scan1", AiDocument(), Fakes.Token));
            Assert.AreEqual(1, failing.RequestList.Count, "a network error on a charged call must not be repeated");
        }

        [TestMethod]
        public async Task Charged_calls_are_retried_on_429()
        {
            var handler = new FakeHttpHandler()
                .Respond(HttpStatusCode.TooManyRequests, "{}")
                .Respond(HttpStatusCode.OK, AiTextJson);
            var api = new CopyleaksAIDetectionApi(Fakes.Client(handler), Fakes.FastRetries());

            var result = await api.SubmitNaturalLanguageAsync("scan1", AiDocument(), Fakes.Token);

            Assert.AreEqual(0.75, result.Summary.AI);
            var requests = handler.RequestList;
            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual(requests[0].BodyText, requests[1].BodyText);
        }

        [TestMethod]
        public async Task Retry_After_is_honoured()
        {
            var handler = new FakeHttpHandler()
                .Respond(HttpStatusCode.TooManyRequests, "{}", r => r.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(1)))
                .Respond(HttpStatusCode.OK, CreditsJson);
            var options = Fakes.FastRetries();
            var api = new CopyleaksScansApi(Fakes.Client(handler), options);

            var watch = Stopwatch.StartNew();
            Assert.AreEqual(42u, await api.CreditBalanceAsync(Fakes.Token));
            watch.Stop();

            Assert.AreEqual(2, handler.RequestList.Count);
            Assert.IsTrue(watch.Elapsed >= TimeSpan.FromMilliseconds(950), $"waited only {watch.ElapsedMilliseconds} ms");
        }

        [TestMethod]
        public async Task Retry_After_longer_than_MaxRetryDelay_is_not_retried()
        {
            var handler = new FakeHttpHandler()
                .Respond(HttpStatusCode.TooManyRequests, "{\"error\":{\"type\":\"rate_limit_error\",\"id\":\"too_many_requests\",\"code\":7,\"message\":\"slow down\"}}",
                         r => r.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromMinutes(15)));
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries());

            var ex = await Assert.ThrowsExceptionAsync<CopyleaksHttpException>(() => api.CreditBalanceAsync(Fakes.Token));

            Assert.AreEqual(1, handler.RequestList.Count);
            Assert.AreEqual(HttpStatusCode.TooManyRequests, ex.HttpErrorCode);
            Assert.AreEqual(TimeSpan.FromMinutes(15), ex.RetryAfter);
            Assert.AreEqual("rate_limit_error", ex.ErrorType);
            Assert.AreEqual("too_many_requests", ex.ErrorId);
            Assert.AreEqual(7, ex.ErrorCode);
            StringAssert.Contains(ex.Message, "slow down");
            StringAssert.Contains(ex.ResponseBody, "slow down");
        }

        [TestMethod]
        public async Task Client_errors_are_not_retried()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.BadRequest, "{\"error\":\"bad\"}");
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries());

            var ex = await Assert.ThrowsExceptionAsync<CopyleaksHttpException>(() => api.CreditBalanceAsync(Fakes.Token));
            Assert.AreEqual(HttpStatusCode.BadRequest, ex.HttpErrorCode);
            Assert.AreEqual(1, handler.RequestList.Count);
            Assert.AreEqual(1, ex.Attempt);
            Assert.IsNull(ex.ErrorType, "a string error is not an error envelope");
        }

        [TestMethod]
        public async Task Timeout_is_applied_per_attempt_and_reported_as_TimeoutException()
        {
            var handler = new FakeHttpHandler().Hang();
            var options = Fakes.FastRetries(maxRetries: 1);
            options.Timeout = TimeSpan.FromMilliseconds(200);
            var api = new CopyleaksScansApi(Fakes.Client(handler), options);

            var watch = Stopwatch.StartNew();
            var ex = await Assert.ThrowsExceptionAsync<TimeoutException>(() => api.CreditBalanceAsync(Fakes.Token));
            watch.Stop();

            Assert.AreEqual(2, handler.RequestList.Count, "a timed out safe call is retried");
            Assert.IsTrue(watch.Elapsed < TimeSpan.FromSeconds(10));
            StringAssert.Contains(ex.Message, "/v3/scans/credits");
        }

        [TestMethod]
        public async Task Timeout_on_charged_call_is_not_retried()
        {
            var handler = new FakeHttpHandler().Hang();
            var options = Fakes.FastRetries();
            options.Timeout = TimeSpan.FromMilliseconds(200);
            var api = new CopyleaksAIDetectionApi(Fakes.Client(handler), options);

            await Assert.ThrowsExceptionAsync<TimeoutException>(() => api.SubmitNaturalLanguageAsync("scan1", AiDocument(), Fakes.Token));
            Assert.AreEqual(1, handler.RequestList.Count);
        }

        [TestMethod]
        public async Task Conflict_on_the_retry_of_a_timed_out_submit_reports_the_attempt()
        {
            // The first attempt reaches the server but its response never arrives; the retry finds the scan already exists.
            var handler = new FakeHttpHandler()
                .Hang()
                .Respond(HttpStatusCode.Conflict, "{\"error\":{\"type\":\"invalid_request_error\",\"id\":\"scan_exists\",\"code\":409,\"message\":\"exists\"}}");
            var options = Fakes.FastRetries();
            options.Timeout = TimeSpan.FromMilliseconds(200);
            var api = new CopyleaksScansApi(Fakes.Client(handler), options);
            var document = new FileDocument { Base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello")), Filename = "file.txt" };

            var ex = await Assert.ThrowsExceptionAsync<CopyleaksHttpException>(() => api.SubmitFileAsync("scan1", document, Fakes.Token));

            Assert.AreEqual(HttpStatusCode.Conflict, ex.HttpErrorCode);
            Assert.AreEqual(2, ex.Attempt, "a 409 on a retry means an earlier attempt was accepted");
            Assert.AreEqual(2, handler.RequestList.Count);
        }

        [TestMethod]
        public void Default_timeout_comes_from_the_RequestsTimeout_configuration()
        {
            Assert.AreEqual(TimeSpan.FromMilliseconds(60000), new CopyleaksClientOptions().Timeout);
            Assert.AreEqual(3, new CopyleaksClientOptions().MaxRetries);
        }

        [TestMethod]
        public async Task Cancellation_stops_an_in_flight_request_without_retrying()
        {
            var handler = new FakeHttpHandler().Hang();
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries());
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200)))
            {
                await AssertCanceledAsync(() => api.CreditBalanceAsync(Fakes.Token, cts.Token));
                Assert.IsTrue(cts.IsCancellationRequested);
            }
            Assert.AreEqual(1, handler.RequestList.Count, "a cancelled call is not retried");
        }

        [TestMethod]
        public async Task Cancelled_token_sends_nothing()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.OK, CreditsJson);
            var api = new CopyleaksScansApi(Fakes.Client(handler));
            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();
                await AssertCanceledAsync(() => api.CreditBalanceAsync(Fakes.Token, cts.Token));
            }
            Assert.AreEqual(0, handler.RequestList.Count);
        }

        [TestMethod]
        public async Task Cancellation_during_the_retry_delay_stops_retrying()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.ServiceUnavailable, "{}");
            var options = Fakes.FastRetries();
            options.RetryBaseDelay = TimeSpan.FromSeconds(4);
            var api = new CopyleaksScansApi(Fakes.Client(handler), options);
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300)))
            {
                var watch = Stopwatch.StartNew();
                await AssertCanceledAsync(() => api.CreditBalanceAsync(Fakes.Token, cts.Token));
                Assert.IsTrue(watch.Elapsed < TimeSpan.FromSeconds(3));
            }
            Assert.AreEqual(1, handler.RequestList.Count);
        }

        [TestMethod]
        public void Api_objects_share_one_HttpClient_and_never_dispose_it()
        {
            var first = new CopyleaksScansApi();
            var second = new CopyleaksAIDetectionApi();
            var third = new CopyleaksIdentityApi();

            Assert.AreSame(first.Client, second.Client);
            Assert.AreSame(first.Client, third.Client);
            Assert.AreEqual(Timeout.InfiniteTimeSpan, first.Client.Timeout, "the SDK timeout is applied per attempt");

            first.Dispose();
            second.Dispose();
            third.Client.CancelPendingRequests(); // throws ObjectDisposedException if the shared client was disposed
        }

        [TestMethod]
        public void Injected_HttpClient_is_used_and_not_disposed()
        {
            var handler = new FakeHttpHandler();
            var client = Fakes.Client(handler);
            var api = new CopyleaksScansApi(client);

            Assert.AreSame(client, api.Client);
            api.Dispose();
            client.CancelPendingRequests(); // throws ObjectDisposedException if the SDK disposed it
        }

        [TestMethod]
        public async Task Works_as_an_IHttpClientFactory_typed_client()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.OK, "{\"textual\":[\"pdf\"],\"ocr\":[\"png\"]}");
            var services = new ServiceCollection();
            services.AddHttpClient<CopyleaksScansApi>().ConfigurePrimaryHttpMessageHandler(() => handler);
            services.AddSingleton(new CopyleaksClientOptions { MaxRetries = 5 });

            using (var provider = services.BuildServiceProvider())
            {
                var api = provider.GetRequiredService<CopyleaksScansApi>();
                Assert.AreEqual(5, api.Options.MaxRetries, "options registered in DI are used");

                var types = await api.GetSupportedFileTypesAsync();

                CollectionAssert.AreEqual(new[] { "pdf" }, types.Textual);
                Assert.AreEqual("/v3/miscellaneous/supported-plagiarism-file-types", handler.Single.Uri.AbsolutePath);
            }
        }

        [TestMethod]
        public async Task Endpoints_can_be_set_per_instance()
        {
            var handler = new FakeHttpHandler()
                .Respond(HttpStatusCode.OK, "{\"access_token\":\"t\"}")
                .Respond(HttpStatusCode.OK, CreditsJson);
            var client = Fakes.Client(handler);

            await new CopyleaksIdentityApi(client, new CopyleaksClientOptions { IdentityEndpoint = "https://id.example.test" }).LoginAsync("a@b.c", "k");
            await new CopyleaksScansApi(client, new CopyleaksClientOptions { ApiEndpoint = "https://api.example.test/" }).CreditBalanceAsync(Fakes.Token);

            Assert.AreEqual("https://id.example.test/v3/account/login/api", handler.RequestList[0].Uri.ToString());
            Assert.AreEqual("https://api.example.test/v3/scans/credits", handler.RequestList[1].Uri.ToString());
        }

        [TestMethod]
        public async Task Path_parameters_are_url_encoded()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.OK, "{\"percents\":100}");
            var api = new CopyleaksScansApi(Fakes.Client(handler));

            await api.ProgressAsync("my scan?#1", Fakes.Token);

            var request = handler.Single;
            Assert.AreEqual("/v3/scans/my%20scan%3F%231/progress", request.Uri.AbsolutePath);
            Assert.AreEqual(string.Empty, request.Uri.Query);
        }

        [TestMethod]
        public async Task Large_submit_is_sent_compressed_with_a_content_length()
        {
            var bytes = new byte[2 * 1024 * 1024 + 7];
            new Random(1).NextBytes(bytes);
            var document = new FileDocument
            {
                Base64 = Convert.ToBase64String(bytes),
                Filename = "big.bin",
                PropertiesSection = new ClientScanProperties { Sandbox = true },
            };
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.Created);
            var api = new CopyleaksScansApi(Fakes.Client(handler));

            await api.SubmitFileAsync("big1", document, Fakes.Token);

            var request = handler.Single;
            Assert.AreEqual("gzip", request.ContentHeaders["Content-Encoding"]);
            Assert.AreEqual("application/json; charset=utf-8", request.ContentHeaders["Content-Type"]);
            Assert.AreEqual(request.RawBody.Length, request.ContentLength);
            Assert.AreEqual(JsonConvert.SerializeObject(document), request.BodyText);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(49151)]
        [DataRow(49152)]
        [DataRow(49153)]
        [DataRow(300001)]
        public async Task Stream_submit_encodes_base64_while_reading(int size)
        {
            var bytes = new byte[size];
            new Random(size).NextBytes(bytes);
            var properties = new ClientScanProperties { Sandbox = true, DeveloperPayload = "payload" };
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.Created);
            var api = new CopyleaksScansApi(Fakes.Client(handler));

            using (var stream = new NonSeekableStream(bytes))
                await api.SubmitFileAsync("stream1", stream, "file.pdf", properties, Fakes.Token);

            var request = handler.Single;
            Assert.AreEqual(HttpMethod.Put, request.Method);
            Assert.AreEqual("/v3/scans/submit/file/stream1", request.Uri.AbsolutePath);
            Assert.IsTrue(request.IsGzip);

            JObject expected = JObject.Parse(JsonConvert.SerializeObject(new FileDocument
            {
                Base64 = Convert.ToBase64String(bytes),
                Filename = "file.pdf",
                PropertiesSection = properties,
            }));
            Assert.IsTrue(JToken.DeepEquals(expected, request.Json), "the streamed body must equal the FileDocument body");
        }

        [TestMethod]
        public async Task Stream_submit_is_retried_from_the_buffered_body()
        {
            var bytes = Encoding.UTF8.GetBytes("retry me");
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.BadGateway).Respond(HttpStatusCode.Created);
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries());

            using (var stream = new NonSeekableStream(bytes))
                await api.SubmitImageOCRAsync("ocr1", stream, "scan.png", "en", new ClientScanProperties { Sandbox = true }, Fakes.Token);

            var requests = handler.RequestList;
            Assert.AreEqual(2, requests.Count);
            Assert.AreEqual("/v3/scans/submit/ocr/ocr1", requests[1].Uri.AbsolutePath);
            Assert.AreEqual(Convert.ToBase64String(bytes), requests[1].Json["base64"].Value<string>());
            Assert.AreEqual("en", requests[1].Json["langCode"].Value<string>());
            Assert.AreEqual(requests[0].BodyText, requests[1].BodyText);
        }

        [TestMethod]
        public async Task Stream_submit_reads_streams_that_only_support_async_reads()
        {
            var bytes = new byte[200001];
            new Random(7).NextBytes(bytes);
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.Created);
            var api = new CopyleaksScansApi(Fakes.Client(handler));

            using (var stream = new AsyncOnlyStream(bytes))
                await api.SubmitFileAsync("async1", stream, "file.pdf", new ClientScanProperties { Sandbox = true }, Fakes.Token);

            var request = handler.Single;
            Assert.AreEqual(Convert.ToBase64String(bytes), request.Json["base64"].Value<string>());
            Assert.AreEqual("file.pdf", request.Json["filename"].Value<string>());
            Assert.AreEqual(request.RawBody.Length, request.ContentLength);
        }

        [TestMethod]
        public async Task Stream_submit_passes_the_cancellation_token_to_the_stream()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.Created);
            var api = new CopyleaksScansApi(Fakes.Client(handler));

            using (var stream = new AsyncOnlyStream(new byte[10], stall: true))
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200)))
            {
                await AssertCanceledAsync(() => api.SubmitImageOCRAsync("async2", stream, "scan.png", "en", new ClientScanProperties(), Fakes.Token, cts.Token));
            }
            Assert.AreEqual(0, handler.RequestList.Count, "nothing is sent when reading the stream was cancelled");
        }

        [TestMethod]
        public async Task Pdf_download_is_retried_until_headers_arrive_and_returned_read_only()
        {
            var bytes = new byte[300000];
            new Random(3).NextBytes(bytes);
            var handler = new FakeHttpHandler()
                .Respond(HttpStatusCode.ServiceUnavailable, "{}")
                .RespondBytes(HttpStatusCode.OK, bytes, "application/pdf");
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries());

            using (var pdf = await api.DownloadPdfReportAsync("s1", Fakes.Token))
            using (var copy = new MemoryStream())
            {
                Assert.AreEqual(0, pdf.Position);
                Assert.IsFalse(pdf.CanWrite);
                await pdf.CopyToAsync(copy);
                CollectionAssert.AreEqual(bytes, copy.ToArray());
            }
            Assert.AreEqual(2, handler.RequestList.Count);
        }

        [TestMethod]
        public async Task Download_error_is_reported_with_its_body()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.NotFound,
                "{\"error\":{\"type\":\"invalid_request_error\",\"id\":\"not_found\",\"code\":404,\"message\":\"missing\"}}");
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries());

            using (var csv = new MemoryStream())
            {
                var ex = await Assert.ThrowsExceptionAsync<CopyleaksHttpException>(
                    () => api.GetUserUsageAsync(new DateTime(2026, 1, 1), new DateTime(2026, 1, 2), csv, Fakes.Token));

                Assert.AreEqual(HttpStatusCode.NotFound, ex.HttpErrorCode);
                Assert.AreEqual("not_found", ex.ErrorId);
                Assert.AreEqual(1, ex.Attempt);
                Assert.AreEqual(0, csv.Length, "nothing is written to the caller's stream for an error");
            }
        }

        [TestMethod]
        public async Task Download_timeout_covers_reading_the_body_and_is_not_retried()
        {
            var handler = new FakeHttpHandler().Respond((request, ct) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StalledContent() }));
            var options = Fakes.FastRetries();
            options.Timeout = TimeSpan.FromMilliseconds(300);
            var api = new CopyleaksScansApi(Fakes.Client(handler), options);

            var watch = Stopwatch.StartNew();
            var ex = await Assert.ThrowsExceptionAsync<TimeoutException>(() => api.DownloadPdfReportAsync("s1", Fakes.Token));
            watch.Stop();

            StringAssert.Contains(ex.Message, "/v3/downloads/s1/report.pdf");
            Assert.IsTrue(watch.Elapsed < TimeSpan.FromSeconds(10));
            Assert.AreEqual(1, handler.RequestList.Count, "a failure while the body is read is not retried");
        }

        [TestMethod]
        public async Task Cancelling_a_download_while_the_body_is_read_throws_OperationCanceledException()
        {
            var handler = new FakeHttpHandler().Respond((request, ct) =>
                Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StalledContent() }));
            var api = new CopyleaksScansApi(Fakes.Client(handler), Fakes.FastRetries());

            using (var csv = new MemoryStream())
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200)))
            {
                await AssertCanceledAsync(() => api.GetUserUsageAsync(new DateTime(2026, 1, 1), new DateTime(2026, 1, 2), csv, Fakes.Token, cts.Token));
            }
            Assert.AreEqual(1, handler.RequestList.Count);
        }

        [TestMethod]
        public async Task Empty_success_body_for_a_json_call_is_reported()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.OK, "");
            var api = new CopyleaksScansApi(Fakes.Client(handler));

            await Assert.ThrowsExceptionAsync<JsonException>(() => api.CreditBalanceAsync(Fakes.Token));
        }

        [TestMethod]
        public async Task Error_without_body_has_a_useful_message()
        {
            var handler = new FakeHttpHandler().Respond(HttpStatusCode.Unauthorized);
            var api = new CopyleaksScansApi(Fakes.Client(handler));

            var ex = await Assert.ThrowsExceptionAsync<CopyleaksHttpException>(() => api.CreditBalanceAsync(Fakes.Token));
            StringAssert.Contains(ex.Message, "401");
        }

        [TestMethod]
        public void Public_constructor_of_CopyleaksHttpException_still_works()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent("{\"error\":{\"type\":\"invalid_request_error\",\"id\":\"scan_exists\",\"code\":\"409\",\"message\":\"exists\"}}"),
            };

#pragma warning disable CS0618 // The obsolete constructor is kept for compatibility, and this test covers it.
            var ex = new CopyleaksHttpException(response);
#pragma warning restore CS0618

            Assert.AreEqual(HttpStatusCode.Conflict, ex.HttpErrorCode);
            Assert.AreEqual("scan_exists", ex.ErrorId);
            Assert.AreEqual(409, ex.ErrorCode);
            Assert.AreEqual(1, ex.Attempt);
        }

        [TestMethod]
        public async Task CopyleaksHttpException_CreateAsync_reads_the_body_without_blocking()
        {
            var response = new HttpResponseMessage(HttpStatusCode.PaymentRequired)
            {
                Content = new StringContent("{\"error\":{\"type\":\"payment_error\",\"id\":\"insufficient_credits\",\"code\":2,\"message\":\"no credits\"}}"),
            };

            var ex = await CopyleaksHttpException.CreateAsync(response);

            Assert.AreEqual(HttpStatusCode.PaymentRequired, ex.HttpErrorCode);
            Assert.AreEqual("insufficient_credits", ex.ErrorId);
            Assert.AreEqual(2, ex.ErrorCode);
            Assert.AreEqual(1, ex.Attempt);
            StringAssert.Contains(ex.ResponseBody, "no credits");
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => CopyleaksHttpException.CreateAsync(null));
        }

        private static async Task AssertCanceledAsync(Func<Task> call)
        {
            try
            {
                await call();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            Assert.Fail("Expected the call to be cancelled.");
        }

        /// <summary>A stream that can only be read forward, like a network stream.</summary>
        private sealed class NonSeekableStream : Stream
        {
            private readonly MemoryStream _inner;

            public NonSeekableStream(byte[] data)
            {
                _inner = new MemoryStream(data);
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
            public override void Flush() { }

            // Return short reads to exercise the chunk handling.
            public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, Math.Min(count, 1000));
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        /// <summary>
        /// A stream that can only be read asynchronously, like a web server request body when synchronous IO is not allowed.
        /// </summary>
        private sealed class AsyncOnlyStream : Stream
        {
            private readonly MemoryStream _inner;
            private readonly bool _stall;

            public AsyncOnlyStream(byte[] data, bool stall = false)
            {
                _inner = new MemoryStream(data);
                _stall = stall;
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) => throw new InvalidOperationException("Synchronous reads are not allowed.");
            public override int Read(Span<byte> buffer) => throw new InvalidOperationException("Synchronous reads are not allowed.");

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                => ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

            public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                if (_stall)
                    await Task.Delay(Timeout.Infinite, cancellationToken);
                await Task.Yield();
                // Short reads exercise the chunk handling.
                return _inner.Read(buffer.Span.Slice(0, Math.Min(buffer.Length, 1000)));
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        /// <summary>A response body whose headers arrived but whose bytes never do.</summary>
        private sealed class StalledContent : HttpContent
        {
            protected override Task<Stream> CreateContentReadStreamAsync() => Task.FromResult<Stream>(new AsyncOnlyStream(Array.Empty<byte>(), stall: true));
            protected override Task<Stream> CreateContentReadStreamAsync(CancellationToken cancellationToken) => CreateContentReadStreamAsync();
            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) => throw new NotSupportedException();

            protected override bool TryComputeLength(out long length)
            {
                length = 0;
                return false;
            }
        }
    }
}
