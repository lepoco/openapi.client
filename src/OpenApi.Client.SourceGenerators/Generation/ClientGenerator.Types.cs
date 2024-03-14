// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Contracts;

namespace OpenApi.Client.SourceGenerators.Genertion;

internal sealed partial class ClientGenerator
{
    public static void AppendTypes(StringBuilder builder, OpenApiContract contract)
    {
        if (contract.Types.Count > 0)
        {
            builder.AppendLine();
        }

        int typesCount = 0;

        foreach (OpenApiType type in contract.Types)
        {
            if (typesCount > 0)
            {
                builder.AppendLine();
            }

            if (type.Summary?.Length > 0)
            {
                builder.Append("        /// <summary>");
                builder.Append(type.Summary);
                builder.AppendLine("</summary>");
            }

            builder.Append("    public sealed class ");
            builder.AppendLine(type.Name);
            builder.AppendLine("    {");

            foreach (KeyValuePair<string, string> property in type.Properties)
            {
                builder.Append("        public ");
                builder.Append(property.Value); // Type
                builder.Append(" ");
                builder.Append(property.Key); // Name
                builder.AppendLine(" { get; set; }");
            }

            builder.AppendLine("    }");

            typesCount++;
        }
    }
}
