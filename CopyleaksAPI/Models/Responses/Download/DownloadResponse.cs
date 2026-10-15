using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Download
{
	public class DownloadResultResponse
	{
		[JsonProperty("statistics")]
		public FullSuspectedStatisticsSection StatisticsSection { get; set; }

		[JsonProperty("text")]
		public FullTextSuspectedVersionSection Text { get; set; }

		[JsonProperty("html")]
		public FullHtmlSuspectedVersionSection Html { get; set; }
		[JsonProperty("version")]
		public int Version { get; set; }
	}
}
