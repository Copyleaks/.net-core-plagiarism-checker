using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Responses.AIDetector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>Decodes the additionalData of a "suspected-ai-text" scan alert into an AIDetectionResult.</summary>
    internal static class AIDetectionAlertParser
    {
        /// <summary>
        /// Private, so the host's JsonConvert.DefaultSettings never apply. Ignores unknown fields, keeps case-insensitive
        /// matching (sandbox data uses "Human", "Ai", "Starts", "Lengths"), rejects trailing content, never parses dates.
        /// </summary>
        private static readonly JsonSerializer Serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new DefaultContractResolver(),
            CheckAdditionalContent = true,
            DateParseHandling = DateParseHandling.None
        });

        /// <summary>Decode the additionalData (a JSON document in a string) of an alert.</summary>
        /// <returns>
        /// Null when the code is not "suspected-ai-text", the data is null, empty or only NUL and ASCII whitespace,
        /// or the data is valid JSON that is not an object.
        /// </returns>
        /// <exception cref="JsonException">The data is not a valid AI detection result JSON document.</exception>
        public static AIDetectionResult Parse(string code, string additionalData)
        {
            if (!string.Equals(code, CopyleaksAlertCodes.SUSPECTED_AI_TEXT, StringComparison.Ordinal))
                return null;
            if (additionalData == null)
                return null;

            // The server can pad the data with a tail of NUL characters; drop it and any trailing ASCII whitespace.
            int length = additionalData.Length;
            while (length > 0 && IsTrailingPadding(additionalData[length - 1]))
                length--;
            if (length == 0)
                return null;

            using (var reader = new JsonTextReader(new StringReader(additionalData.Substring(0, length))))
            {
                // Move to the first token, skipping comments. Only comments means there is no data.
                do
                {
                    if (!reader.Read())
                        return null;
                } while (reader.TokenType == JsonToken.Comment);

                if (reader.TokenType == JsonToken.StartObject)
                    return Serializer.Deserialize<AIDetectionResult>(reader);

                // Valid JSON that is not an object (an array, number, string, true/false or null) is not an AI detection result.
                // Read the whole document first, so malformed data still throws.
                JToken.ReadFrom(reader);
                while (reader.Read())
                {
                    // The reader throws on any content after the document other than a comment.
                }
                return null;
            }
        }

        /// <summary>
        /// Trimmed from the end of the data: NUL and ASCII whitespace (tab, LF, VT, FF, CR, space).
        /// Unicode whitespace such as a no-break space is not trimmed.
        /// </summary>
        private static bool IsTrailingPadding(char c)
        {
            return c == '\0' || c == '\t' || c == '\n' || c == '\v' || c == '\f' || c == '\r' || c == ' ';
        }
    }
}
