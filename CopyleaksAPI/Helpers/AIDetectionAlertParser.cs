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
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Responses.AIDetector;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>
    /// Decodes the additionalData of a "suspected-ai-text" scan alert into an AIDetectionResult
    /// </summary>
    internal static class AIDetectionAlertParser
    {
        /// <summary>
        /// A private serializer, so the host's JsonConvert.DefaultSettings never apply.
        /// Unknown fields are ignored, and DefaultContractResolver keeps Newtonsoft's case-insensitive
        /// property matching, which the sandbox data needs ("Human", "Ai", "Starts", "Lengths").
        /// Trailing content after the JSON document is rejected, and strings are never parsed as dates.
        /// </summary>
        private static readonly JsonSerializer Serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new DefaultContractResolver(),
            CheckAdditionalContent = true,
            DateParseHandling = DateParseHandling.None
        });

        /// <summary>
        /// Decode the alert data of a "suspected-ai-text" alert
        /// </summary>
        /// <param name="code">The alert code</param>
        /// <param name="additionalData">The alert additionalData, a JSON document encoded as a string</param>
        /// <returns>
        /// The AI detection result, or null when the code is not "suspected-ai-text"
        /// or the data is null, empty or only NUL characters and whitespace
        /// </returns>
        /// <exception cref="JsonException">The data is not a valid AI detection result JSON document</exception>
        public static AIDetectionResult Parse(string code, string additionalData)
        {
            if (!string.Equals(code, CopyleaksAlertCodes.SUSPECTED_AI_TEXT, StringComparison.Ordinal))
                return null;
            if (additionalData == null)
                return null;

            // The server can pad the data with a tail of NUL characters; drop it and any trailing whitespace.
            int length = additionalData.Length;
            while (length > 0 && (additionalData[length - 1] == '\0' || char.IsWhiteSpace(additionalData[length - 1])))
                length--;
            if (length == 0)
                return null;

            using (var reader = new JsonTextReader(new StringReader(additionalData.Substring(0, length))))
            {
                return Serializer.Deserialize<AIDetectionResult>(reader);
            }
        }
    }
}
