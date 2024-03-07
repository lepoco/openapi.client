// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using OpenApi.Client.SourceGenerators.Clients;
using System;
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
    private static IncrementalValueProvider<ImmutableArray<AdditionalText>> texts;
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<ImmutableArray<string>> translations = context.AdditionalTextsProvider
                                .Where(text => text.Path.EndsWith("json",
                                               StringComparison.OrdinalIgnoreCase))
                                .Select((text, token) => text.GetText(token)?.ToString())
                                .Where(text => text is not null)!
                                .Collect<string>();

        context.RegisterPostInitializationOutput(i =>
        {
            i.AddSource("OpenApiClientAttribute.g.cs", OpenApiClientGenerationHelper.Attribute);
        });

        IncrementalValuesProvider<OpenApiClient?> classesToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "OpenApi.Client.OpenApiClientAttribute",
                predicate: static (s, _) => true,
                transform: static (syntaxContext, cancellationToken) => ComputeOpenApiContract(syntaxContext.SemanticModel, syntaxContext.TargetNode, cancellationToken))
            .Where(static m => m is not null);


        // Generate source code for each found
        context.RegisterSourceOutput(classesToGenerate,
            static (spc, source) => Execute(source, spc));
    }

    internal static OpenApiClient? ComputeOpenApiContract(SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken)
    {
        if (semanticModel.GetDeclaredSymbol(node) is not INamedTypeSymbol namedSymbol)
        {
            return null;
        }

        var specification = string.Empty;
        var useDependencyInjection = false;
        var attributes = namedSymbol.GetAttributes();

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
                    TypedConstant useDependencyInjectionArgument = attribute.ConstructorArguments[1];
                    useDependencyInjection = (bool?)useDependencyInjectionArgument.Value ?? false;
                }
            }
        }

        string className = namedSymbol.Name;
        string namespaceName = namedSymbol.ContainingNamespace.ToString();
        string jsonData = string.Empty;

        // Access project directory (assuming generator has access)
        string projectDirectory = Directory.GetCurrentDirectory(); // Adjust based on your environment

        // Build resource path relative to project
        string resourcePath = Path.Combine(projectDirectory, specification);

        // Check if file exists
        if (File.Exists(resourcePath))
        {
            // Read file content
            try
            {
                jsonData = File.ReadAllText(resourcePath);
            }
            catch
            {
                // ignore
            }
            // Pass jsonData to OpenApiClient class for processing
            // ... (your logic to process jsonData)
        }
        else
        {
            // Handle case where file doesn't exist
            // ... (log error, throw exception)
        }

        //var additionalFileProvider = texts.Transform(texts => texts.FirstOrDefault(file => Path.GetFileName(file.Path) == specification));

        // Use the GetOutput method to get the additional file
        //var additionalFile = additionalFileProvider.GetOutput();

        //if (additionalFile == null)
        //{
        //    // The additional file was not found
        //    return null;
        //}

        jsonData += resourcePath;

        return new OpenApiClient(namespaceName, className, specification, useDependencyInjection, jsonData);
    }
}