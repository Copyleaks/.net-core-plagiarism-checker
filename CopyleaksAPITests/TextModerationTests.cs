using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API;
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

            var model = new CopyleaksTextModerationRequestModel(
                 text: "This is some text to scan.",
                 sandbox: true,
                 language: "en",
                 labels: new object[]
                    {
                        new { id = "other-v1" },
                        new { id = "adult-v1" },
                        new { id = "toxic-v1" },
                        new { id = "violent-v1" },
                        new { id = "profanity-v1" },
                        new { id = "self-harm-v1" },
                        new { id = "harassment-v1" },
                        new { id = "hate-speech-v1" },
                        new { id = "drugs-v1" },
                        new { id = "firearms-v1" },
                        new { id = "cybersecurity-v1" },
                   }
            );

            var result = await new CopyleaksTextModerationApi().SubmitTextAsync(scanId, model, authToken).ConfigureAwait(false);

            Assert.IsNotNull(result, "Result should not be null.");
        }

    }
}
