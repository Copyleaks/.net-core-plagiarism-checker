namespace Copyleaks.SDK.V3.API.Models.Responses
{
    public class CopyleaksErrorResponse
    {
        public short CopyleaksErrorCode { get; private set; }

        public int httpStatusCode { get; set; }
        
        public string Message { get; set; }

    }
}
