using System.Text.Json;
using System;
namespace MichaelKappel.Repositories.Common.JsonConverters
{
    public class LenientConverterBooleanNullable : System.Text.Json.Serialization.JsonConverter<bool?>
    {
        public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    return reader.GetUInt32() == 1;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;
            }

            string value = reader.GetString()!;
            switch (value.ToLower())
            {
                case "true":
                case "yes":
                case "y":
                case "1":
                    return true;
                case "false":
                case "no":
                case "n":
                case "0":
                    return false;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteBooleanValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
