// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.Cli.Settings;
using OpenApi.Client.SourceGenerators.Schema;
using OpenApi.Client.SourceGenerators.Serialization;

namespace OpenApi.Client.Cli.Commands;

/// <summary>
/// Represents the Generate command in the Open API Client CLI.
/// </summary>
/// <remarks>
/// This command reads an Open API file, deserializes it into an IApiDocument object using the OpenApiSerializer,
/// and then creates a new client class.
/// </remarks>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class GenerateCommand : AsyncCommand<GenerateCommandSettings>
{
    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        GenerateCommandSettings settings
    )
    {
        string contents = await File.ReadAllTextAsync(settings.File);

        SerializationResult<IApiDocument>? serializationResult =
            new OpenApiSerializer().Deserialize(settings.File, contents);

        return 0;
    }

    /// <inheritdoc />
    public override ValidationResult Validate(
        CommandContext context,
        GenerateCommandSettings settings
    )
    {
        if (!Path.Exists(settings.File))
        {
            return ValidationResult.Error($"The file '{settings.File}' does not exist.");
        }

        return ValidationResult.Success();
    }
}
