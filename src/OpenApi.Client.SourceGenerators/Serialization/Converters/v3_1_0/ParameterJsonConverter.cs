// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Schema.v3_1_0;

namespace OpenApi.Client.SourceGenerators.Serialization.Converters.v3_1_0;

public sealed class ParameterJsonConverter : JsonConverter<IParameter>
{
    public override IParameter? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        using JsonDocument jsonDoc = JsonDocument.ParseValue(ref reader);
        JsonElement jsonObject = jsonDoc.RootElement;

        if (jsonObject.TryGetProperty("$ref", out _))
        {
            return JsonSerializer.Deserialize<ParameterReference>(jsonObject.GetRawText(), options);
        }

        return JsonSerializer.Deserialize<Parameter>(jsonObject.GetRawText(), options);
    }

    public override void Write(
        Utf8JsonWriter writer,
        IParameter value,
        JsonSerializerOptions options
    )
    {
        if (value is ParameterReference)
        {
            JsonSerializer.Serialize(writer, value as ParameterReference, options);
        }
        else if (value is Parameter)
        {
            JsonSerializer.Serialize(writer, value as Parameter, options);
        }
    }
}
