// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using OpenApi.Client.SourceGenerators.Contracts;
using OpenApi.Client.SourceGenerators.Converters;
using OpenApi.Client.SourceGenerators.Schema;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;

namespace OpenApi.Client.SourceGenerators;

public partial class OpenApiClientGenerator
{
    private static readonly JsonSerializerOptions jsonSettings = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    internal static void Execute(OpenApiContract? clientToGenerate, SourceProductionContext context)
    {
        if (clientToGenerate is { } value)
        {
            context.AddSource(
                $"{value.ClassName}.g.cs",
                SourceText.From(GeneratePartialClass(value), Encoding.UTF8)
            );
        }
    }

    internal static void Generate(
        SourceProductionContext context,
        (OpenApiContract? Contract, ImmutableArray<(string, string)> Files) compilationAndFiles
    )
    {
        if (compilationAndFiles.Contract is { } value)
        {
            foreach ((string, string) file in compilationAndFiles.Files)
            {
                if (
                    file.Item1.ToLower().Contains(compilationAndFiles.Contract.OpenApiSpecification)
                )
                {
                    compilationAndFiles.Contract.ContractData = file.Item2;
                }
            }

            context.AddSource(
                $"{value.ClassName}.g.cs",
                SourceText.From(GeneratePartialClass(value), Encoding.UTF8)
            );
        }
    }

    private static string GeneratePartialClass(OpenApiContract contract)
    {
        return $$$"""
            // <auto-generated/>
            // Generated with Open API Client Source Generator. Created by Leszek Pomianowski and OpenAPI Client Contributors.
            #pragma warning disable
            #nullable enable
            namespace {{{contract.Namespace}}}
            {
                /// <summary>An interface for the <c>{{{contract.OpenApiSpecification}}}</c> Open API Client.</summary>
                /// <remarks>Generated with Open API Client Source Generator. See: <see href="https://github.com/lepoco/openapi.client"/></remarks>
                public interface I{{{contract.ClassName}}}
                {
                    /// <summary>Gets the last status code from the HTTP request.</summary>
                    public global::System.Net.HttpStatusCode? GetLastStatusCode();
                }

                public partial class {{{contract.ClassName}}} : I{{{contract.ClassName}}}
                {
                    /// <summary>The HTTP Client used for the Open API Client.</summary>
                    internal readonly global::System.Net.Http.HttpClient HttpClient;

                    private global::System.Net.HttpStatusCode? lastStatusCode;

                    public {{{contract.ClassName}}}(global::System.Net.Http.HttpClient httpClient)
                    {
                        HttpClient = httpClient;
                    }

                    /// <inheritdoc/>
                    public global::System.Net.HttpStatusCode? GetLastStatusCode()
                    {
                        return lastStatusCode;
                    }

            {{{ComputeOpenApiMethods(contract)}}}
                }
            }
            """;
    }

    private static string ComputeOpenApiMethods(OpenApiContract contract)
    {
        var builder = new StringBuilder("");
        var openApiDocument = JsonSerializer.Deserialize<ApiDocument>(contract.ContractData, jsonSettings);

        foreach (var path in openApiDocument.Paths)
        {
            if (path.Value.Get != null)
            {
                builder.Append("        /// <inheritdoc/>");
                builder.AppendLine();
                builder.Append("        public async Task ");
                builder.Append(PascalCaseConverter.Convert(path.Value.Get.OperationId));
                builder.Append("Async");
                builder.AppendLine("(CancellationToken cancellationToken){await Task.CompletedTask;}");
                builder.AppendLine();
            }
        }

        return builder.ToString();
    }
}
