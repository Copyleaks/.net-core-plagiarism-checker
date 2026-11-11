namespace Copyleaks.SDK.V3.API.Models.Constants
{
    /// <summary>
    /// Values of error.type in an API error response (<see cref="Exceptions.CopyleaksHttpException.ErrorType"/>).
    /// See https://docs.copyleaks.com/using-the-apis/api-errors/. Other values may be added later.
    /// </summary>
    public static class CopyleaksErrorTypes
    {
        /// <summary>The request has missing or invalid parameters.</summary>
        public const string INVALID_REQUEST_ERROR = "invalid_request_error";

        /// <summary>The credentials or the access token are missing, invalid or expired.</summary>
        public const string AUTHENTICATION_ERROR = "authentication_error";

        /// <summary>The account has a billing problem, for example not enough credits.</summary>
        public const string PAYMENT_ERROR = "payment_error";

        /// <summary>The server failed to process the request.</summary>
        public const string API_ERROR = "api_error";
    }
}
