// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using OpenApi.Client.SourceGenerators.Contracts;
using OpenApi.Client.SourceGenerators.Genertion;
using OpenApi.Client.SourceGenerators.Schema;
using OpenApi.Client.SourceGenerators.Serialization;

namespace OpenApi.Client.SourceGenerators;

public partial class OpenApiClientGenerator
{
    private static void Execute(
        SourceProductionContext spc,
        (
            RequestedClassToGenerate? Contract,
            ImmutableArray<(string Name, string Contents)> Files
        ) compilationAndFiles
    )
    {
        OpenApiSerializer serializer = new(spc);
        IApiDocument? openApiDocument = default;
        string documentFilename = "UNKNOWN_DOCUMENT";

        if (compilationAndFiles.Contract is { } value)
        {
            foreach ((string Name, string Contents) file in compilationAndFiles.Files)
            {
                documentFilename = file.Name;

                if (file.Name.Equals(compilationAndFiles.Contract.SelectedFile))
                {
                    openApiDocument = serializer.Serialize(file.Name, file.Contents);
                }
            }

            if (openApiDocument is null)
            {
                spc.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            id: "OAPIC003",
                            title: "Error",
                            messageFormat: $"Document {documentFilename} is empty.",
                            category: "OpenApi.Client.SourceGenerators.DocumentEmpty",
                            defaultSeverity: DiagnosticSeverity.Error,
                            isEnabledByDefault: true
                        ),
                        Location.None
                    )
                );
                return;
            }

            OpenApiContract contract = OpenApiContractParser.Parse(
                compilationAndFiles.Contract.NamespaceName,
                compilationAndFiles.Contract.ClassName,
                compilationAndFiles.Contract.Access,
                openApiDocument
            );
            ClientGenerator generator = new(spc, contract);

            spc.AddSource(
                $"{value.ClassName}.g.cs",
                SourceText.From(generator.Generate(), Encoding.UTF8)
            );
        }
    }
}
