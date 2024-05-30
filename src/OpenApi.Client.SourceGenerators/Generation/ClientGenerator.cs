// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Contracts;

namespace OpenApi.Client.SourceGenerators.Generation;

internal sealed partial class ClientGenerator(
    OpenApiContract contract,
    ClientGeneratorSerializer serializer
)
{
    public const string GeneratorVersion = "1.0.0";

    public static readonly string[] Placeholders = ["%CLASS%", "%TITLE%", "%ACCESS%", "%VERSION%"];

    public const string DocumentPrefix = """
        // <auto-generated/>
        // Generated with Open API Client Source Generator. Created by Leszek Pomianowski and OpenAPI Client Contributors.
        #pragma warning disable
        #nullable enable
        """;

    public GenerationResult<string> Generate()
    {
        StringBuilder builder = new(DocumentPrefix);

        builder.AppendLine();
        builder.Append("namespace ");
        builder.AppendLine(contract.Namespace);
        builder.AppendLine("{");

        builder.AppendLine(ResultTemplate);
        builder.AppendLine();

        AppendInterface(builder, contract);
        AppendClass(builder, contract, serializer);
        AppendAttributes(builder, contract);
        AppendSerializer(builder, contract, serializer);
        AppendTypes(builder, contract);

        builder.AppendLine("}");

        builder.Replace("%CLASS%", contract.ClassName);
        builder.Replace("%TITLE%", RemoveUnsafeWords(contract.Title));
        builder.Replace("%ACCESS%", contract.Access);
        builder.Replace("%VERSION%", GeneratorVersion);

        return builder.ToString();
    }

    private static string? RemoveUnsafeWords(string? input)
    {
        if (input is null)
        {
            return null;
        }

        foreach (string placeholder in ClientGenerator.Placeholders)
        {
            input = input.Replace(placeholder, string.Empty);
        }

        return input;
    }
}
