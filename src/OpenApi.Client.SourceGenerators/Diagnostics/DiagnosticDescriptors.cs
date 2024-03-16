// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;

namespace OpenApi.Client.SourceGenerators.Diagnostics;

/// <summary>
/// A container for all <see cref="DiagnosticDescriptor"/> instances for errors reported by analyzers in this project.
/// </summary>
internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor DocumentMissing = new DiagnosticDescriptor(
        id: "OAPIC001",
        title: "Missing OpenAPI document",
        messageFormat: "Open API document with name \"{0}\" is missing",
        category: "OpenApi.Client.SourceGenerators.DocumentMissing",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Each OpenAPI document must be added to .csproj with a relative path to the project. The OpenApiClient attribute should have only the file name, without the path and extension.",
        helpLinkUri: "https://github.com/lepoco/openapi.client/blob/main/documentation/OAPIC001.md"
    );

    public static readonly DiagnosticDescriptor DocumentEmpty = new DiagnosticDescriptor(
        id: "OAPIC002",
        title: "Empty OpenAPI document",
        messageFormat: "Open API document with name \"{0}\" is empty",
        category: "OpenApi.Client.SourceGenerators.DocumentEmpty",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The OpenAPI document was found but it is empty.",
        helpLinkUri: "https://github.com/lepoco/openapi.client/blob/main/documentation/OAPIC002.md"
    );

    public static readonly DiagnosticDescriptor DocumentDeserializationFailed =
        new DiagnosticDescriptor(
            id: "OAPIC003",
            title: "Deserializing OpenAPI document failed",
            messageFormat: "Deserialization of document \"{0}\" failed with message: \"{1}\"",
            category: "OpenApi.Client.SourceGenerators.DocumentDeserializationFailed",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The document was read successfully, but it failed to deserialize into an object.",
            helpLinkUri: "https://github.com/lepoco/openapi.client/blob/main/documentation/OAPIC003.md"
        );

    public static readonly DiagnosticDescriptor GenerationFailed = new DiagnosticDescriptor(
        id: "OAPIC004",
        title: "Generating OpenAPI Client failed",
        messageFormat: "Generation of source code from document \"{0}\" failed with message: \"{1}\"",
        category: "OpenApi.Client.SourceGenerators.GenerationFailed",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The document was successfully read and deserialized into an OpenApi object, but source code generation failed.",
        helpLinkUri: "https://github.com/lepoco/openapi.client/blob/main/documentation/OAPIC004.md"
    );

    public static readonly DiagnosticDescriptor GeneratedSourceEmpty = new DiagnosticDescriptor(
        id: "OAPIC005",
        title: "Generating OpenAPI Client failed",
        messageFormat: "Generation of source code from document \"{0}\" failed",
        category: "OpenApi.Client.SourceGenerators.GeneratedSourceEmpty",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The document was successfully read and deserialized into an OpenApi object, but the generated source code is empty.",
        helpLinkUri: "https://github.com/lepoco/openapi.client/blob/main/documentation/OAPIC005.md"
    );

    public static readonly DiagnosticDescriptor MissingPaths = new DiagnosticDescriptor(
        id: "OAPIC006",
        title: "No paths in OpenAPI document",
        messageFormat: "No paths to generate in the file \"{0}\" were found",
        category: "OpenApi.Client.SourceGenerators.GeneratedSourceEmpty",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The document was successfully read and deserialized into an OpenApi object, but the contract does not have any paths.",
        helpLinkUri: "https://github.com/lepoco/openapi.client/blob/main/documentation/OAPIC006.md"
    );
}
