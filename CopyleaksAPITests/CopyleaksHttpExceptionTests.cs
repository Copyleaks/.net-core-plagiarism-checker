using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Models.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CopyleaksAPITests
{
    /// <summary>Parsing of the API error envelope into CopyleaksHttpException.</summary>
    [TestClass]
    public class CopyleaksHttpExceptionTests
    {
        private static Task<CopyleaksHttpException> ExceptionFor(string body, HttpStatusCode status = HttpStatusCode.BadRequest)
        {
            var response = new HttpResponseMessage(status) { Content = new StringContent(body ?? string.Empty) };
            return CopyleaksHttpException.CreateAsync(response);
        }

        private static void AssertNoEnvelope(CopyleaksHttpException ex)
        {
            Assert.IsNull(ex.ErrorType);
            Assert.IsNull(ex.ErrorId);
            Assert.IsNull(ex.ErrorCode);
            Assert.IsNull(ex.ErrorMessage);
            Assert.IsNull(ex.ErrorUrl);
            Assert.IsNotNull(ex.ErrorDetails);
            Assert.AreEqual(0, ex.ErrorDetails.Count);
        }

        [TestMethod]
        public async Task Full_envelope_is_parsed()
        {
            const string body = "{\"error\":{\"type\":\"invalid_request_error\",\"id\":\"missing_parameter\",\"code\":1," +
                "\"message\":\"Bad request. One or several required parameters are missing or incorrect.\"," +
                "\"url\":\"https://docs.copyleaks.com/using-the-apis/api-errors/#error-reference\"," +
                "\"details\":[{\"param\":\"labels\",\"message\":\"Id Label EXAMPLE-v1 is not supported\"}]}}";

            var ex = await ExceptionFor(body);

            Assert.AreEqual(HttpStatusCode.BadRequest, ex.HttpErrorCode);
            Assert.AreEqual(CopyleaksErrorTypes.INVALID_REQUEST_ERROR, ex.ErrorType);
            Assert.AreEqual("missing_parameter", ex.ErrorId);
            Assert.AreEqual(1, ex.ErrorCode);
            Assert.AreEqual("Bad request. One or several required parameters are missing or incorrect.", ex.ErrorMessage);
            Assert.AreEqual("https://docs.copyleaks.com/using-the-apis/api-errors/#error-reference", ex.ErrorUrl);
            Assert.AreEqual(1, ex.ErrorDetails.Count);
            Assert.AreEqual("labels", ex.ErrorDetails[0].Param);
            Assert.AreEqual("Id Label EXAMPLE-v1 is not supported", ex.ErrorDetails[0].Message);
            // The exception message and the raw body are unchanged.
            Assert.AreEqual(body, ex.Message);
            Assert.AreEqual(body, ex.ResponseBody);
        }

        [TestMethod]
        public async Task Missing_optional_fields_are_null_and_details_empty()
        {
            var ex = await ExceptionFor("{\"error\":{\"type\":\"payment_error\",\"id\":\"insufficient_credits\",\"code\":2,\"message\":\"no credits\"}}",
                HttpStatusCode.PaymentRequired);

            Assert.AreEqual(CopyleaksErrorTypes.PAYMENT_ERROR, ex.ErrorType);
            Assert.AreEqual("insufficient_credits", ex.ErrorId);
            Assert.AreEqual(2, ex.ErrorCode);
            Assert.AreEqual("no credits", ex.ErrorMessage);
            Assert.IsNull(ex.ErrorUrl);
            Assert.AreEqual(0, ex.ErrorDetails.Count);

            var empty = await ExceptionFor("{\"error\":{}}");
            AssertNoEnvelope(empty);
        }

        [TestMethod]
        public async Task Code_as_numeric_string_is_parsed_and_other_codes_are_ignored()
        {
            Assert.AreEqual(409, (await ExceptionFor("{\"error\":{\"code\":\"409\"}}")).ErrorCode);
            Assert.IsNull((await ExceptionFor("{\"error\":{\"code\":\"abc\"}}")).ErrorCode);
            Assert.IsNull((await ExceptionFor("{\"error\":{\"code\":1.5}}")).ErrorCode);
            Assert.IsNull((await ExceptionFor("{\"error\":{\"code\":true}}")).ErrorCode);
            Assert.IsNull((await ExceptionFor("{\"error\":{\"code\":null}}")).ErrorCode);
        }

        [TestMethod]
        public async Task Non_json_body_leaves_every_field_empty()
        {
            var ex = await ExceptionFor("<html>Bad Gateway</html>", HttpStatusCode.BadGateway);
            AssertNoEnvelope(ex);
            Assert.AreEqual("<html>Bad Gateway</html>", ex.Message);

            AssertNoEnvelope(await ExceptionFor(""));
            AssertNoEnvelope(await ExceptionFor("{\"error\":{\"type\":\"api_error\""));
            AssertNoEnvelope(await ExceptionFor("{\"error\":{\"type\":\"api_error\"}} trailing"));
            AssertNoEnvelope(await ExceptionFor("[{\"error\":{\"type\":\"api_error\"}}]"));
        }

        [TestMethod]
        public async Task Json_without_an_error_object_leaves_every_field_empty()
        {
            AssertNoEnvelope(await ExceptionFor("{\"message\":\"Unauthorized\"}", HttpStatusCode.Unauthorized));
            AssertNoEnvelope(await ExceptionFor("{\"error\":\"bad\"}"));
            AssertNoEnvelope(await ExceptionFor("{\"error\":null}"));
            AssertNoEnvelope(await ExceptionFor("{\"error\":[{\"type\":\"api_error\"}]}"));
        }

        [TestMethod]
        public async Task Malformed_details_and_non_string_fields_are_skipped()
        {
            var notArray = await ExceptionFor("{\"error\":{\"type\":\"api_error\",\"details\":{\"param\":\"x\"}}}");
            Assert.AreEqual(CopyleaksErrorTypes.API_ERROR, notArray.ErrorType);
            Assert.AreEqual(0, notArray.ErrorDetails.Count);

            var mixed = await ExceptionFor("{\"error\":{\"type\":{\"nested\":true},\"id\":5,\"message\":\"still read\"," +
                "\"details\":[\"text\",null,7,{\"param\":3,\"message\":\"m\"},{\"param\":\"p\"},{\"message\":\"2026-11-07T10:00:00Z\"}]}}");
            Assert.IsNull(mixed.ErrorType);
            Assert.IsNull(mixed.ErrorId);
            Assert.AreEqual("still read", mixed.ErrorMessage);
            Assert.AreEqual(3, mixed.ErrorDetails.Count);
            Assert.IsNull(mixed.ErrorDetails[0].Param);
            Assert.AreEqual("m", mixed.ErrorDetails[0].Message);
            Assert.AreEqual("p", mixed.ErrorDetails[1].Param);
            Assert.IsNull(mixed.ErrorDetails[1].Message);
            Assert.AreEqual("2026-11-07T10:00:00Z", mixed.ErrorDetails[2].Message, "date-like strings stay as sent");
        }

        [TestMethod]
        public void Error_type_constants_match_the_documented_values()
        {
            Assert.AreEqual("invalid_request_error", CopyleaksErrorTypes.INVALID_REQUEST_ERROR);
            Assert.AreEqual("authentication_error", CopyleaksErrorTypes.AUTHENTICATION_ERROR);
            Assert.AreEqual("payment_error", CopyleaksErrorTypes.PAYMENT_ERROR);
            Assert.AreEqual("api_error", CopyleaksErrorTypes.API_ERROR);
        }
    }
}
