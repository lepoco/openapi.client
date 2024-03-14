// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Schema;

namespace OpenApi.Client.SourceGenerators.Serialization;

public sealed class OpenApiSerializer()
{
    private static readonly JsonSerializerOptions jsonSettings = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(),
            new Converters.v2_0_0.ParameterJsonConverter(),
            new Converters.v2_0_0.ResponseJsonConverter(),
            new Converters.v3_0_3.ParameterJsonConverter(),
            new Converters.v3_0_3.RequestBodyJsonConverter(),
            new Converters.v3_0_3.ResponseJsonConverter(),
            new Converters.v3_0_3.SchemaTypeJsonConverter(),
            new Converters.v3_1_0.ParameterJsonConverter(),
            new Converters.v3_1_0.RequestBodyJsonConverter(),
            new Converters.v3_1_0.ResponseJsonConverter()
        }
    };

    public SerializationResult<IApiDocument> Deserialize(string? documentName, string? input)
    {
        documentName ??= "UNKNOWN_FILE_NAME";

        if (string.IsNullOrEmpty(input))
        {
            return new SerializationResultError(
                "OAPIC001",
                "OpenApi.Client.SourceGenerators.DocumentEmpty",
                $"Document {documentName} is empty."
            );
        }

        ApiDocumentVersion? version = null;

        try
        {
            version = GetProvidedDocumentVersion(input!);
        }
        catch (Exception e)
        {
            return new SerializationResultError(
                "OAPIC001",
                "OpenApi.Client.SourceGenerators.VersionUnknown",
                $"Extracting version from {documentName} failed with error: {e.Message}"
            );
        }

        if (version is null)
        {
            return new SerializationResultError(
                "OAPIC002",
                "OpenApi.Client.SourceGenerators.VersionUnknown",
                $"Unable to determine the Open API version of the document: {documentName}"
            );
        }

        IApiDocument? serializationResult;

        try
        {
            serializationResult = SerializeApiDocument(input!, version.Value);
        }
        catch (Exception e)
        {
            return new SerializationResultError(
                "OAPIC001",
                "OpenApi.Client.SourceGenerators.DocumentSerializationFailed",
                $"Serialization of document \"{documentName}\" failed with error: {e.Message}"
            );
        }

        return new SerializationResult<IApiDocument>(serializationResult);
    }

    private ApiDocumentVersion? GetProvidedDocumentVersion(string input)
    {
        OpenApiMarkerObject? markerObject = null;

        markerObject = JsonSerializer.Deserialize<OpenApiMarkerObject>(input, jsonSettings);

        if (markerObject is null)
        {
            return null;
        }

        string version =
            markerObject.Value.openapi?.Trim().ToLower()
            ?? markerObject.Value.swagger?.Trim().ToLower()
            ?? markerObject.Value.swaggerVersion?.Trim().ToLower()
            ?? string.Empty;

        return version switch
        {
            "1" => ApiDocumentVersion.v1_2_0,
            "1.2.0" => ApiDocumentVersion.v1_2_0,
            "1.2" => ApiDocumentVersion.v1_2_0,
            "2.0.0" => ApiDocumentVersion.v2_0_0,
            "2.0" => ApiDocumentVersion.v2_0_0,
            "2" => ApiDocumentVersion.v2_0_0,
            "3" => ApiDocumentVersion.v3_0_0,
            "3.0" => ApiDocumentVersion.v3_0_0,
            "3.0.0" => ApiDocumentVersion.v3_0_0,
            "3.0.1" => ApiDocumentVersion.v3_0_1,
            "3.0.2" => ApiDocumentVersion.v3_0_2,
            "3.0.3" => ApiDocumentVersion.v3_0_3,
            "3.1.0" => ApiDocumentVersion.v3_1_0,
            "3.1" => ApiDocumentVersion.v3_1_0,
            _ => null
        };
    }

    private IApiDocument? SerializeApiDocument(string input, ApiDocumentVersion version)
    {
        switch (version)
        {
            case ApiDocumentVersion.v1_2_0:
                return JsonSerializer.Deserialize<Schema.v1_2_0.ApiDocument>(input, jsonSettings);

            case ApiDocumentVersion.v2_0_0:
                return JsonSerializer.Deserialize<Schema.v2_0_0.ApiDocument>(input, jsonSettings);

            case ApiDocumentVersion.v3_0_0:
            case ApiDocumentVersion.v3_0_1:
            case ApiDocumentVersion.v3_0_2:
            case ApiDocumentVersion.v3_0_3:
                return JsonSerializer.Deserialize<Schema.v3_0_3.ApiDocument>(input, jsonSettings);

            case ApiDocumentVersion.v3_1_0:
                return JsonSerializer.Deserialize<Schema.v3_1_0.ApiDocument>(input, jsonSettings);

            default:
                return null;
        }
    }

    private readonly record struct OpenApiMarkerObject(
        string? openapi,
        string? swagger,
        string? swaggerVersion
    );
}
