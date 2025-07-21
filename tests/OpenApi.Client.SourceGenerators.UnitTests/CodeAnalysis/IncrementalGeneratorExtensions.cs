// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenApi.Client.SourceGenerators.UnitTests.CodeAnalysis;

public static class IncrementalGeneratorExtensions
{
    /// <summary>
    /// Creates a <see cref="GeneratorDriver"/> for the specified generator and source code.
    /// <para>
    /// @see <see href="https://andrewlock.net/creating-a-source-generator-part-2-testing-an-incremental-generator-with-snapshot-testing/"/>
    /// </para>
    /// <para>
    /// @see <see href="https://github.com/DavidFineboym/LoggingDecoratorGenerator/blob/18c2888d9f5e87345871cfed11147bfdf7a32613/LoggingDecoratorGenerator.Tests/TestHelper.cs#L14"/>
    /// </para>
    /// </summary>
    public static GeneratorDriver RunGenerators(
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

        IEnumerable<PortableExecutableReference> references = AppDomain
            .CurrentDomain.GetAssemblies()
            .Where(static assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            .Select(static assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Concat(
                [MetadataReference.CreateFromFile(typeof(IncrementalGeneratorExtensions).Assembly.Location)]
            );

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
