// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Threading.Tasks;
using OpenApi.Client.Cli.Settings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace OpenApi.Client.Cli.Commands;

public sealed class GenerateCommand : ICommand<GenerateCommandSettings>
{
    /// <inheritdoc />
    public Task<int> Execute(CommandContext context, GenerateCommandSettings settings)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ValidationResult Validate(CommandContext context, CommandSettings settings)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<int> Execute(CommandContext context, CommandSettings settings)
    {
        throw new NotImplementedException();
    }
}
