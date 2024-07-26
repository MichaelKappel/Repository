using LegacyCommon.Models.General;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using LegacyCommon.LM.General;
using LegacyCommon.LM.Claims;

namespace MichaelKappel.Repositories.Common.JsonConverters
{


    public class ClaimPagingResultsConverter : JsonConverter<PagingResultsModel<ClaimLM>>
    {
        public override PagingResultsModel<ClaimLM> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject token");

            PagingResultsModel<ClaimLM> pagingResult = new PagingResultsModel<ClaimLM>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return pagingResult;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "NextPageIndex":
                            pagingResult.NextPageIndex = reader.TokenType != JsonTokenType.Null ? reader.GetInt32() : null;
                            break;
                        case "PageCount":
                            pagingResult.PageCount = reader.GetInt32();
                            break;
                        case "PageRecordCount":
                            pagingResult.PageRecordCount = reader.GetInt32();
                            break;
                        case "PageSize":
                            pagingResult.PageSize = reader.GetInt32();
                            break;
                        case "PreviousPageIndex":
                            pagingResult.PreviousPageIndex = reader.TokenType != JsonTokenType.Null ? reader.GetInt32() : null;
                            break;
                        case "TotalRecordCount":
                            pagingResult.TotalRecordCount = reader.GetInt32();
                            break;
                        case "Results":
                            if (reader.TokenType != JsonTokenType.StartArray)
                                throw new JsonException("Expected StartArray token for Results");
                            var results = JsonSerializer.Deserialize<List<ClaimLM>>(ref reader, options);
                            pagingResult.Results = results ?? new List<ClaimLM>();
                            break;
                        default:
                            throw new JsonException($"Unknown property: {propertyName}");
                    }
                }
            }

            throw new JsonException("Expected EndObject token");
        }

        public override void Write(Utf8JsonWriter writer, PagingResultsModel<ClaimLM> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("NextPageIndex", value.NextPageIndex.GetValueOrDefault());
            writer.WriteNumber("PageCount", value.PageCount);
            writer.WriteNumber("PageRecordCount", value.PageRecordCount);
            writer.WriteNumber("PageSize", value.PageSize);
            writer.WriteNumber("PreviousPageIndex", value.PreviousPageIndex.GetValueOrDefault());
            writer.WriteNumber("TotalRecordCount", value.TotalRecordCount);

            writer.WritePropertyName("Results");
            writer.WriteStartArray();
            foreach (var result in value.Results)
            {
                JsonSerializer.Serialize(writer, result, options);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }

    public class AdaCodeNumberStatisticPagingResultsConverter : JsonConverter<PagingResultsModel<AdaCodeNumberStatisticLM>>
    {
        public override PagingResultsModel<AdaCodeNumberStatisticLM> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject token");

            PagingResultsModel<AdaCodeNumberStatisticLM> pagingResult = new PagingResultsModel<AdaCodeNumberStatisticLM>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return pagingResult;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "NextPageIndex":
                            pagingResult.NextPageIndex = reader.TokenType != JsonTokenType.Null ? reader.GetInt32() : null;
                            break;
                        case "PageCount":
                            pagingResult.PageCount = reader.GetInt32();
                            break;
                        case "PageRecordCount":
                            pagingResult.PageRecordCount = reader.GetInt32();
                            break;
                        case "PageSize":
                            pagingResult.PageSize = reader.GetInt32();
                            break;
                        case "PreviousPageIndex":
                            pagingResult.PreviousPageIndex = reader.TokenType != JsonTokenType.Null ? reader.GetInt32() : null;
                            break;
                        case "TotalRecordCount":
                            pagingResult.TotalRecordCount = reader.GetInt32();
                            break;
                        case "Results":
                            if (reader.TokenType != JsonTokenType.StartArray)
                                throw new JsonException("Expected StartArray token for Results");
                            var results = JsonSerializer.Deserialize<List<AdaCodeNumberStatisticLM>>(ref reader, options);
                            pagingResult.Results = results ?? new List<AdaCodeNumberStatisticLM>();
                            break;
                        default:
                            throw new JsonException($"Unknown property: {propertyName}");
                    }
                }
            }

            throw new JsonException("Expected EndObject token");
        }

        public override void Write(Utf8JsonWriter writer, PagingResultsModel<AdaCodeNumberStatisticLM> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("NextPageIndex", value.NextPageIndex.GetValueOrDefault());
            writer.WriteNumber("PageCount", value.PageCount);
            writer.WriteNumber("PageRecordCount", value.PageRecordCount);
            writer.WriteNumber("PageSize", value.PageSize);
            writer.WriteNumber("PreviousPageIndex", value.PreviousPageIndex.GetValueOrDefault());
            writer.WriteNumber("TotalRecordCount", value.TotalRecordCount);

            writer.WritePropertyName("Results");
            writer.WriteStartArray();
            foreach (var result in value.Results)
            {
                JsonSerializer.Serialize(writer, result, options);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
