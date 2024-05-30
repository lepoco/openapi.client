// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.Cli.Settings;

/// <summary>
/// Settings for the Generate command in the Open API Client CLI.
/// </summary>
/// <remarks>
/// This class represents the command line options and arguments for the Generate command.
/// The File property is a required argument that specifies the Open API file to parse.
/// The Output property is an optional option that specifies the output directory. If not provided, the current directory is used.
/// The Serializer property is an optional option that specifies the JSON serializer to use. If not provided, System.Text.Json is used.
/// </remarks>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class GenerateCommandSettings : CommandSettings
{
    /// <summary>
    /// Gets or sets the path to the Open API file to parse.
    /// </summary>
    [CommandArgument(0, "<PATH>")]
    [Description("The Open API file to parse.")]
    public string File { get; set; } = default!;

    /// <summary>
    /// Gets or sets the output directory. If not provided, the current directory is used.
    /// </summary>
    [CommandOption("-o|--output <PATH>")]
    [Description("The output directory.")]
    public string Output { get; set; } = "./";

    /// <summary>
    /// Gets or sets the JSON serializer to use. If not provided, System.Text.Json is used.
    /// </summary>
    [CommandOption("-s|--serializer <TYPE>")]
    [Description("The JSON serializer.")]
    public JsonSerializerType Serializer { get; set; } = JsonSerializerType.SystemTextJson;
}
