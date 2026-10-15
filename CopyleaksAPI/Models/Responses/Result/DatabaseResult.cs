using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
    public class DatabaseResult : BasicInternalResult
    {
        /// <summary>True if you uploaded the content; false if another user did.</summary>
        [JsonIgnore]
        public bool IsUploadedByMe {
            get {
                return ScanId != null;
            }
        }
    }
}