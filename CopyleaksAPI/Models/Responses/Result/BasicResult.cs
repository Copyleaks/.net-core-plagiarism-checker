﻿/********************************************************************************
 The MIT License(MIT)
 
 Copyright(c) 2016 Copyleaks LTD (https://copyleaks.com)
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
********************************************************************************/

using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Responses.Result
{
    public class BasicResult
    {
        /// <summary>
        /// The result id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Document Title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Document Description
        /// </summary>
        [JsonProperty("introduction")]
        public string Introduction { get; set; }

        /// <summary>
        /// Total matched words
        /// </summary>
        [JsonProperty("matchedWords")]
        public uint MatchedWords { get; set; }

        [JsonProperty("identicalWords")]
        public uint IdenticalWords { get; set; }

        [JsonProperty("similarWords")]
        public uint SimilarWords { get; set; }

        [JsonProperty("paraphrasedWords")]
        public uint ParaphrasedWords { get; set; }

        /// <summary>
        /// Result meta data
        /// </summary>
        [JsonProperty("metadata")]
        public ResultMetaData MetaData { get; set; }

        /// <summary>
        /// Result tags
        /// </summary>
		[JsonProperty("tags")]
		public TagsResult[] Tags { get; set; }

	}

}