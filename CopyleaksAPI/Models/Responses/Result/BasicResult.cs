using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
    public class BasicResult
    {
        /// <summary>The result id</summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>Document Title</summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>Document Description</summary>
        [JsonProperty("introduction")]
        public string Introduction { get; set; }

        /// <summary>Total matched words</summary>
        [JsonProperty("matchedWords")]
        public uint MatchedWords { get; set; }

        [JsonProperty("identicalWords")]
        public uint IdenticalWords { get; set; }

        [JsonProperty("similarWords")]
        public uint SimilarWords { get; set; }

        [JsonProperty("paraphrasedWords")]
        public uint ParaphrasedWords { get; set; }

        /// <summary>Result meta data</summary>
        [JsonProperty("metadata")]
        public ResultMetaData MetaData { get; set; }

        /// <summary>Result tags</summary>
		[JsonProperty("tags")]
		public TagsResult[] Tags { get; set; }

	}

}