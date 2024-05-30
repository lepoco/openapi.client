// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Contracts;

namespace OpenApi.Client.SourceGenerators.Generation;

internal sealed partial class ClientGenerator
{
    public const string AtttributesHeader = """
            /// <summary>Represents an attribute used to specify the name of a property as it appears in the OpenAPI document.</summary>
            [global::System.CodeDom.Compiler.GeneratedCode("OpenApiClient", "%VERSION%")]
            %ACCESS% sealed class OpenApiPropertyAttribute : global::System.Attribute
            {
                public OpenApiPropertyAttribute(string name)
                {
                    Name = name;
                }

                public string Name { get; }
            }

        """;

    public static void AppendAttributes(StringBuilder builder, OpenApiContract contract)
    {
        builder.AppendLine(AtttributesHeader);
    }
}
