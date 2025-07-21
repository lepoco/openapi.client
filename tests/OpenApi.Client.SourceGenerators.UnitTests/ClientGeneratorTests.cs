// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Client;

namespace OpenApi.Client.SourceGenerators.UnitTests;

public sealed class ClientGeneratorTests
{
    [Theory]
    // [InlineData("OpenApis/openapi-1.2.0.json")]
    [InlineData("OpenApis/openapi-2.0.0.json")]
    [InlineData("OpenApis/openapi-3.0.0.json")]
    [InlineData("OpenApis/openapi-3.0.3.json")]
    [InlineData("OpenApis/openapi-3.0.1.extended.json")]
    [InlineData("OpenApis/openapi-3.0.2.extended.json")]
    [InlineData("OpenApis/openapi-3.1.0.json")]
    public async Task GenerateShouldCreateNotEmptyFile(string file)
    {
        FileStream fileStream = File.OpenRead(file);

        ClientGenerator generator = new(
            new GeneratorData
            {
                Source = fileStream,
                NamespaceName = "MyCompany.SimpleOverview",
                ClassName = "SimpleOverviewClient",
                Access = Accessibility.Public,
                SerializationTool = SerializationTool.SystemTextJson,
                Templates = null,
            }
        );

        GenerationResult generatorResult = await generator.GenerateAsync();

        generatorResult.GeneratedClient.Should().NotBeNullOrWhiteSpace();
        generatorResult.Errors.Should().BeEmpty();
    }
}
