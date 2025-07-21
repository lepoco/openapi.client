// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.Cli.Commands;

CommandApp app = new();

app.Configure(config =>
{
    config.Settings.ApplicationName = "openapiclient";
    config.Settings.CaseSensitivity = CaseSensitivity.None;

    config
        .AddCommand<GenerateCommand>("generate")
        .WithDescription("Generates the Open API Client from the provided json.")
        .WithExample(["generate", "my-open-api.json", "--output", "MyOpenApiClient.cs"]);
    ;
});

await app.RunAsync(args);

return;

// Public partial declaration of a class for integration tests
namespace OpenApi.Client.Cli
{
    public partial class Program;
}
