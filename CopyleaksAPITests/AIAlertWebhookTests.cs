using System;
using System.IO;
using System.Linq;
using Copyleaks.SDK.V3.API.Models.Callbacks;
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Responses.AIDetector;
using Copyleaks.SDK.V3.API.Models.Responses.Result;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.NotificationsModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace CopyleaksAPITests
{
    /// <summary>
    /// Offline tests for reading the AI alert from the completed webhook.
    /// The fixtures in Resources/Webhooks are shared by all Copyleaks SDKs, and the
    /// expected values below are the ones listed in the shared expected.json.
    /// </summary>
    [TestClass]
    public class AIAlertWebhookTests
    {
        private const double DELTA = 1e-9;

        private static CompletedWebhookModel LoadWebhook(string fixture)
        {
            return JsonConvert.DeserializeObject<CompletedWebhookModel>(ReadFixture(fixture));
        }

        private static string ReadFixture(string fixture)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Resources", "Webhooks", fixture));
        }

        private static void AssertAIAlert(Alerts alert)
        {
            Assert.IsNotNull(alert, "The AI alert should be found.");
            Assert.AreEqual(CopyleaksAlertCodes.SUSPECTED_AI_TEXT, alert.Code);
            Assert.AreEqual(2, alert.Category);
            Assert.AreEqual(4, alert.Severity);
            Assert.IsNull(alert.HelpLink);
        }

        private static void AssertProdResult(AIDetectionResult result)
        {
            Assert.IsNotNull(result, "The AI detection result should not be null.");
            Assert.AreEqual("v11.0", result.ModelVersion);
            Assert.AreEqual(0.5217, result.Summary.AI, DELTA);
            Assert.AreEqual(0.4783, result.Summary.Human, DELTA);
            Assert.AreEqual(2, result.Results.Count);
            CollectionAssert.AreEqual(new[] { 702 }, result.Results[0].Matches[0].Text.Chars.Lengths);
            Assert.AreEqual(2, result.Results[1].Classification);
            CollectionAssert.AreEqual(new[] { 3 }, result.Results[1].Matches[0].Html.Chars.GroupIds);
            Assert.AreEqual(1, result.TranslationProvider);
            Assert.IsTrue(result.Translation.StartsWith("Hello  world", StringComparison.Ordinal));
            CollectionAssert.AreEqual(new[] { 18.386, 1.407 }, result.Explain.Patterns.Statistics.AICount);
            CollectionAssert.AreEqual(new[] { 1, 2 }, result.Explain.Patterns.Statistics.Source);
        }

        [TestMethod]
        public void PROD_AI_ALERT_RETURNS_RESULT()
        {
            var webhook = LoadWebhook("completed_prod_ai.json");

            var alert = webhook.GetAIDetectionAlert();
            AssertAIAlert(alert);
            Assert.IsTrue(alert.AdditionalData.StartsWith("{\"results\": [", StringComparison.Ordinal), "The raw additionalData should stay available.");

            AssertProdResult(webhook.GetAIDetectionResult());
            AssertProdResult(alert.GetAIDetectionResult());
        }

        [TestMethod]
        public void SANDBOX_AI_ALERT_ACCEPTS_PASCAL_CASE_KEYS()
        {
            var webhook = LoadWebhook("completed_sandbox_ai.json");
            AssertAIAlert(webhook.GetAIDetectionAlert());

            var result = webhook.GetAIDetectionResult();
            Assert.IsNotNull(result, "The AI detection result should not be null.");
            Assert.AreEqual("v8.0", result.ModelVersion);
            Assert.AreEqual(1.0, result.Summary.AI, DELTA);
            Assert.AreEqual(0.0, result.Summary.Human, DELTA);
            Assert.AreEqual(1, result.Results.Count);
            CollectionAssert.AreEqual(new[] { 0 }, result.Results[0].Matches[0].Text.Chars.Starts);
            CollectionAssert.AreEqual(new[] { 1453 }, result.Results[0].Matches[0].Text.Chars.Lengths);
            CollectionAssert.AreEqual(new[] { 230 }, result.Results[0].Matches[0].Text.Words.Lengths);
        }

        [TestMethod]
        public void NUL_TAIL_AI_ALERT_RETURNS_SAME_RESULT_AS_PROD()
        {
            var webhook = LoadWebhook("completed_nul_tail_ai.json");
            AssertAIAlert(webhook.GetAIDetectionAlert());
            AssertProdResult(webhook.GetAIDetectionResult());
        }

        [TestMethod]
        public void EMPTY_AI_ALERT_DATA_RETURNS_NULL()
        {
            var webhook = LoadWebhook("completed_empty_ai.json");
            AssertAIAlert(webhook.GetAIDetectionAlert());
            Assert.IsNull(webhook.GetAIDetectionResult());
        }

        [TestMethod]
        public void MISSING_AI_ALERT_DATA_RETURNS_NULL()
        {
            var webhook = LoadWebhook("completed_missing_data_ai.json");
            AssertAIAlert(webhook.GetAIDetectionAlert());
            Assert.IsNull(webhook.GetAIDetectionResult());
        }

        [TestMethod]
        public void NO_AI_ALERT_RETURNS_NULL()
        {
            var webhook = LoadWebhook("completed_no_ai.json");
            Assert.IsNull(webhook.GetAIDetectionAlert());
            Assert.IsNull(webhook.GetAIDetectionResult());

            // The non-AI alert's own helper returns null as well.
            var otherAlert = webhook.Notifications.Alerts.Single();
            Assert.AreEqual("suspected-character-replacement", otherAlert.Code);
            Assert.IsNull(otherAlert.GetAIDetectionResult());
        }

        [TestMethod]
        public void NON_AI_ALERT_WITH_AI_DATA_RETURNS_NULL()
        {
            // Real AI data from the prod fixture, so only the alert code decides the outcome.
            var aiData = LoadWebhook("completed_prod_ai.json").GetAIDetectionAlert().AdditionalData;

            // Other codes, a different-case code (the match is ordinal) and a missing code all return null.
            var otherCodes = new[] { CopyleaksAlertCodes.AI_DETECTION_FAILED, "suspected-character-replacement", "Suspected-AI-Text", null };
            foreach (var code in otherCodes)
            {
                var alert = new Alerts { Code = code, AdditionalData = aiData };
                Assert.IsNull(alert.GetAIDetectionResult(), $"Alerts with code '{code}' should return null.");

                var legacyAlert = new AlertNotification { Code = code, AdditionalData = aiData };
                Assert.IsNull(legacyAlert.GetAIDetectionResult(), $"AlertNotification with code '{code}' should return null.");
            }

            // Control: the same data under the AI code is decoded.
            AssertProdResult(new Alerts { Code = CopyleaksAlertCodes.SUSPECTED_AI_TEXT, AdditionalData = aiData }.GetAIDetectionResult());
            AssertProdResult(new AlertNotification { Code = CopyleaksAlertCodes.SUSPECTED_AI_TEXT, AdditionalData = aiData }.GetAIDetectionResult());
        }

        [TestMethod]
        public void NO_NOTIFICATIONS_RETURNS_NULL()
        {
            var webhook = LoadWebhook("completed_no_notifications.json");
            Assert.IsNull(webhook.Notifications);
            Assert.IsNull(webhook.GetAIDetectionAlert());
            Assert.IsNull(webhook.GetAIDetectionResult());
        }

        [TestMethod]
        public void MALFORMED_AI_ALERT_DATA_THROWS_JSON_EXCEPTION()
        {
            var webhook = LoadWebhook("completed_malformed_ai.json");
            AssertAIAlert(webhook.GetAIDetectionAlert());

            JsonException thrown = null;
            try
            {
                webhook.GetAIDetectionResult();
            }
            catch (JsonException ex)
            {
                thrown = ex;
            }
            Assert.IsNotNull(thrown, "Malformed additionalData should throw a Newtonsoft JsonException.");
        }

        [TestMethod]
        public void HOST_JSON_DEFAULT_SETTINGS_ARE_NOT_USED()
        {
            var alert = new Alerts
            {
                Code = CopyleaksAlertCodes.SUSPECTED_AI_TEXT,
                AdditionalData = "{\"results\": [], \"summary\": {\"human\": 0.25, \"ai\": 0.75}, \"modelVersion\": \"v11.0\", \"fieldAddedLater\": 1}"
            };

            var previousDefaults = JsonConvert.DefaultSettings;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };
            try
            {
                // The host setting would reject the unknown field...
                Assert.ThrowsException<JsonSerializationException>(() => JsonConvert.DeserializeObject<AIDetectionResult>(alert.AdditionalData));

                // ...but the helper uses its own serializer.
                var result = alert.GetAIDetectionResult();
                Assert.IsNotNull(result);
                Assert.AreEqual(0.75, result.Summary.AI, DELTA);
            }
            finally
            {
                JsonConvert.DefaultSettings = previousDefaults;
            }
        }

        [TestMethod]
        public void LEGACY_COMPLETED_CALLBACK_ALERT_RETURNS_RESULT()
        {
            var callback = JsonConvert.DeserializeObject<CompletedCallback>(ReadFixture("completed_prod_ai.json"));

            var aiAlert = callback.Notifications.Alerts.Single(alert => alert.Code == CopyleaksAlertCodes.SUSPECTED_AI_TEXT);
            AssertProdResult(aiAlert.GetAIDetectionResult());

            var otherAlert = callback.Notifications.Alerts.Single(alert => alert.Code != CopyleaksAlertCodes.SUSPECTED_AI_TEXT);
            Assert.IsNull(otherAlert.GetAIDetectionResult());
        }
    }
}
