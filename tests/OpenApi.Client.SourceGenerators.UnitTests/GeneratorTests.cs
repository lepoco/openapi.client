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

        ValidationRuleSet ruleSet = ValidationRuleSet.GetDefaultRuleSet();

        ReadResult readResult = await OpenApiModelFactory.LoadAsync(
            input: fileStream,
            format: null,
            settings: new OpenApiReaderSettings { LeaveStreamOpen = false, RuleSet = ruleSet }
        );

        readResult.Document.Should().NotBeNull();
        readResult.Diagnostic?.Errors.Should().BeEmpty();

        ClientGenerator generator = new(
            readResult.Document,
            new GeneratorData
            {
                NamespaceName = "MyCompany.SimpleOverview",
                ClassName = "SimpleOverviewClient",
                Access = Accessibility.Public,
                SerializationTool = SerializationTool.SystemTextJson,
                Location = Location.None,
                SelectedFile = "",
                Templates = null,
            }
        );

        GenerationResult generatorResult = generator.Generate();

        generatorResult.GeneratedClient.Should().NotBeNullOrWhiteSpace();
    }
}
