// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace OpenApi.Client.SourceGenerators;

/// <summary>
/// Generator for Open API Clients.
/// </summary>
[Generator]
public partial class OpenApiClientGenerator : IIncrementalGenerator
{
    private const string MarkerAttributeName = "OpenApiClientAttribute";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        const string searchedAttribute = $"OpenApi.Client.{MarkerAttributeName}";

        // TODO: Fix files
        IncrementalValuesProvider<(string, string)> additionalFiles = context
            .AdditionalTextsProvider.Where(a =>
                a.Path.EndsWith("json") || a.Path.EndsWith("yml") || a.Path.EndsWith("yaml")
            )
            .Select(
                (a, c) =>
                    (Path.GetFileNameWithoutExtension(a.Path).ToLower(), a.GetText(c)!.ToString())
            );

        context.RegisterPostInitializationOutput(i =>
        {
            i.AddSource($"{MarkerAttributeName}.g.cs", OpenApiClientGenerationHelper.Attribute);
        });

        IncrementalValuesProvider<RequestedClassToGenerate?> classesToGenerate = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                searchedAttribute,
                predicate: static (s, _) => true,
                transform: static (syntaxContext, cancellationToken) =>
                    ComputeClassForGeneration(
                        syntaxContext.SemanticModel,
                        syntaxContext.TargetNode,
                        cancellationToken
                    )
            )
            .Where(static m => m is not null);

        IncrementalValuesProvider<(
            RequestedClassToGenerate? Left,
            ImmutableArray<(string, string)> Right
        )> compilationAndFiles = classesToGenerate.Combine(additionalFiles.Collect());

        context.RegisterSourceOutput(compilationAndFiles, Execute);
    }

    private static RequestedClassToGenerate? ComputeClassForGeneration(
        SemanticModel semanticModel,
        SyntaxNode node,
        CancellationToken cancellationToken
    )
    {
        if (semanticModel.GetDeclaredSymbol(node) is not INamedTypeSymbol namedSymbol)
        {
            return null;
        }

        string specification = string.Empty;
        RequestedSerializationTool serializationTool = RequestedSerializationTool.SystemTextJson;
        ImmutableArray<AttributeData> attributes = namedSymbol.GetAttributes();

        foreach (AttributeData attribute in attributes)
        {
            if (attribute.AttributeClass?.Name == MarkerAttributeName)
            {
                if (attribute.ConstructorArguments.Length > 0)
                {
                    TypedConstant specificationArgument = attribute.ConstructorArguments[0];
                    specification = (string?)specificationArgument.Value ?? string.Empty;
                }

                if (attribute.ConstructorArguments.Length > 1)
                {
                    TypedConstant useDependencyInjectionArgument = attribute.ConstructorArguments[
                        1
                    ];

                    if (((int?)useDependencyInjectionArgument.Value ?? 0) == 1)
                    {
                        serializationTool = RequestedSerializationTool.NewtonsoftJson;
                    }
                }
            }
        }

        return new RequestedClassToGenerate
        {
            NamespaceName = namedSymbol.ContainingNamespace.ToString(),
            ClassName = namedSymbol.Name,
            SelectedFile = specification.ToLower(),
            SerializationTool = serializationTool,
            Access = namedSymbol.DeclaredAccessibility
        };
    }

    private enum RequestedSerializationTool
    {
        SystemTextJson,
        NewtonsoftJson
    }

    private sealed class RequestedClassToGenerate
    {
        public required string NamespaceName { get; init; }

        public required string ClassName { get; init; }

        public required string SelectedFile { get; init; }

        public required Accessibility Access { get; init; }

        public required RequestedSerializationTool SerializationTool { get; init; }
    }
}
