/********************************************************************************
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
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Requests.TextModeration
{
    public class CopyleaksTextModerationRequestModel
    {
        /// <summary>
        /// Text to produce Text Moderation report for.
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Use sandbox mode to test your integration with the Copyleaks API without consuming any credits.
        /// </summary>
        [JsonProperty("sandbox")]
        public bool Sandbox { get; set; } = false;

        /// <summary>
        /// The language code of your content. The selected language should be on the Supported Languages list above.
        /// If the 'language' field is not specified, our system will automatically detect the language of the content.
        /// </summary>
        [JsonProperty("language")]
        public string? Language { get; set; }

        /// <summary>
        /// A list of label configurations to be used for the moderation process.
        /// </summary>
        [JsonProperty("labels")]
        [MinLength(1, ErrorMessage = "Labels array must have at least 1 elemnt.")]
        [MaxLength(32, ErrorMessage = "Labels array must have at most 32 elemnts.")]
        public object[] Labels { get; set; }

        // Constructor
        public CopyleaksTextModerationRequestModel(string text, bool sandbox, string? language, object[] labels)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Sandbox = sandbox;
            Language = language;

            if (!labels.Any())
                throw new ArgumentException("Labels array must have at least 1 element.", nameof(labels));

            if (labels.Length > 32)
                throw new ArgumentException("Labels array must have at most 32 elements.", nameof(labels));

            Labels = labels;
        }
    }
}
