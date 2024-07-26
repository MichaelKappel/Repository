using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace MichaelKappel.Repositories.Common.JsonConverters
{
    public class LenientConverterDateOnly : JsonConverter<DateOnly>
    {
        private const string DefaultFormat = "yyyy-MM-dd";
        private static readonly string[] SupportedFormats = new[] {
            "yyyy-MM-dd",   // ISO 8601 format
            "MM/dd/yyyy",   // US format
            "dd/MM/yyyy",   // UK format
            "yyyyMMdd",     // Compact format
            "dd-MM-yyyy"    // Common international format
        };

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected a JSON string to convert to DateOnly");

            var dateString = reader.GetString();
            if (dateString == null)
                throw new JsonException("Date string is null");

            // Try parsing the date string using supported formats
            if (DateOnly.TryParseExact(dateString, SupportedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            throw new JsonException($"Failed to parse '{dateString}' to a DateOnly.");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            // Convert the DateOnly value to string using the default format
            writer.WriteStringValue(value.ToString(DefaultFormat, CultureInfo.InvariantCulture));
        }
    }
}
