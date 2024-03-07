// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using OpenApi.Client.SourceGenerators.Contracts;
using OpenApi.Client.SourceGenerators.Serialization;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;

namespace OpenApi.Client.SourceGenerators;

/// <summary>
/// Generator for Open API Clients.
/// </summary>
[Generator]
public partial class OpenApiClientGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<(string, string)> additionalFiles = context
            .AdditionalTextsProvider.Where(a =>
                a.Path.EndsWith("json") || a.Path.EndsWith("yml") || a.Path.EndsWith("yaml")
            )
            .Select((a, c) => (Path.GetFileNameWithoutExtension(a.Path), a.GetText(c)!.ToString()));

        context.RegisterPostInitializationOutput(i =>
        {
            i.AddSource("OpenApiClientAttribute.g.cs", OpenApiClientGenerationHelper.Attribute);
        });

        IncrementalValuesProvider<OpenApiContract?> classesToGenerate = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                "OpenApi.Client.OpenApiClientAttribute",
                predicate: static (s, _) => true,
                transform: static (syntaxContext, cancellationToken) =>
                    ComputeOpenApiContract(
                        syntaxContext.SemanticModel,
                        syntaxContext.TargetNode,
                        cancellationToken
                    )
            )
            .Where(static m => m is not null);

        IncrementalValuesProvider<(
            OpenApiContract? Left,
            ImmutableArray<(string, string)> Right
        )> compilationAndFiles = classesToGenerate.Combine(additionalFiles.Collect());

        context.RegisterSourceOutput(compilationAndFiles, Generate);
    }

    internal static OpenApiContract? ComputeOpenApiContract(
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
        SerializationTool serializationTool = SerializationTool.SystemTextJson;
        ImmutableArray<AttributeData> attributes = namedSymbol.GetAttributes();

        foreach (AttributeData attribute in attributes)
        {
            if (attribute.AttributeClass?.Name == "OpenApiClientAttribute")
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
                        serializationTool = SerializationTool.NewtonsoftJson;
                    }
                }
            }
        }

        return new OpenApiContract
        {
            Namespace = namedSymbol.ContainingNamespace.ToString(),
            ClassName = namedSymbol.Name,
            OpenApiSpecification = specification.ToLower(),
            SerializationTool = serializationTool
        };
    }
}
