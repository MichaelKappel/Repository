using System.Text.Json;
using System;
namespace MichaelKappel.Repositories.Common.JsonConverters
{
    public class LenientConverterString : System.Text.Json.Serialization.JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    return reader.GetUInt32().ToString();
                case JsonTokenType.True:
                    return "true";
                case JsonTokenType.False:
                    return "false";
                case JsonTokenType.Null:
                    return "null";
                default:
                    // Attempt to get value as String
                    try
                    {
                        return reader.GetString() ?? "null";
                    }
                    catch
                    {
                        // In case GetString() fails, return a default String
                        return "unhandled";
                    }
            }
        }
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}