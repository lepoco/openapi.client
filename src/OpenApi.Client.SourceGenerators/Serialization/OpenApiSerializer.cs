// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using OpenApi.Client.SourceGenerators.Schema;

namespace OpenApi.Client.SourceGenerators.Serialization;

internal sealed class OpenApiSerializer(SourceProductionContext sourceProductionContext)
{
    private static readonly JsonSerializerOptions jsonSettings = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public IApiDocument? Serialize(string? documentName, string? input)
    {
        documentName ??= "UNKNOWN_FILE_NAME";

        if (string.IsNullOrEmpty(input))
        {
            sourceProductionContext.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "OAPIC001",
                        title: "Error",
                        messageFormat: $"Document {documentName} is empty.",
                        category: "OpenApi.Client.SourceGenerators.DocumentEmpty",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true
                    ),
                    Location.None
                )
            );

            return null;
        }

        ApiDocumentVersion? version = GetProvidedDocumentVersion(documentName, input!);

        if (version is null)
        {
            sourceProductionContext.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "OAPIC002",
                        title: "Error",
                        messageFormat: $"Unable to determine the Open API version of the document: {documentName}",
                        category: "OpenApi.Client.SourceGenerators.VersionUnknown",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true
                    ),
                    Location.None
                )
            );

            return null;
        }

        return SerializeApiDocument(input!, version.Value);
    }

    private ApiDocumentVersion? GetProvidedDocumentVersion(string documentName, string input)
    {
        OpenApiMarkerObject? markerObject = null;

        try
        {
            markerObject = JsonSerializer.Deserialize<OpenApiMarkerObject>(input, jsonSettings);
        }
        catch (Exception e)
        {
            sourceProductionContext.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "OAPIC002",
                        title: "Error",
                        messageFormat: $"Unable to determine the Open API version of the document: {documentName}, with exception: {e.Message}",
                        category: "OpenApi.Client.SourceGenerators.VersionUnknown",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true
                    ),
                    Location.None
                )
            );

            return null;
        }

        if (markerObject is null)
        {
            sourceProductionContext.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "OAPIC002",
                        title: "Error",
                        messageFormat: $"Unable to determine the Open API version of the document: {documentName}",
                        category: "OpenApi.Client.SourceGenerators.VersionUnknown",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true
                    ),
                    Location.None
                )
            );

            return null;
        }

        string version =
            markerObject.Value.openapi?.Trim().ToLower()
            ?? markerObject.Value.swagger?.Trim().ToLower()
            ?? markerObject.Value.swaggerVersion?.Trim().ToLower()
            ?? string.Empty;

        return version switch
        {
            "1.2.0" => ApiDocumentVersion.v1_2_0,
            "1.2" => ApiDocumentVersion.v1_2_0,
            "2.0.0" => ApiDocumentVersion.v2_0_0,
            "2.0" => ApiDocumentVersion.v2_0_0,
            "2" => ApiDocumentVersion.v2_0_0,
            "3.1.0" => ApiDocumentVersion.v3_1_0,
            "3.1" => ApiDocumentVersion.v3_1_0,
            _ => null
        };
    }

    private IApiDocument? SerializeApiDocument(string input, ApiDocumentVersion version)
    {
        try
        {
            switch (version)
            {
                case ApiDocumentVersion.v1_2_0:
                    return JsonSerializer.Deserialize<Schema.v1_2_0.ApiDocument>(
                        input,
                        jsonSettings
                    );

                case ApiDocumentVersion.v2_0_0:
                    return JsonSerializer.Deserialize<Schema.v2_0_0.ApiDocument>(
                        input,
                        jsonSettings
                    );

                case ApiDocumentVersion.v3_1_0:
                    return JsonSerializer.Deserialize<Schema.v3_1_0.ApiDocument>(
                        input,
                        jsonSettings
                    );

                default:
                    return null;
            }
        }
        catch (Exception e)
        {
            sourceProductionContext.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "OAPIC001",
                        title: "Error",
                        messageFormat: $"Open API document deserialization failed with message: {e.Message}",
                        category: "OpenApi.Client.SourceGenerators.DocumentSerializationFailed",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true
                    ),
                    Location.None
                )
            );

            return null;
        }
    }

    private readonly record struct OpenApiMarkerObject(
        string? openapi,
        string? swagger,
        string? swaggerVersion
    );
}
