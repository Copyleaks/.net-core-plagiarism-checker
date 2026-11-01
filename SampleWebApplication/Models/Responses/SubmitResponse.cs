namespace Copyleaks.SDK.Demo.Models.Responses
{
    public class SubmitResponse : BaseResponse
    {
        public string ScanId { get; set; }

        public string Token { get; set; }
        public uint ClientCredits { get; set; }
    }
}
