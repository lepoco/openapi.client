// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

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

        context.RegisterPostInitializationOutput(i =>
        {
            i.AddSource($"{MarkerAttributeName}.g.cs", OpenApiClientGenerationHelper.Attribute);
        });

        IncrementalValueProvider<ImmutableArray<(string, string)>> additionalFiles = context
            .AdditionalTextsProvider.Where(text =>
                text.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            )
            .Select(
                (additionalText, cancellationToken) =>
                    (
                        Path.GetFileNameWithoutExtension(additionalText.Path).ToLower(),
                        additionalText.GetText(cancellationToken)!.ToString()
                    )
            )
            .Where(text => text.Item1 is not null && text.Item2 is not null)!
            .Collect();

        IncrementalValuesProvider<RequestedClassToGenerate?> classInfos = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                searchedAttribute,
                predicate: static (s, _) => true,
                transform: ComputeClassForGeneration
            )
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classInfos.Combine(additionalFiles), Execute);
    }

    private static RequestedClassToGenerate? ComputeClassForGeneration(
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
        RequestedSerializationTool serializationTool = RequestedSerializationTool.SystemTextJson;
        ImmutableArray<AttributeData> attributes = namedSymbol.GetAttributes();

        foreach (AttributeData attribute in attributes)
        {
            if (attribute.AttributeClass?.Name != MarkerAttributeName)
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
                    serializationTool = RequestedSerializationTool.NewtonsoftJson;
                }
            }
        }

        return new RequestedClassToGenerate
        {
            NamespaceName = namedSymbol.ContainingNamespace.ToString(),
            ClassName = namedSymbol.Name,
            SelectedFile = specification.ToLower(),
            SerializationTool = serializationTool,
            Access = namedSymbol.DeclaredAccessibility,
            Location = location
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

        public required Location? Location { get; init; }
    }
}
