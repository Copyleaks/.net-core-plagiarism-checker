using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
	/// <summary>The scan result</summary>
	public class Result : BaseResponse
	{
		[JsonProperty("scannedDocument")]
		public ScannedDocumentSection ScannedDocument { get; set; }

		[JsonProperty("results")]
		public ResultsSection Results { get; set; }
	}
}
