// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using OpenApi.Client.Cli.Settings;
using OpenApi.Client.SourceGenerators.Contracts;
using OpenApi.Client.SourceGenerators.Genertion;
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
    private static readonly Regex PathNormalizer = new(@"(\\\\|//)", RegexOptions.Compiled);

    private static readonly Regex NamespaceValidator =
        new(@"^[_a-zA-Z][_a-zA-Z0-9]*(\.[_a-zA-Z][_a-zA-Z0-9]*)*$", RegexOptions.Compiled);

    private static readonly Regex ClassNameValidator =
        new(@"^[_a-zA-Z][_a-zA-Z0-9]*$", RegexOptions.Compiled);

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        GenerateCommandSettings settings
    )
    {
        using CancellationTokenSource cancellationTokenSource = new();
        string contents = await File.ReadAllTextAsync(settings.File);

        SerializationResult<IApiDocument>? serializationResult =
            new OpenApiSerializer().Deserialize(settings.File, contents);

        if (serializationResult.HasErrors)
        {
            foreach (
                SerializationResultError serializationResultError in serializationResult.Errors
            )
            {
                AnsiConsole.MarkupLine($"[red]Error: {serializationResultError.Message}[/]");
            }

            return -1;
        }

        if (serializationResult.Result is null)
        {
            AnsiConsole.MarkupLine($"[red]Error: Serialized JSON returned empty API.[/]");

            return -2;
        }

        string? generatedSource = null;

        OpenApiContract contract = OpenApiContractParser.Parse(
            settings.Namespace,
            settings.ClassName,
            Accessibility.Public,
            serializationResult.Result
        );

        ClientGenerator generator = new(contract);
        GenerationResult<string> generatorResult = generator.Generate();

        if (generatorResult.HasErrors)
        {
            foreach (GenerationResultError generatorResultError in generatorResult.Errors)
            {
                AnsiConsole.MarkupLine($"[red]Error: {generatorResultError.Message}[/]");
            }

            return -1;
        }

        generatedSource = generatorResult.Result;

        await File.WriteAllTextAsync(
            settings.Output,
            generatedSource,
            cancellationTokenSource.Token
        );

        return 0;
    }

    /// <inheritdoc />
    public override ValidationResult Validate(
        CommandContext context,
        GenerateCommandSettings settings
    )
    {
        if (string.IsNullOrEmpty(settings.ClassName))
        {
            settings.ClassName = Path.GetFileNameWithoutExtension(settings.Output);
        }

        settings.ClassName = settings.ClassName.Trim();

        if (!ClassNameValidator.IsMatch(settings.ClassName))
        {
            return ValidationResult.Error(
                $"The name '{settings.ClassName}' is not valid C# type name."
            );
        }

        settings.Namespace = settings.Namespace.Trim();

        if (!NamespaceValidator.IsMatch(settings.Namespace))
        {
            return ValidationResult.Error(
                $"The namespace '{settings.Namespace}' is not valid C# namespace."
            );
        }

        settings.File = PathNormalizer.Replace(settings.File, "/");
        settings.File = settings.File.Replace("\\", "/");

        if (!Path.Exists(settings.File))
        {
            return ValidationResult.Error($"The file '{settings.File}' does not exist.");
        }

        settings.Output = PathNormalizer.Replace(settings.Output, "/");
        settings.Output = settings.Output.Replace("\\", "/");

        return ValidationResult.Success();
    }
}
