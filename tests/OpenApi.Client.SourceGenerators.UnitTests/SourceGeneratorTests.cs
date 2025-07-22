// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Linq;
using OpenApi.Client.SourceGenerators.UnitTests.CodeAnalysis;

namespace OpenApi.Client.SourceGenerators.UnitTests;

public sealed class SourceGeneratorTests
{
    [Fact]
    public void Initialize_ShouldGenerateAttribute()
    {
        GeneratorDriver driver = new OpenApiClientGenerator().RunGenerators();

        GeneratorDriverRunResult result = driver.GetRunResult();

        result.GeneratedTrees.Length.Should().BeGreaterThan(0);
        result
            .GeneratedTrees.First()
            .Should()
            .Satisfy<SyntaxTree>(x =>
            {
                x.FilePath.Should().Contain("OpenApiClientAttribute.g.cs");

                string generatedSourceCode = x.GetText().ToString();

                generatedSourceCode
                    .Should()
                    .Contain("internal sealed class OpenApiClientAttribute : global::System.Attribute");
            });
    }

    [Fact]
    public void Initialize_ShouldGenerateSourceCode()
    {
        GeneratorDriver driver = new OpenApiClientGenerator().RunGenerators(
            """
            using OpenApi.Client;

            namespace TickTack.Module
            {
                [OpenApiClient(
                    "openapi-3.1.0",
                    "get-square",
                    "get-board",
                    Serialization = OpenApiClientSerialization.SystemTextJson,
                    Nullable = true,
                    UseRecords = true
                )]
                internal partial class TickTackToeClient
                {
                }
            }
            """,
            JsonAdditionalText.FromFile("OpenApis/openapi-3.1.0.json")
        );

        GeneratorDriverRunResult result = driver.GetRunResult();

        SyntaxTree? generatedClientTree = result.GeneratedTrees.FirstOrDefault(x =>
            x.FilePath.Contains("TickTackToeClient.g.cs")
        );
        generatedClientTree.Should().NotBeNull();

        string generatedText = generatedClientTree.GetText().ToString();

        generatedText
            .Should()
            .ContainAll("internal partial class TickTackToeClient", "GetSquare", "GetBoard")
            .And.NotContainAll("PutSquare");
    }
}
