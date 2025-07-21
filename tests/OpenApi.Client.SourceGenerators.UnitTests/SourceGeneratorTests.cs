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
        GeneratorDriver driver = new OpenApiClientGenerator().CreateDriver();

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
        GeneratorDriver driver = new OpenApiClientGenerator().CreateDriver(
            """
            using OpenApi.Client;

            namespace OpenApiClientUnitTest
            {
                [OpenApiClient("openapi-3.1.0")]
                public partial class MyClient
                {
                }
            }
            """,
            JsonAdditionalText.FromFile("OpenApis/openapi-3.1.0.json")
        );

        GeneratorDriverRunResult result = driver.GetRunResult();

        var test = 1;
    }
}
