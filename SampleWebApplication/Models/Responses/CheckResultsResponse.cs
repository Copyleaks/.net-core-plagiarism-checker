using Copyleaks.SDK.V3.API.Models.Callbacks;

namespace Copyleaks.SDK.Demo.Models.Responses
{
    public class CheckResultsResponse: BaseResponse
    {
        public CompletedCallback CompletedCallback { get; set; }

        public string ScanId { get; set; }
    }
}
