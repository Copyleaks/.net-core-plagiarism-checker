using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
    public class ReportResponse
    {
        [JsonProperty("report")]
        public Uri DownloadEndPoint { get; set; }

        [JsonProperty("status")]
        public eReportStatus Status { get; set; }
    }

}