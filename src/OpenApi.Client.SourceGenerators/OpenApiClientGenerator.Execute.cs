// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using OpenApi.Client.SourceGenerators.Contracts;
using OpenApi.Client.SourceGenerators.Genertion;
using OpenApi.Client.SourceGenerators.Serialization;

namespace OpenApi.Client.SourceGenerators;

public partial class OpenApiClientGenerator
{
    private static void Execute(
        SourceProductionContext spc,
        (
            RequestedClassToGenerate? Contract,
            ImmutableArray<(string Name, string Contents)> Files
        ) compilationAndFiles
    )
    {
        if (compilationAndFiles.Contract is null)
        {
            return;
        }

        string? additionalFileContents = null;

        foreach ((string Name, string Contents) file in compilationAndFiles.Files)
        {
            if (
                compilationAndFiles.Contract.SelectedFile.Equals(
                    file.Name,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                additionalFileContents = file.Contents;
            }
        }

        if (additionalFileContents is null)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "OAPIC003",
                        title: "Error",
                        messageFormat: $"Document {compilationAndFiles.Contract.SelectedFile} is empty.",
                        category: "OpenApi.Client.SourceGenerators.DocumentEmpty",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true
                    ),
                    Location.None
                )
            );
        }

        SerializationResult<Schema.IApiDocument>? serializationResult =
            new OpenApiSerializer().Deserialize(
                compilationAndFiles.Contract.SelectedFile,
                additionalFileContents
            );

        if (serializationResult?.HasErrors ?? false)
        {
            foreach (SerializationResultError error in serializationResult.Errors)
            {
                spc.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: error.Id,
                            title: "Error",
                            messageFormat: error.Message,
                            category: error.Category,
                            defaultSeverity: DiagnosticSeverity.Error,
                            isEnabledByDefault: true
                        ),
                        Location.None
                    )
                );
            }
        }

        if (serializationResult?.Result is null)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        id: "OAPIC001",
                        title: "Error",
                        messageFormat: $"Document {compilationAndFiles.Contract.SelectedFile} serialization failed.",
                        category: "OpenApi.Client.SourceGenerators.DocumentSerializationFailed",
                        defaultSeverity: DiagnosticSeverity.Error,
                        isEnabledByDefault: true
                    ),
                    Location.None
                )
            );

            return;
        }

        OpenApiContract contract = OpenApiContractParser.Parse(
            compilationAndFiles.Contract.NamespaceName,
            compilationAndFiles.Contract.ClassName,
            compilationAndFiles.Contract.Access,
            serializationResult.Result
        );
        ClientGenerator generator = new(contract);

        spc.AddSource(
            $"{compilationAndFiles.Contract.ClassName}.g.cs",
            SourceText.From(generator.Generate(), Encoding.UTF8)
        );
    }
}
