// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace OpenApi.Client.SourceGenerators.UnitTests.CodeAnalysis;

public static class IncrementalGeneratorExtensions
{
    /// <summary>
    /// Creates a <see cref="GeneratorDriver"/> for the specified generator and source code.
    /// </summary>
    public static GeneratorDriver CreateDriver(
        this IIncrementalGenerator generator,
        string? sourceCode = null,
        params AdditionalText[] additionalTexts
    )
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
            sourceCode
                ?? """

                """
        );

        IEnumerable<MetadataReference> references =
        [
            MetadataReference.CreateFromFile(typeof(IncrementalGeneratorExtensions).Assembly.Location),
        ];

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "OpenApiClientUnitTest",
            syntaxTrees: [syntaxTree],
            references: references
        );

        List<IIncrementalGenerator> generators = [generator];

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: generators.Select(GeneratorExtensions.AsSourceGenerator),
            additionalTexts: additionalTexts
        );

        return driver.RunGenerators(compilation);
    }
}
