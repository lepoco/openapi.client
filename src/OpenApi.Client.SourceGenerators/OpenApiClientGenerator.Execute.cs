// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using OpenApi.Client.SourceGenerators.Contracts;
using OpenApi.Client.SourceGenerators.Diagnostics;
using OpenApi.Client.SourceGenerators.Generation;

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
                    compilationAndFiles.Contract.Location ?? Location.None,
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
                    compilationAndFiles.Contract.Location ?? Location.None,
                    compilationAndFiles.Contract.SelectedFile
                )
            );

            return;
        }

        OpenApiDocument? serializationResult = new OpenApiStreamReader().Read(
            new MemoryStream(Encoding.UTF8.GetBytes(compilationAndFiles.Contract.SelectedFile)),
            out OpenApiDiagnostic? diagnostic
        );

        if (diagnostic?.Errors.Count > 0)
        {
            foreach (OpenApiError? error in diagnostic.Errors)
            {
                spc.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.DocumentDeserializationFailed,
                        compilationAndFiles.Contract.Location ?? Location.None,
                        compilationAndFiles.Contract.SelectedFile,
                        error.Message
                    )
                );
            }

            return;
        }

        if (serializationResult is null)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.GenerationFailed,
                    compilationAndFiles.Contract.Location ?? Location.None,
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
            serializationResult
        );

        if (contract.Paths.Count == 0)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.MissingPaths,
                    compilationAndFiles.Contract.Location ?? Location.None,
                    compilationAndFiles.Contract.SelectedFile
                )
            );

            return;
        }

        try
        {
            ClientGenerator generator =
                new(
                    contract,
                    compilationAndFiles.Contract.SerializationTool switch
                    {
                        RequestedSerializationTool.NewtonsoftJson
                            => ClientGeneratorSerializer.NewtonsoftJson,
                        _ => ClientGeneratorSerializer.SystemTextJson
                    }
                );
            GenerationResult<string> generatorResult = generator.Generate();

            if (generatorResult.HasErrors)
            {
                foreach (GenerationResultError error in generatorResult.Errors)
                {
                    spc.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.GenerationFailed,
                            compilationAndFiles.Contract.Location ?? Location.None,
                            compilationAndFiles.Contract.SelectedFile,
                            error.Message
                        )
                    );
                }

                return;
            }

            generatedSource = generatorResult.Result;
        }
        catch (Exception e)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.GenerationFailed,
                    compilationAndFiles.Contract.Location ?? Location.None,
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
                    compilationAndFiles.Contract.Location ?? Location.None,
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
