﻿using System.Text.Json;
using System;
namespace MichaelKappel.Repositories.Common.JsonConverters
{
    public class LenientConverterInt32Nullable : System.Text.Json.Serialization.JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string str = reader.GetString() ?? string.Empty;
                if (string.IsNullOrEmpty(str))
                {
                    return null;
                }
                else if (int.TryParse(str, out int value))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }

            return int.MaxValue;
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteNumberValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
