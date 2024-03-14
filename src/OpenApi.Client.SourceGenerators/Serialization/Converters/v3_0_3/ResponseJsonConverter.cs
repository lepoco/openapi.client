// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Schema.v3_0_3;

namespace OpenApi.Client.SourceGenerators.Serialization.Converters.v3_0_3;

public sealed class ResponseJsonConverter : JsonConverter<IResponse>
{
    public override IResponse? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        using JsonDocument jsonDoc = JsonDocument.ParseValue(ref reader);
        JsonElement jsonObject = jsonDoc.RootElement;

        if (jsonObject.TryGetProperty("$ref", out _))
        {
            return JsonSerializer.Deserialize<ResponseReference>(jsonObject.GetRawText(), options);
        }

        return JsonSerializer.Deserialize<Response>(jsonObject.GetRawText(), options);
    }

    public override void Write(
        Utf8JsonWriter writer,
        IResponse value,
        JsonSerializerOptions options
    )
    {
        if (value is ResponseReference)
        {
            JsonSerializer.Serialize(writer, value as ResponseReference, options);
        }
        else if (value is Response)
        {
            JsonSerializer.Serialize(writer, value as Response, options);
        }
    }
}
