using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CopyleaksAPITests
{
    [TestClass]
    public class ImageDetectionTests
    {
        public const string USER_EMAIL = "<EMAIL>";
        public const string USER_KEY = "<API KEY>";

        private HttpClient Client { get; set; }
        private CopyleaksIdentityApi IdentityClient { get; set; }
        private CopyleaksAiImageDetectionApi ImageDetectionClient { get; set; }

        public ImageDetectionTests()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            Client = new HttpClient(handler);
            IdentityClient = new CopyleaksIdentityApi(Client);
            ImageDetectionClient = new CopyleaksAiImageDetectionApi(Client);
        }

        [TestMethod]
        public async Task SUBMIT()
        {
            var loginResponse = await IdentityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
            var authToken = loginResponse.Token;
            string scanId = Guid.NewGuid().ToString();


            string imagePath = @"PATH TO YOUR IMAGE";
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            string base64Image = Convert.ToBase64String(imageBytes);

            var model = new CopyleaksAiImageDetectionRequestModel(
                 base64: base64Image,
                 fileName: "my-image.png",
                 model: CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA,
                 sandbox: true
            );

            var result = await ImageDetectionClient.SubmitAsync(scanId, model, authToken).ConfigureAwait(false);

            Assert.IsNotNull(result, "Result should not be null.");
        }

    }
}
