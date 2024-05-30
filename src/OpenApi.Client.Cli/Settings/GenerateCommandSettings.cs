// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.Cli.Settings;

public sealed class GenerateCommandSettings : CommandSettings
{
    [CommandOption("-f|--file <PATH>")]
    [Description("The Open API file to parse.")]
    public string File { get; set; }

    [CommandOption("-o|--output <PATH>")]
    [Description("The output directory.")]
    public string Output { get; set; }
}
