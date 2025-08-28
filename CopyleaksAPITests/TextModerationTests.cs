using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Requests.TextModeration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CopyleaksAPITests
{
    [TestClass]
    public class TextModerationTests
    {
        public const string USER_EMAIL = "<EMAIL>";
        public const string USER_KEY = "<API KEY>";

        private HttpClient Client { get; set; }
        private CopyleaksIdentityApi IdentityClient { get; set; }
        private CopyleaksTextModerationApi TextModerationClient { get; set; }

        public TextModerationTests()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            Client = new HttpClient(handler);
            IdentityClient = new CopyleaksIdentityApi(Client);
            TextModerationClient = new CopyleaksTextModerationApi(Client);
        }

        [TestMethod]
        public async Task SUBMIT_TEXT()
        {
            var loginResponse = await IdentityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
            var authToken = loginResponse.Token;
            string scanId = Guid.NewGuid().ToString();

            var labelsArray = new object[]
            {
                new { id = CopyleaksTextModerationConstants.ADULT_V1 },
                new { id = CopyleaksTextModerationConstants.TOXIC_V1 },
                new { id = CopyleaksTextModerationConstants.VIOLENT_V1 },
                new { id = CopyleaksTextModerationConstants.PROFANITY_V1 },
                new { id = CopyleaksTextModerationConstants.SELF_HARM_V1 },
                new { id = CopyleaksTextModerationConstants.HARASSMENT_V1 },
                new { id = CopyleaksTextModerationConstants.HATE_SPEECH_V1 },
                new { id = CopyleaksTextModerationConstants.DRUGS_V1 },
                new { id = CopyleaksTextModerationConstants.FIREARMS_V1 },
                new { id = CopyleaksTextModerationConstants.CYBERSECURITY_V1 }
            };
            var model = new CopyleaksTextModerationRequestModel(
                 text: "This is some text to scan.",
                 sandbox: true,
                 language: CopyleaksTextModerationLanguages.ENGLISH,
                 labels: labelsArray
            );

            var result = await new CopyleaksTextModerationApi().SubmitTextAsync(scanId, model, authToken).ConfigureAwait(false);

            Assert.IsNotNull(result, "Result should not be null.");
        }

    }
}
