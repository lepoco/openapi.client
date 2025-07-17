// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Client;

namespace OpenApi.Client.SourceGenerators.UnitTests;

public sealed class GeneratorTests
{
    [Theory]
    [InlineData("OpenApis/openapi-3.0.3.json")]
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
    }
}
