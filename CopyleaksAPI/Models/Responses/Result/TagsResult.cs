using Newtonsoft.Json;


namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
	public class TagsResult
	{
		[JsonProperty("code")]
		public string Code { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

        [JsonProperty("helpLink")]
        public string HelpLink { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }
	}
}
