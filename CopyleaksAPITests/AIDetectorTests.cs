using Copyleaks.SDK.V3.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API.Models.Requests.AIDetection;

namespace CopyleaksAPITests
{
    [TestClass]
    public class AIDetectorTests
    {
        public const string USER_EMAIL = "<EMAIL>";
        public const string USER_KEY = "<API KEY>";

        private HttpClient Client { get; set; }
        private CopyleaksIdentityApi IdentityClient { get; set; }
        private CopyleaksAIDetectionApi AIDetectionClient { get; set; }

        public AIDetectorTests()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            Client = new HttpClient(handler);
            IdentityClient = new CopyleaksIdentityApi(Client);
            AIDetectionClient = new CopyleaksAIDetectionApi(Client);
        }

        [TestMethod]
        public async Task SUBMIT_NATURAL_LANGUAGE_TEST()
        {
            var loginResponse = await IdentityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
            var authToken = loginResponse.Token;
            string scanId = Guid.NewGuid().ToString();
            var text = @"Lions are social animals, living in groups called prides, typically consisting of several females, their offspring, and a few males. Female lions are the primary hunters, working together to catch prey. Lions are known for their strength, teamwork, and complex social structures.";

            var naturalLanguageDocument = new NaturalLanguageDocument()
            {
                Text = text,
                Sandbox = true,
            };

            var res = await AIDetectionClient.SubmitNaturalLanguageAsync(scanId, naturalLanguageDocument, authToken);

            Assert.IsNotNull(res, "Result should not be null.");
            Assert.AreEqual("sandbox", res.ScanType, "The ScanType should be sandbox");
        }


        [TestMethod]
        public async Task SUBMIT_SOURCE_CODE_TEST()
        {
            var loginResponse = await IdentityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
            var authToken = loginResponse.Token;
            string scanId = Guid.NewGuid().ToString();

            var sampleCode = @"
                def add(a, b):
                    return a + b

                def multiply(a, b):
                    return a * b

                def main():
                    x = 5
                    y = 10
                    sum_result = add(x, y)
                    product_result = multiply(x, y)
                    print(f'Sum: {sum_result}')
                    print(f'Product: {product_result}')

                if __name__ == '__main__':
                    main()
                ";

            var sourceCodeDocument = new SourceCodeDocument()
            {
                Text = sampleCode,
                Filename = "example.py",
                Sandbox = true
            };

            var res = await AIDetectionClient.SubmitSourceCodeAsync(scanId, sourceCodeDocument, authToken);

            Assert.IsNotNull(res, "Result should not be null.");
            Assert.AreEqual("sandbox", res.ScanType, "The ScanType should be sandbox");
        }
    }
}
