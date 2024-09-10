using Copyleaks.SDK.V3.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API.Models.Requests.WritingAssistant;

namespace CopyleaksAPITests
{
    [TestClass]
    public class WritingAssistantTests
    {
        public const string USER_EMAIL = "<EMAIL>";
        public const string USER_KEY = "<API KEY>";

        private HttpClient Client { get; set; }
        private CopyleaksIdentityApi IdentityClient { get; set; }
        private CopyleaksWritingAssistantApi WritingAssistantClient { get; set; }

        public WritingAssistantTests()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            Client = new HttpClient(handler);
            IdentityClient = new CopyleaksIdentityApi(Client);
            WritingAssistantClient = new CopyleaksWritingAssistantApi(Client);
        }

        [TestMethod]
        public async Task SUBMIT_TEXT()
        {
            var loginResponse = await IdentityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
            var authToken = loginResponse.Token;
            string scanId = Guid.NewGuid().ToString();
            var text = @"Lions are the only cat that live in groups, called pride. A prides typically consists of a few adult males, several feales, and their offspring. This social structure is essential for hunting and raising young cubs. Female lions, or lionesses are the primary hunters of the prid. They work together in cordinated groups to take down prey usually targeting large herbiores like zbras, wildebeest and buffalo. Their teamwork and strategy during hunts highlight the intelligence and coperation that are key to their survival.";

            var document = new WritingAssistantDocument()
            {
                Sandbox = true,
                Text = text,
                Score = new Score()
                {
                    SentenceStructureScoreWeight = 0.2,
                    GrammarScoreWeight = 0.3,
                    MechanicsScoreWeight = 0.4,
                    WordChoiceScoreWeight = 0.5,
                },
            };

            var res = await WritingAssistantClient.SubmitTextAsync(scanId, document, authToken);

            Assert.IsNotNull(res, "Result should not be null.");
            Assert.AreEqual("sandbox", res.ScanType, "The ScanType should be sandbox");
        }

        [TestMethod]
        public async Task GET_CORRECTION_TYPES()
        {
            var loginResponse = await IdentityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
            var authToken = loginResponse.Token;

            var res = await WritingAssistantClient.GetCorrectionTypesAsync(authToken, "en");

            Assert.IsNotNull(res, "Result should not be null.");
        }
    }
}
