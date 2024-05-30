// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.Cli.Settings;

namespace OpenApi.Client.Cli.Commands;

public sealed class GenerateCommand : AsyncCommand<GenerateCommandSettings>
{
    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, GenerateCommandSettings settings)
    {
        await Task.CompletedTask;

        return 0;
    }

    public override ValidationResult Validate(CommandContext context, GenerateCommandSettings settings)
    {
        if (!Path.Exists(settings.File))
        {
            return ValidationResult.Error($"The file '{settings.File}' does not exist.");
        }

        return ValidationResult.Success();
    }
}
