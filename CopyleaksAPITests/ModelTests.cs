using System;
using System.Linq;
using Copyleaks.SDK.V3.API.Models.Callbacks;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Copyleaks.SDK.V3.API.Models.Requests.WritingAssistant;
using Copyleaks.SDK.V3.API.Models.Responses;
using Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection;
using Copyleaks.SDK.V3.API.Models.Responses.AiVideoDetection;
using Copyleaks.SDK.V3.API.Models.Responses.Download;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks;
using Copyleaks.SDK.V3.API.Models.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CopyleaksAPITests
{
    /// <summary>
    /// Offline tests of the request and response models against the API reference.
    /// </summary>
    [TestClass]
    public class ModelTests
    {
        private static JObject Serialize(object value) => JObject.Parse(JsonConvert.SerializeObject(value));

        // ---------------- Scan properties ----------------

        [TestMethod]
        public void Default_scan_properties_leave_server_defaults_alone()
        {
            var properties = Serialize(new ClientScanProperties());

            Assert.IsNull(properties["priority"], "priority is omitted so the server default applies");
            Assert.AreEqual(2880, properties["expiration"].Value<int>(), "the API default and maximum retention");
            Assert.IsNull(properties["pdf"]["version"], "the deprecated pdf.version is not sent by default");
            Assert.IsNull(properties["pdf"]["Version"]);
            Assert.IsNull(properties["pdf"]["reportVersion"]);
            foreach (string name in new[] { "writingFeedback", "overview", "internalAiSourceMatch", "references", "scanTimeZone",
                                            "course", "assignment", "institution", "aiSourceMatch", "displayLanguage" })
                Assert.IsNull(properties[name], $"{name} is omitted when unset");
            Assert.IsNull(properties["aiGeneratedText"]["sensitivity"]);
            Assert.IsNull(properties["aiGeneratedText"]["explain"]);
            Assert.IsNull(properties["scanning"]["include"]);
            Assert.IsNull(properties["scanning"]["exclude"]["backlinksDomains"]);
            Assert.IsNull(properties["scanning"]["exclude"]["text"]);
        }

        [TestMethod]
        public void New_scan_properties_use_the_documented_json_names()
        {
            var scan = new ClientScanProperties
            {
                Priority = eScanPriority.High,
                ScanTimeZone = "America/New_York",
                Course = new CourseEntity { Id = "course-1" },
                Assignment = new AssignmentEntity { Id = "assignment-1" },
                Institution = new InstitutionEntity { Id = "institution-1" },
                AIGeneratedText = new AIGeneratedText { Detect = true, Sensitivity = 3, Explain = new AIExplainSettings { Enable = true } },
                WritingFeedback = new WritingFeedbackSettings { Enable = true, Score = new Score { WordChoiceScoreWeight = 0.25 } },
                Overview = new OverviewSettings { Enable = true, IgnoreAIDetection = true, IgnorePlagiarismDetection = false, IgnoreWritingFeedback = true, IgnoreAuthorData = true },
                InternalAISourceMatch = new InternalAISourceMatch { Enable = true },
                AISourceMatch = new AISourceMatch { Enable = true },
                References = new ReferencesSettings { Validate = true },
            };
            scan.ProtectionLayersSection.Include = new ScanningInclude { IdPattern = "abc*" };
            scan.ProtectionLayersSection.Exclude.IdPattern = "old*";
            scan.ProtectionLayersSection.Exclude.BacklinksDomains = new[] { "example.com" };
            scan.ProtectionLayersSection.Exclude.Text = new[] { "lorem ipsum" };
            scan.ReportSection.ReportVersion = "v3";
            scan.ReportSection.Colors = new ReportCustomizationColors { AIDetection = "#FE5758", WritingFeedback = "#FF9A02" };

            var json = Serialize(scan);

            Assert.AreEqual(3, json["priority"].Value<int>());
            Assert.AreEqual("America/New_York", json["scanTimeZone"].Value<string>());
            Assert.AreEqual("course-1", json["course"]["id"].Value<string>());
            Assert.AreEqual("assignment-1", json["assignment"]["id"].Value<string>());
            Assert.AreEqual("institution-1", json["institution"]["id"].Value<string>());
            Assert.IsTrue(JToken.DeepEquals(JObject.Parse("{\"detect\":true,\"sensitivity\":3,\"explain\":{\"enable\":true}}"), json["aiGeneratedText"]));
            Assert.IsTrue(JToken.DeepEquals(JObject.Parse(
                "{\"enable\":true,\"score\":{\"grammarScoreWeight\":1.0,\"mechanicsScoreWeight\":1.0,\"sentenceStructureScoreWeight\":1.0,\"wordChoiceScoreWeight\":0.25}}"),
                json["writingFeedback"]));
            Assert.IsTrue(JToken.DeepEquals(JObject.Parse(
                "{\"enable\":true,\"ignoreAIDetection\":true,\"ignorePlagiarismDetection\":false,\"ignoreWritingFeedback\":true,\"ignoreAuthorData\":true}"),
                json["overview"]));
            Assert.IsTrue(JToken.DeepEquals(JObject.Parse("{\"enable\":true}"), json["internalAiSourceMatch"]));
            Assert.IsTrue(JToken.DeepEquals(JObject.Parse("{\"enable\":true}"), json["aiSourceMatch"]));
            Assert.IsTrue(JToken.DeepEquals(JObject.Parse("{\"validate\":true}"), json["references"]));
            Assert.AreEqual("abc*", json["scanning"]["include"]["idPattern"].Value<string>());
            Assert.AreEqual("old*", json["scanning"]["exclude"]["idPattern"].Value<string>());
            CollectionAssert.AreEqual(new[] { "example.com" }, json["scanning"]["exclude"]["backlinksDomains"].ToObject<string[]>());
            CollectionAssert.AreEqual(new[] { "lorem ipsum" }, json["scanning"]["exclude"]["text"].ToObject<string[]>());
            Assert.AreEqual("v3", json["pdf"]["reportVersion"].Value<string>());
            Assert.AreEqual("#FE5758", json["pdf"]["Colors"]["aiDetection"].Value<string>());
            Assert.AreEqual("#FF9A02", json["pdf"]["Colors"]["writingFeedback"].Value<string>());
        }

        [TestMethod]
        public void Deprecated_pdf_version_is_sent_only_when_set()
        {
#pragma warning disable CS0618 // Version is obsolete; this test checks it still works.
            var report = new ReportCustomization { Create = true, Version = ePdfReportVersion.V3 };
#pragma warning restore CS0618
            Assert.AreEqual(3, Serialize(report)["version"].Value<int>());
            Assert.IsNull(Serialize(new ReportCustomization { Create = true })["version"]);
        }

        [TestMethod]
        public void Writing_score_weights_default_to_one()
        {
            var score = new Score();
            Assert.AreEqual(1.0, score.GrammarScoreWeight);
            Assert.AreEqual(1.0, score.MechanicsScoreWeight);
            Assert.AreEqual(1.0, score.SentenceStructureScoreWeight);
            Assert.AreEqual(1.0, score.WordChoiceScoreWeight);
        }

        // ---------------- Webhooks ----------------

        [TestMethod]
        public void New_result_webhook_keeps_developer_payload_repository_metadata_and_internal_ai_data()
        {
            const string json = @"{
                ""score"": { ""aggregatedScore"": 12.5 },
                ""developerPayload"": ""my-payload"",
                ""internet"": [ { ""id"": ""i1"", ""url"": ""https://example.com"", ""matchedWords"": 3 } ],
                ""repositories"": [ {
                    ""id"": ""r1"", ""repositoryId"": ""repo-1"", ""matchedWords"": 4, ""scanId"": ""old-scan"",
                    ""metadata"": { ""filename"": ""essay.docx"", ""submittedBy"": ""teacher@example.com"" }
                } ],
                ""internalAIData"": [ {
                    ""id"": ""ai1"", ""title"": ""t"", ""introduction"": ""i"", ""matchedWords"": 10, ""identicalWords"": 5,
                    ""similarWords"": 3, ""paraphrasedWords"": 2, ""totalWords"": 100,
                    ""tags"": [ { ""code"": ""internal-ai-source-match"", ""title"": ""AI-Generated"", ""description"": ""d"" } ]
                } ]
            }";

            var webhook = JsonConvert.DeserializeObject<NewResultWebhookModel>(json);

            Assert.AreEqual("my-payload", webhook.DeveloperPayload);
            Assert.AreEqual(12.5m, webhook.Score.AggregatedScore);
            Assert.AreEqual("repo-1", webhook.Repositories[0].RepositoryId);
            Assert.AreEqual("teacher@example.com", webhook.Repositories[0].Metadata.SubmittedBy);
            Assert.AreEqual("essay.docx", webhook.Repositories[0].Metadata.Filename);
            Assert.AreEqual(1, webhook.InternalAIData.Length);
            Assert.AreEqual(2, webhook.InternalAIData[0].ParaphrasedWords);
            Assert.AreEqual("internal-ai-source-match", webhook.InternalAIData[0].Tags[0].Code);
        }

        [TestMethod]
        public void Error_webhook_keeps_every_error_field()
        {
            const string json = @"{
                ""status"": 1,
                ""developerPayload"": ""dp"",
                ""error"": {
                    ""type"": ""invalid_request_error"", ""id"": ""missing_parameter"", ""code"": 1,
                    ""message"": ""Bad request."", ""url"": ""https://docs.copyleaks.com/errors/missing_parameter"",
                    ""details"": [ { ""param"": ""properties.webhooks.status"", ""message"": ""required"" } ]
                }
            }";

            var webhook = JsonConvert.DeserializeObject<ErrorWebhookModel>(json);

            Assert.AreEqual(1, webhook.Status);
            Assert.AreEqual("dp", webhook.DeveloperPayload);
            Assert.AreEqual("invalid_request_error", webhook.error.Type);
            Assert.AreEqual("missing_parameter", webhook.error.Id);
            Assert.AreEqual(1, webhook.error.Code);
            Assert.AreEqual("Bad request.", webhook.error.Message);
            Assert.AreEqual("https://docs.copyleaks.com/errors/missing_parameter", webhook.error.Url);
            Assert.AreEqual("properties.webhooks.status", webhook.error.Details[0].Param);
            Assert.AreEqual("required", webhook.error.Details[0].Message);
        }

        [TestMethod]
        public void Completed_webhook_has_references_validation_and_internal_ai_data()
        {
            const string json = @"{
                ""status"": 0,
                ""scannedDocument"": { ""scanId"": ""s1"", ""totalWords"": 10 },
                ""results"": {
                    ""internet"": [], ""database"": [], ""batch"": [], ""repositories"": [],
                    ""internalAIData"": [ { ""id"": ""x"", ""matchedWords"": 7, ""totalWords"": 70 } ],
                    ""score"": { ""aggregatedScore"": 0 }
                },
                ""referencesValidation"": { ""summary"": { ""total"": 4, ""academic"": 3, ""academicValidated"": 2, ""nonAcademicValidated"": 1 } }
            }";

            var webhook = JsonConvert.DeserializeObject<CompletedWebhookModel>(json);

            Assert.AreEqual(7, webhook.Results.InternalAIData[0].MatchedWords);
            Assert.AreEqual(4, webhook.ReferencesValidation.Summary.Total);
            Assert.AreEqual(3, webhook.ReferencesValidation.Summary.Academic);
            Assert.AreEqual(2, webhook.ReferencesValidation.Summary.AcademicValidated);
            Assert.AreEqual(1, webhook.ReferencesValidation.Summary.NonAcademicValidated);
            Assert.IsNull(webhook.ReferencesValidation.Results);
        }

        [TestMethod]
        public void Export_completed_webhook_accepts_a_null_status_code()
        {
            const string json = @"{ ""completed"": false, ""developerPayload"": ""dp"",
                ""tasks"": [ { ""endpoint"": ""https://example.com/a"", ""isHealthy"": false, ""httpStatusCode"": null },
                             { ""endpoint"": ""https://example.com/b"", ""isHealthy"": true, ""httpStatusCode"": 200 } ] }";

            var webhook = JsonConvert.DeserializeObject<ExportCompletedWebhook>(json);
            Assert.IsNull(webhook.Tasks[0].HttpStatusCode);
            Assert.AreEqual(200, webhook.Tasks[1].HttpStatusCode);

            var legacy = JsonConvert.DeserializeObject<ExportCompletedCallback>(json);
            Assert.IsNull(legacy.Tasks.First().HttpStatusCode);
            Assert.AreEqual(200u, legacy.Tasks.Last().HttpStatusCode);
        }

        // ---------------- Responses and export files ----------------

        [TestMethod]
        public void Ai_image_response_has_detection_flag_objects_labels_and_content_summary()
        {
            const string json = @"{
                ""model"": ""ai-image-1-ultra"",
                ""result"": { ""starts"": [0, 512], ""lengths"": [256, 512], ""labels"": [4, 2] },
                ""summary"": { ""human"": 0.3, ""ai"": 0.7 },
                ""isAiDetected"": true,
                ""imageInfo"": { ""shape"": { ""height"": 1024, ""width"": 768 },
                                 ""metadata"": { ""issuedBy"": ""OpenAI"", ""contentSummary"": ""Created using generative AI"" } },
                ""detectedObjects"": [ { ""object"": ""face"", ""mask"": { ""starts"": [1024], ""lengths"": [512] } } ],
                ""scannedDocument"": { ""scanId"": ""img1"", ""actualCredits"": 1, ""expectedCredits"": 1 }
            }";

            var response = JsonConvert.DeserializeObject<CopyleaksAiImageDetectionResponseModel>(json);

            Assert.IsTrue(response.IsAiDetected.Value);
            CollectionAssert.AreEqual(new[] { 4, 2 }, response.Result.Labels);
            Assert.AreEqual("Created using generative AI", response.ImageInfo.Metadata.ContentSummary);
            Assert.AreEqual("face", response.DetectedObjects[0].Object);
            CollectionAssert.AreEqual(new[] { 1024 }, response.DetectedObjects[0].Mask.Starts);
            CollectionAssert.AreEqual(new[] { 512 }, response.DetectedObjects[0].Mask.Lengths);
        }

        [TestMethod]
        public void Ai_video_webhook_payload()
        {
            const string json = @"{
                ""model"": ""ai-video-1-pro"",
                ""audioResult"": { ""starts"": [13000], ""lengths"": [14000], ""exclude"": { ""starts"": [0], ""lengths"": [2950] } },
                ""visualResult"": { ""starts"": [11566], ""lengths"": [6134], ""exclude"": { ""starts"": [], ""lengths"": [] } },
                ""summary"": { ""audioAIRatio"": 0.4902, ""visualAIRatio"": 0.5817, ""overallAIRatio"": 0.7487 },
                ""isAiDetected"": true,
                ""videoInfo"": { ""metadata"": { ""issuedBy"": ""OpenAI"", ""appOrDeviceUsed"": ""Sora"", ""contentSummary"": ""Created using generative AI"" }, ""duration"": 55.7 },
                ""scannedVideo"": { ""scanId"": ""my-video-scan-1"", ""actualCredits"": 1, ""expectedCredits"": 1, ""creationTime"": ""2026-05-05T12:37:50Z"" }
            }";

            var payload = JsonConvert.DeserializeObject<CopyleaksAiVideoDetectionResponseModel>(json);

            Assert.AreEqual("ai-video-1-pro", payload.Model);
            Assert.AreEqual(13000, payload.AudioResult.Starts[0]);
            Assert.AreEqual(2950, payload.AudioResult.Exclude.Lengths[0]);
            Assert.AreEqual(0, payload.VisualResult.Exclude.Starts.Count);
            Assert.AreEqual(0.7487, payload.Summary.OverallAIRatio);
            Assert.IsTrue(payload.IsAiDetected.Value);
            Assert.AreEqual(55.7, payload.VideoInfo.Duration);
            Assert.AreEqual("Sora", payload.VideoInfo.Metadata.AppOrDeviceUsed);
            Assert.AreEqual("my-video-scan-1", payload.ScannedVideo.ScanId);
        }

        [TestMethod]
        public void Crawled_version_has_citation_checker_results()
        {
            const string json = @"{
                ""metadata"": { ""words"": 30, ""excluded"": 2 },
                ""referencesValidation"": {
                    ""summary"": { ""total"": 2, ""academic"": 1, ""academicValidated"": 1, ""nonAcademicValidated"": 0 },
                    ""results"": [ {
                        ""type"": ""academic"",
                        ""textRanges"": { ""starts"": [131], ""lengths"": [131] },
                        ""htmlRanges"": { ""starts"": [240], ""lengths"": [131], ""groupIds"": [0] },
                        ""parsed"": { ""title"": ""Attention Is All You Need"", ""authors"": [""Vaswani, A.""], ""year"": 2017, ""journal"": ""NeurIPS"" },
                        ""suggestions"": [ { ""url"": ""https://doi.org/10.48550/arXiv.1706.03762"", ""doi"": ""10.48550/arXiv.1706.03762"",
                                            ""title"": ""Attention Is All You Need"", ""authors"": [""Vaswani, A.""], ""publishedYear"": 2017, ""modifiedYear"": 2023,
                                            ""journal"": ""NeurIPS"", ""signals"": { ""title"": true, ""year"": true } } ]
                    } ]
                }
            }";

            var crawled = JsonConvert.DeserializeObject<DownloadSourceResponse>(json);

            var reference = crawled.ReferencesValidation.Results[0];
            Assert.AreEqual(2, crawled.ReferencesValidation.Summary.Total);
            Assert.AreEqual("academic", reference.Type);
            Assert.AreEqual(131, reference.TextRanges.Starts[0]);
            Assert.AreEqual(0, reference.HtmlRanges.GroupIds[0]);
            Assert.AreEqual(2017, reference.Parsed.Year);
            Assert.AreEqual(2023, reference.Suggestions[0].ModifiedYear);
            Assert.IsTrue(reference.Suggestions[0].Signals.Title.Value);
            Assert.IsNull(reference.Suggestions[0].Signals.Authors, "an unchecked signal stays null");
        }

        [TestMethod]
        public void Overview_export_file()
        {
            var overview = JsonConvert.DeserializeObject<OverviewResult>("{\"overview\":\"### Summary\",\"modelVersion\":\"v1\"}");
            Assert.AreEqual("### Summary", overview.Overview);
            Assert.AreEqual("v1", overview.ModelVersion);
        }

        [TestMethod]
        public void Deprecated_probability_still_reads()
        {
            var result = JsonConvert.DeserializeObject<Copyleaks.SDK.V3.API.Models.Responses.AIDetector.ClassificationResult>("{\"classification\":2,\"probability\":0.9}");
#pragma warning disable CS0618 // Probability is obsolete; this test checks it still reads.
            Assert.AreEqual(0.9, result.Probability);
#pragma warning restore CS0618
        }
    }
}
