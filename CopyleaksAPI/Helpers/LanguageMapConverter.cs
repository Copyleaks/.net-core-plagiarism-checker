using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>Reads the server's language object (name to code) into a case-insensitive dictionary.</summary>
    internal sealed class LanguageMapConverter : JsonConverter<Dictionary<string, string>>
    {
        public override Dictionary<string, string> ReadJson(JsonReader reader, Type objectType, Dictionary<string, string> existingValue,
                                                            bool hasExistingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            switch (token.Type)
            {
                case JTokenType.Null:
                case JTokenType.Undefined:
                    return null;
                case JTokenType.Object:
                    foreach (JProperty property in ((JObject)token).Properties())
                        result[property.Name] = property.Value.Type == JTokenType.Null ? null : property.Value.ToString();
                    return result;
                default:
                    throw new JsonSerializationException($"Unexpected {token.Type} for a language list.");
            }
        }

        public override void WriteJson(JsonWriter writer, Dictionary<string, string> value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteStartObject();
            foreach (var pair in value)
            {
                writer.WritePropertyName(pair.Key);
                writer.WriteValue(pair.Value);
            }
            writer.WriteEndObject();
        }
    }
}
