// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using OpenApi.Client.SourceGenerators.Client;
using OpenApi.Client.SourceGenerators.Diagnostics;

namespace OpenApi.Client.SourceGenerators;

/// <summary>
/// Generator for Open API Clients.
/// </summary>
[Generator]
public sealed class OpenApiClientGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        const string searchedAttribute = $"OpenApi.Client.{OpenApiClientGeneration.MarkerAttributeName}";

        context.RegisterPostInitializationOutput(i =>
        {
            i.AddSource(
                $"{OpenApiClientGeneration.MarkerAttributeName}.g.cs",
                OpenApiClientGeneration.MarkerAttributeSource
            );
        });

        IncrementalValueProvider<ImmutableArray<(string, string)>> additionalFiles = context
            .AdditionalTextsProvider.Where(text =>
                text.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            )
            .Select(
                (additionalText, cancellationToken) =>
                    (
                        Path.GetFileNameWithoutExtension(additionalText.Path).ToLower(),
                        additionalText.GetText(cancellationToken)!.ToString() // stream
                    )
            )
            .Where(text => text.Item1 is not null && text.Item2 is not null)!
            .Collect();

        IncrementalValuesProvider<GeneratorData?> classInfos = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                searchedAttribute,
                predicate: static (s, _) => true,
                transform: ComputeClassForGeneration
            )
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classInfos.Combine(additionalFiles), Execute);
    }

    private static GeneratorData? ComputeClassForGeneration(
        GeneratorAttributeSyntaxContext syntaxContext,
        CancellationToken cancellationToken
    )
    {
        if (
            syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.TargetNode)
            is not INamedTypeSymbol namedSymbol
        )
        {
            return null;
        }

        string specification = string.Empty;
        Location? location = null;
        SerializationTool serializationTool = SerializationTool.SystemTextJson;
        ImmutableArray<AttributeData> attributes = namedSymbol.GetAttributes();

        foreach (AttributeData attribute in attributes)
        {
            if (attribute.AttributeClass?.Name != OpenApiClientGeneration.MarkerAttributeName)
            {
                continue;
            }

            location = attribute.ApplicationSyntaxReference?.SyntaxTree.GetLocation(
                attribute.ApplicationSyntaxReference.Span
            );

            if (attribute.ConstructorArguments.Length > 0)
            {
                TypedConstant specificationArgument = attribute.ConstructorArguments[0];
                specification = (string?)specificationArgument.Value ?? string.Empty;
            }

            if (attribute.ConstructorArguments.Length > 1)
            {
                TypedConstant useDependencyInjectionArgument = attribute.ConstructorArguments[1];

                if (((int?)useDependencyInjectionArgument.Value ?? 0) == 1)
                {
                    serializationTool = SerializationTool.NewtonsoftJson;
                }
            }
        }

        return new GeneratorData
        {
            NamespaceName = namedSymbol.ContainingNamespace.ToString(),
            ClassName = namedSymbol.Name,
            SelectedFile = specification.ToLower(),
            SerializationTool = serializationTool,
            Access = namedSymbol.DeclaredAccessibility,
            Location = location,
            Templates = null,
        };
    }

    private static void Execute(
        SourceProductionContext spc,
        (
            GeneratorData? GeneratorData,
            ImmutableArray<(string Name, string Contents)> Files
        ) compilationAndFiles
    )
    {
        if (compilationAndFiles.GeneratorData is null)
        {
            return;
        }

        string? additionalFileContents = null;

        foreach ((string Name, string Contents) file in compilationAndFiles.Files)
        {
            if (
                compilationAndFiles.GeneratorData.SelectedFile.Equals(
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
                    compilationAndFiles.GeneratorData.Location ?? Location.None,
                    compilationAndFiles.GeneratorData.SelectedFile
                )
            );

            return;
        }

        if (additionalFileContents.Length == 0)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.DocumentEmpty,
                    compilationAndFiles.GeneratorData.Location ?? Location.None,
                    compilationAndFiles.GeneratorData.SelectedFile
                )
            );

            return;
        }

        ReadResult readResult = OpenApiDocument.Load(
            new MemoryStream(Encoding.UTF8.GetBytes(compilationAndFiles.GeneratorData.SelectedFile)),
            format: null,
            settings: null
        );

        if (readResult.Diagnostic?.Errors.Count > 0)
        {
            foreach (OpenApiError? error in readResult.Diagnostic.Errors)
            {
                spc.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.DocumentDeserializationFailed,
                        compilationAndFiles.GeneratorData.Location ?? Location.None,
                        compilationAndFiles.GeneratorData.SelectedFile,
                        error.Message
                    )
                );
            }

            return;
        }

        if (readResult.Document is null)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.GenerationFailed,
                    compilationAndFiles.GeneratorData.Location ?? Location.None,
                    compilationAndFiles.GeneratorData.SelectedFile,
                    "Serializer returned empty contract."
                )
            );

            return;
        }

        ClientGenerator generator = new(readResult.Document, compilationAndFiles.GeneratorData);
        GenerationResult generatorResult;

        try
        {
            generatorResult = generator.Generate();

            if (generatorResult.HasErrors)
            {
                foreach (GenerationError? error in generatorResult.Errors)
                {
                    spc.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.GenerationFailed,
                            compilationAndFiles.GeneratorData.Location ?? Location.None,
                            compilationAndFiles.GeneratorData.SelectedFile,
                            error.Message
                        )
                    );
                }

                return;
            }
        }
        catch (Exception e)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.GenerationFailed,
                    compilationAndFiles.GeneratorData.Location ?? Location.None,
                    compilationAndFiles.GeneratorData.SelectedFile,
                    e.Message
                )
            );

            return;
        }

        if (generatorResult.GeneratedClient is null)
        {
            spc.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.GeneratedSourceEmpty,
                    compilationAndFiles.GeneratorData.Location ?? Location.None,
                    compilationAndFiles.GeneratorData.SelectedFile
                )
            );

            return;
        }

        spc.AddSource(
            $"{compilationAndFiles.GeneratorData.ClassName}.g.cs",
            SourceText.From(generatorResult.GeneratedClient, Encoding.UTF8)
        );
    }
}
