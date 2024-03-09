// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using OpenApi.Client.SourceGenerators.Models;
using OpenApi.Client.SourceGenerators.Schema;
using OpenApi.Client.SourceGenerators.Serialization;
using System;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;

namespace OpenApi.Client.SourceGenerators;

public partial class OpenApiClientGenerator
{
    private static readonly JsonSerializerOptions jsonSettings = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    internal static void Execute(
        SourceProductionContext spc,
        (
            RequestedClassToGenerate? Contract,
            ImmutableArray<(string, string)> Files
        ) compilationAndFiles
    )
    {
        ApiDocument? openApiDocument = default;
        string documentFilename = "unknown";

        if (compilationAndFiles.Contract is { } value)
        {
            foreach ((string, string) file in compilationAndFiles.Files)
            {
                if (file.Item1.Equals(compilationAndFiles.Contract.SelectedFile))
                {
                    documentFilename = file.Item1;

                    try
                    {
                        openApiDocument = JsonSerializer.Deserialize<ApiDocument>(
                            file.Item2,
                            jsonSettings
                        );
                    }
                    catch (Exception e)
                    {
                        spc.ReportDiagnostic(
                            Diagnostic.Create(
                                new DiagnosticDescriptor(
                                    id: "OAPIC001",
                                    title: "Error",
                                    messageFormat: e.Message,
                                    category: "OpenApi.Client.SourceGenerators.DocumentSerializationFailed",
                                    defaultSeverity: DiagnosticSeverity.Error,
                                    isEnabledByDefault: true
                                ),
                                Location.None
                            )
                        );
                        return;
                    }
                }
            }

            if (openApiDocument is null)
            {
                spc.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "OAPIC001",
                            title: "Error",
                            messageFormat: $"Document {documentFilename} is empty.",
                            category: "OpenApi.Client.SourceGenerators.DocumentEmpty",
                            defaultSeverity: DiagnosticSeverity.Error,
                            isEnabledByDefault: true
                        ),
                        Location.None
                    )
                );
                return;
            }

            ApiDocumentToStringConverter converter =
                new(
                    openApiDocument,
                    compilationAndFiles.Contract.NamespaceName,
                    compilationAndFiles.Contract.ClassName
                );

            spc.AddSource(
                $"{value.ClassName}.g.cs",
                SourceText.From(converter.Convert(), Encoding.UTF8)
            );
        }
    }
}
