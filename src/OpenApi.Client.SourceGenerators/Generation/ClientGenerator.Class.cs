// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Text;

namespace OpenApi.Client.SourceGenerators.Genertion;

internal sealed partial class ClientGenerator
{
    private const string ClassHeader = """
            /// <summary>An interface for the <c>{{ContractTitle}}</c> Open API Client.</summary>
            /// <remarks>Generated with Open API Client Source Generator. See: <see href=\"https://github.com/lepoco/openapi.client\"/></remarks>
            {{ContractAccess}} partial class {{ContractClassName}} : I{{ContractClassName}}
            {
                private global::System.Net.HttpStatusCode? lastStatusCode;

                /// <summary>The HTTP Client used for the Open API Client.</summary>
                internal readonly global::System.Net.Http.HttpClient HttpClient;

                public {{ContractClassName}}(global::System.Net.Http.HttpClient httpClient)
                {
                    HttpClient = httpClient;
                }
        """;

    private const string ClassFooter = """

                /// <summary>Gets the last status code from the HTTP request.</summary>
                public global::System.Net.HttpStatusCode? GetLastStatusCode()
                {
                    return lastStatusCode;
                }

                private void ResetStatus()
                {
                    lastStatusCode = null;
                }
            }
        """;

    private void AppendClass(StringBuilder builder)
    {
        builder.AppendLine(ClassHeader);

        //    private void GenerateClass(StringBuilder builder)
        //    {
        //        builder.Append("    public partial class ");
        //        builder.Append(className);
        //        builder.Append(" : I");
        //        builder.AppendLine(className);
        //        builder.AppendLine("    {");
        //        builder.AppendLine(
        //            "        /// <summary>The HTTP Client used for the Open API Client.</summary>"
        //        );
        //        builder.AppendLine(
        //            "        internal readonly global::System.Net.Http.HttpClient HttpClient;"
        //        );
        //        builder.AppendLine();
        //        builder.AppendLine("        private global::System.Net.HttpStatusCode? lastStatusCode;");
        //        builder.AppendLine();
        //        builder.Append("        public ");
        //        builder.Append(className);
        //        builder.AppendLine("(HttpClient httpClient)");
        //        builder.AppendLine("        {");
        //        builder.AppendLine("            HttpClient = httpClient;");
        //        builder.AppendLine("        }");
        //        builder.AppendLine();
        //        builder.AppendLine("            /// <inheritdoc/>");
        //        builder.AppendLine("        public global::System.Net.HttpStatusCode? GetLastStatusCode()");
        //        builder.AppendLine("        {");
        //        builder.AppendLine("            return lastStatusCode;");
        //        builder.AppendLine("        }");
        //        builder.AppendLine();

        //        int count = 0;

        //        foreach (KeyValuePair<string, PathItem> path in apiDocument.Paths)
        //        {
        //            if (count > 0)
        //            {
        //                builder.AppendLine();
        //            }

        //            builder.Append("        /// <inheritdoc/>");
        //            builder.AppendLine();
        //            builder.Append("        public async global::System.Threading.Tasks.Task<");
        //            builder.Append(resultClassName);
        //            builder.Append("> ");
        //            builder.Append(PascalCaseConverter.Convert(path.Value.Get.OperationId));
        //            builder.Append("Async");
        //            builder.AppendLine("(global::System.Threading.CancellationToken cancellationToken)");
        //            builder.AppendLine("        {");
        //            builder.AppendLine("            try");
        //            builder.AppendLine("            {");
        //            builder.Append(
        //                "                using HttpResponseMessage response = await HttpClient.GetAsync(\""
        //            );
        //            builder.Append(path.Key);
        //            builder.AppendLine("\", cancellationToken);");
        //            builder.AppendLine(
        //                "                string responseBody = await response.Content.ReadAsStringAsync();"
        //            );
        //            builder.Append("                return new ");
        //            builder.Append(resultClassName);
        //            builder.AppendLine("(response.StatusCode);");
        //            builder.AppendLine("            }");
        //            builder.AppendLine("            catch (global::System.Exception e)");
        //            builder.AppendLine("            {");
        //            builder.Append("                return new ");
        //            builder.Append(resultClassName);
        //            builder.Append("(new ");
        //            builder.Append(resultClassName);
        //            builder.Append("Error[]{new ");
        //            builder.Append(resultClassName);
        //            builder.AppendLine("Error(e.Message)});");
        //            builder.AppendLine("            }");
        //            builder.AppendLine("        }");

        //            count++;
        //        }

        //        builder.AppendLine("    }");
        //    }
        //}

        builder.AppendLine(ClassFooter);
    }
}
