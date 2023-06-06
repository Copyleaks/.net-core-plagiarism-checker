using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
	public class ResultMetaData
	{
		[JsonProperty("finalUrl")]
		public string FinalUrl { get; set; }

		[JsonProperty("canonicalUrl")]
		public string CanonicalUrl { get; set; }

		[JsonProperty("author")]
		public string Author { get; set; }

		[JsonProperty("organization")]
		public string Organization { get; set; }

		[JsonProperty("filename")]
		public string Filename { get; set; }

		[JsonProperty("publishDate")]
		public string PublishDate { get; set; }

		[JsonProperty("creationDate")]
		public string CreationDate { get; set; }

		[JsonProperty("lastModificationDate")]
		public string LastModificationDate { get; set; }

		[JsonProperty("submissionDate")]
		public string SubmissionDate { get; set; }

		[JsonProperty("submittedBy")]
		public string SubmittedBy { get; set; }

        [JsonProperty("submitterAuthorId")]
        public string SubmitterAuthorId { get; set; }

        [JsonProperty("customMetadata")]
        public CustomMetadata[] CustomMetadata { get; set; }
	}
}
