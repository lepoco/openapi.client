// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using OpenApi.Client.SourceGenerators.Contracts;
using OpenApi.Client.SourceGenerators.Diagnostics;
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
                break;
            }
        }

        if (additionalFileContents is null)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.DocumentMissing,
                    Location.None,
                    compilationAndFiles.Contract.SelectedFile
                )
            );

            return;
        }

        if (additionalFileContents.Length == 0)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.DocumentEmpty,
                    Location.None,
                    compilationAndFiles.Contract.SelectedFile
                )
            );

            return;
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
                        DiagnosticDescriptors.DocumentDeserializationFailed,
                        Location.None,
                        compilationAndFiles.Contract.SelectedFile,
                        error.Message
                    )
                );
            }

            return;
        }

        if (serializationResult?.Result is null)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.GenerationFailed,
                    Location.None,
                    compilationAndFiles.Contract.SelectedFile,
                    "Serializer returned empty contract"
                )
            );

            return;
        }

        string? generatedSource = null;

        OpenApiContract contract = OpenApiContractParser.Parse(
            compilationAndFiles.Contract.NamespaceName,
            compilationAndFiles.Contract.ClassName,
            compilationAndFiles.Contract.Access,
            serializationResult.Result
        );

        if (contract.Paths.Count == 0)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.MissingPaths,
                    Location.None,
                    compilationAndFiles.Contract.SelectedFile
                )
            );

            return;
        }

        try
        {
            ClientGenerator generator = new(contract);
            GenerationResult<string> geneatorResult = generator.Generate();

            if (geneatorResult.HasErrors)
            {
                foreach (SerializationResultError error in serializationResult.Errors)
                {
                    spc.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.GenerationFailed,
                            Location.None,
                            compilationAndFiles.Contract.SelectedFile,
                            error.Message
                        )
                    );
                }

                return;
            }

            generatedSource = geneatorResult.Result;
        }
        catch (Exception e)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.GenerationFailed,
                    Location.None,
                    compilationAndFiles.Contract.SelectedFile,
                    e.Message
                )
            );

            return;
        }

        if (generatedSource is null)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.GeneratedSourceEmpty,
                    Location.None,
                    compilationAndFiles.Contract.SelectedFile
                )
            );

            return;
        }

        spc.AddSource(
            $"{compilationAndFiles.Contract.ClassName}.g.cs",
            SourceText.From(generatedSource, Encoding.UTF8)
        );
    }
}
