// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Contracts;

namespace OpenApi.Client.SourceGenerators.Genertion;

internal sealed partial class ClientGenerator
{
    public const string InterfaceHeader = """
            /// <summary>An interface for the <c>%T</c> Open API Client.</summary>
            /// <remarks>Generated with Open API Client Source Generator. See: <see href="https://github.com/lepoco/openapi.client"/></remarks>
            %A interface I%C
            {
                /// <summary>Gets the last status code from the HTTP request.</summary>
                global::System.Net.HttpStatusCode? GetLastStatusCode();

        """;

    public const string InterfaceFooter = """
            }
        """;

    public static void AppendInterface(StringBuilder builder, OpenApiContract contract)
    {
        builder.AppendLine(InterfaceHeader);

        int methodsCount = 0;

        foreach (OpenApiPath path in contract.Paths)
        {
            if (methodsCount > 0)
            {
                builder.AppendLine();
            }

            if (path.Summary?.Length > 0)
            {
                builder.Append("        /// <summary>");
                builder.Append(path.Summary);
                builder.AppendLine("</summary>");
            }

            builder.Append("        global::System.Threading.Tasks.Task<%CResult");

            if (path.ResponseType?.Length > 0)
            {
                builder.Append("<");
                builder.Append(path.ResponseType);
                builder.Append(">");
            }

            builder.Append("> ");
            builder.Append(path.Name);
            builder.Append("(");

            if (path.RequestBodyType?.Length > 0)
            {
                builder.Append(path.RequestBodyType);
                builder.Append(" request, ");
            }

            if (path.RequestQueryType?.Length > 0)
            {
                builder.Append(path.RequestQueryType);
                builder.Append(" query, ");
            }

            builder.AppendLine("global::System.Threading.CancellationToken cancellationToken);");

            methodsCount++;
        }

        builder.AppendLine(InterfaceFooter);
    }
}
