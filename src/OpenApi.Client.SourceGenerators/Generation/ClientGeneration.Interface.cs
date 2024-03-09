// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Text;

namespace OpenApi.Client.SourceGenerators.Genertion;

internal sealed partial class ClientGenerator
{
    private const string InterfaceHeader = """
            /// <summary>An interface for the <c>{{ContractTitle}}</c> Open API Client.</summary>
            /// <remarks>Generated with Open API Client Source Generator. See: <see href="https://github.com/lepoco/openapi.client"/></remarks>
            public interface I{{ContractClassName}}
            {
                /// <summary>Gets the last status code from the HTTP request.</summary>
                global::System.Net.HttpStatusCode? GetLastStatusCode();
        """;

    private const string InterfaceFooter = """
            }
        """;

    private void AppendInterface(StringBuilder builder)
    {
        builder.AppendLine(InterfaceHeader);

        //        foreach (KeyValuePair<string, PathItem> path in apiDocument.Paths)
        //        {
        //            if (count > 0)
        //            {
        //                builder.AppendLine();
        //            }

        //            builder.Append("        /// <summary>");
        //            builder.Append(path.Value.Get.Summary); // TODO: Format, remove unsafe
        //            builder.Append("</summary>");
        //            builder.AppendLine();
        //            builder.Append("        global::System.Threading.Tasks.Task<");
        //            builder.Append(resultClassName);
        //            builder.Append("> ");
        //            builder.Append(PascalCaseConverter.Convert(path.Value.Get.OperationId));
        //            builder.Append("Async");
        //            builder.AppendLine("(global::System.Threading.CancellationToken cancellationToken);");

        //            count++;
        //        }

        builder.AppendLine(InterfaceFooter);
    }
}
