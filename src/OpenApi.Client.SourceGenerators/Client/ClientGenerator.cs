// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Net.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi;
using OpenApi.Client.SourceGenerators.Converters;

namespace OpenApi.Client.SourceGenerators.Client;

public sealed class ClientGenerator(OpenApiDocument document, GeneratorData metadata)
{
    private readonly List<GenerationError> errors = [];

    /// <summary>
    /// Generates the client source code.
    /// </summary>
    /// <returns>The generated source code as a string.</returns>
    public GenerationResult Generate()
    {
        string? client = ComputeClient();

        return new GenerationResult { GeneratedClient = client, Errors = [.. errors] };
    }

    private string ComputeClient()
    {
        // Generate the interface
        InterfaceDeclarationSyntax interfaceDeclaration = SyntaxFactory
            .InterfaceDeclaration('I' + metadata.ClassName)
            .AddModifiers(SyntaxFactory.Token(GetAccess()))
            .AddMembers(ComputeInterfaceMembers().ToArray())
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        // Add namespace
        NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory
            .NamespaceDeclaration(SyntaxFactory.ParseName(metadata.NamespaceName))
            .AddMembers(interfaceDeclaration);

        // Generate the full source code
        CompilationUnitSyntax compilationUnit = SyntaxFactory
            .CompilationUnit()
            .AddMembers(namespaceDeclaration);

        // Add header to the generated file using OpenApiClientGeneration.Header
        SyntaxTriviaList headerTrivia = SyntaxFactory.ParseLeadingTrivia(OpenApiClientGeneration.Header);

        compilationUnit = compilationUnit.WithLeadingTrivia(headerTrivia);

        string compilationResult = compilationUnit.NormalizeWhitespace().ToFullString();

        return compilationResult;
    }

    private IEnumerable<MemberDeclarationSyntax> ComputeInterfaceMembers()
    {
        foreach (KeyValuePair<string, IOpenApiPathItem> openApiPath in document.Paths)
        {
            foreach (
                KeyValuePair<HttpMethod, OpenApiOperation> openApiOperation in openApiPath.Value.Operations
                    ?? []
            )
            {
                string methodName;

                if (openApiOperation.Key == HttpMethod.Get)
                {
                    methodName = "Get";
                }
                else if (openApiOperation.Key == HttpMethod.Post)
                {
                    methodName = "Post";
                }
                else if (openApiOperation.Key == HttpMethod.Delete)
                {
                    methodName = "Delete";
                }
                else if (openApiOperation.Key == HttpMethod.Put)
                {
                    methodName = "Put";
                }
                else
                {
                    methodName = openApiOperation.Key.Method.ToPascalCase();
                }

                methodName += openApiOperation.Value.OperationId?.ToPascalCase();

                yield return SyntaxFactory
                    .MethodDeclaration(
                        SyntaxFactory.IdentifierName("global::System.Threading.Tasks.Task"), // Use global:: prefix for Task
                        methodName
                    )
                    .WithModifiers(SyntaxFactory.TokenList())
                    .WithParameterList(SyntaxFactory.ParameterList())
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }
        }
    }

    private SyntaxKind GetAccess()
    {
        return metadata.Access switch
        {
            Accessibility.Public => SyntaxKind.PublicKeyword,
            Accessibility.Internal => SyntaxKind.InternalKeyword,
            Accessibility.Protected => SyntaxKind.ProtectedKeyword,
            Accessibility.Private => SyntaxKind.PrivateKeyword,
            _ => throw new ArgumentOutOfRangeException(nameof(metadata.Access), metadata.Access, null),
        };
    }
}
