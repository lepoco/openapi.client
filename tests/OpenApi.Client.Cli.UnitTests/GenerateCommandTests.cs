// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.Cli.Commands;
using OpenApi.Client.Cli.UnitTests.Hosting;

namespace OpenApi.Client.Cli.UnitTests;

public sealed class GenerateCommandTests
{
    [Fact]
    public async Task GenerateCommand_Should_ExecuteSuccessfully()
    {
        _ = nameof(GenerateCommand.Validate);
        _ = nameof(GenerateCommand.ExecuteAsync);

        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"{Path.GetRandomFileName()}.cs");

        string getAbsolutePath = Path.GetFullPath("OpenApis/openapi-3.1.0.json");

        await HostFactoryResolver.Run<Program>(
            "generate",
            $"{getAbsolutePath}",
            "--output",
            $"{outputPath}",
            "--namespace",
            "TicTacToeModule.Client",
            "--classname",
            "TicTacToeClient"
        );

        File.Exists(outputPath).Should().BeTrue("The output file should be created.");
        string generatedCode = await File.ReadAllTextAsync(outputPath);

        generatedCode.Should().NotBeNullOrWhiteSpace();
        generatedCode
            .Should()
            .ContainAll(
                "namespace TicTacToeModule.Client",
                "class TicTacToeClient",
                "interface ITicTacToeClient"
            );
    }
}
