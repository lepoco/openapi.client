// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis.Text;
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

        IncrementalValuesProvider<SourceGeneratorMetadata?> classInfos = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                searchedAttribute,
                predicate: static (s, _) => true,
                transform: ComputeClassForGeneration
            )
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classInfos.Combine(additionalFiles), Execute);
    }

    private static SourceGeneratorMetadata? ComputeClassForGeneration(
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
        bool nullable = true;
        bool useRecords = true;
        string? templates = null;
        string[] operations = [];

        Location? location = null;
        SerializationTool serializationTool = SerializationTool.SystemTextJson;
        ImmutableArray<AttributeData> attributes = namedSymbol.GetAttributes();

        foreach (AttributeData attribute in attributes)
        {
            if (attribute.AttributeClass?.Name != OpenApiClientGeneration.MarkerAttributeName)
            {
                continue;
            }

            foreach (KeyValuePair<string, TypedConstant> namedArgument in attribute.NamedArguments)
            {
                if (namedArgument.Key == "Templates")
                {
                    templates = namedArgument.Value.Value as string;
                }

                if (namedArgument.Key == "Nullable")
                {
                    nullable = namedArgument.Value.Value is not bool boolValue || boolValue;
                }

                if (namedArgument.Key == "UseRecords")
                {
                    nullable = namedArgument.Value.Value is not bool boolValue || boolValue;
                }

                if (namedArgument.Key == "SerializationTool")
                {
                    serializationTool = (namedArgument.Value.Value is int intValue ? intValue : 0) switch
                    {
                        1 => SerializationTool.NewtonsoftJson,
                        _ => SerializationTool.SystemTextJson,
                    };
                }
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

                operations = (
                    useDependencyInjectionArgument.Kind == TypedConstantKind.Array
                        ? useDependencyInjectionArgument
                            .Values.Select(v => v.Value as string)
                            .Where(s => s != null)
                            .ToArray()
                        : []
                )!;
            }
        }

        return new SourceGeneratorMetadata
        {
            NamespaceName = namedSymbol.ContainingNamespace.ToString(),
            ClassName = namedSymbol.Name,
            SelectedFile = specification.ToLower(),
            SerializationTool = serializationTool,
            Access = namedSymbol.DeclaredAccessibility,
            Location = location,
            Templates = templates,
            Nullable = nullable,
            UseRecords = useRecords,
            Operations = operations,
        };
    }

    private static void Execute(
        SourceProductionContext spc,
        (
            SourceGeneratorMetadata? GeneratorData,
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
                || compilationAndFiles.GeneratorData.SelectedFile.Equals(
                    $"{file.Name}.json",
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

        ClientGenerator generator = new(
            new GeneratorData
            {
                Contents = additionalFileContents,
                Access = compilationAndFiles.GeneratorData.Access,
                ClassName = compilationAndFiles.GeneratorData.ClassName,
                NamespaceName = compilationAndFiles.GeneratorData.NamespaceName,
                SerializationTool = compilationAndFiles.GeneratorData.SerializationTool,
                Templates = compilationAndFiles.GeneratorData.Templates,
                Operations = compilationAndFiles.GeneratorData.Operations,
                Nullable = compilationAndFiles.GeneratorData.Nullable,
            }
        );

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

    private sealed record SourceGeneratorMetadata
    {
        public required string NamespaceName { get; init; }

        public required string ClassName { get; init; }

        public required string SelectedFile { get; init; }

        public required Accessibility Access { get; init; }

        public required SerializationTool SerializationTool { get; init; }

        public required Location? Location { get; init; }

        public required string? Templates { get; init; }

        public string[] Operations { get; init; } = [];

        public bool UseRecords { get; init; } = true;

        public bool Nullable { get; init; } = true;
    }
}
