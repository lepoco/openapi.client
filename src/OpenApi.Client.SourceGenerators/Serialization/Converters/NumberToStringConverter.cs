// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Serialization.Converters;

public sealed class NumberToStringConverter : JsonConverter<string>
{
    public override string? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal().ToString();
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
