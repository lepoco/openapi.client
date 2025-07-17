// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Net.Http;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using OpenApi.Client.SourceGenerators.Converters;
using OpenApi.Client.SourceGenerators.Readers;
using OpenApi.Client.SourceGenerators.Reflection;

namespace OpenApi.Client.SourceGenerators.Client;

/// <summary>
/// Generates a C# client interface based on an OpenAPI document.
/// </summary>
public sealed class ClientGenerator(GeneratorData metadata)
{
    private readonly List<GenerationError> errors = [];

    private OpenApiDocument document = null!;

    private static readonly string GeneratorVersion = AssemblyVersionProvider.GetInformationalVersion();

    private const string GeneratorName = "OpenApiClient";

    /// <summary>
    /// Generates the client source code.
    /// </summary>
    /// <returns>The generated source code as a string.</returns>
    public async Task<GenerationResult> GenerateAsync(CancellationToken cancellationToken = default)
    {
        OpenApiReaderSettings settings = CreateReaderSettings();
        ReadResult result;

        if (metadata.Source is not null)
        {
            result = await OpenApiModelFactory.LoadAsync(
                input: metadata.Source,
                format: null,
                settings: settings,
                cancellationToken: cancellationToken
            );
        }
        else if (metadata.Contents is not null)
        {
            result = OpenApiModelFactory.Parse(input: metadata.Contents, format: null, settings: settings);
        }
        else
        {
            errors.Add(
                new GenerationError
                {
                    Reason = GenerationErrorReason.DocumentDeserializationFailed,
                    Message = "No source or contents provided for OpenAPI document.",
                }
            );

            return new GenerationResult { GeneratedClient = null, Errors = [.. errors] };
        }

        return CreateClientFromReadResult(result);
    }

    public GenerationResult Generate()
    {
        OpenApiReaderSettings settings = CreateReaderSettings();
        ReadResult result;

        if (metadata.Contents is not null)
        {
            result = OpenApiModelFactory.Parse(input: metadata.Contents, format: null, settings: settings);
        }
        else
        {
            errors.Add(
                new GenerationError
                {
                    Reason = GenerationErrorReason.DocumentDeserializationFailed,
                    Message = "No source or contents provided for OpenAPI document.",
                }
            );

            return new GenerationResult { GeneratedClient = null, Errors = [.. errors] };
        }

        return CreateClientFromReadResult(result);
    }

    private static OpenApiReaderSettings CreateReaderSettings()
    {
        ValidationRuleSet ruleSet = ValidationRuleSet.GetDefaultRuleSet();

        OpenApiReaderSettings settings = new() { LeaveStreamOpen = false, RuleSet = ruleSet };

        settings.Readers.Remove(OpenApiConstants.Json);
        settings.Readers.Add(OpenApiConstants.Json, new CustomOpenApiJsonReader());

        return settings;
    }

    private GenerationResult CreateClientFromReadResult(ReadResult result)
    {
        if (result.Diagnostic?.Errors.Count > 0)
        {
            foreach (OpenApiError? error in result.Diagnostic.Errors)
            {
                errors.Add(
                    new GenerationError
                    {
                        Reason = GenerationErrorReason.DocumentDeserializationFailed,
                        Message = error.Message,
                    }
                );
            }

            return new GenerationResult { GeneratedClient = null, Errors = [.. errors] };
        }

        if (result.Document is null)
        {
            errors.Add(
                new GenerationError
                {
                    Reason = GenerationErrorReason.DocumentDeserializationFailed,
                    Message = "Failed to deserialize OpenAPI document.",
                }
            );

            return new GenerationResult { GeneratedClient = null, Errors = [.. errors] };
        }

        document = result.Document;

        string client;

        try
        {
            client = ComputeClient();
        }
        catch (Exception e)
        {
            errors.Add(
                new GenerationError
                {
                    Reason = GenerationErrorReason.ClientGenerationFailed,
                    Message = $"An error occurred while generating the client: {e.Message}",
                }
            );

            return new GenerationResult { GeneratedClient = null, Errors = [.. errors] };
        }

        GenerationResult generationResult = new() { GeneratedClient = client, Errors = [.. errors] };

        return generationResult;
    }

    private string ComputeClient()
    {
        MemberDeclarationSyntax exceptionDeclaration = ComputeException();
        MemberDeclarationSyntax interfaceDeclaration = ComputeInterface();
        MemberDeclarationSyntax classDeclaration = ComputeClass();
        IEnumerable<MemberDeclarationSyntax> modelsDeclaration = ComputeModels();

        NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory
            .NamespaceDeclaration(SyntaxFactory.ParseName(metadata.NamespaceName))
            .AddMembers([exceptionDeclaration, interfaceDeclaration, classDeclaration, .. modelsDeclaration]);

        CompilationUnitSyntax compilationUnit = SyntaxFactory
            .CompilationUnit()
            .AddMembers(namespaceDeclaration);

        // Add an empty line after OpenApiClientGeneration.Header
        SyntaxTriviaList headerTrivia = SyntaxFactory.ParseLeadingTrivia(OpenApiClientGeneration.Header);

        compilationUnit = compilationUnit.WithLeadingTrivia(headerTrivia);

        string compilationResult = compilationUnit.NormalizeWhitespace().ToFullString();

        return compilationResult;
    }

    private MemberDeclarationSyntax ComputeException()
    {
        SyntaxTriviaList summaryTrivia = SyntaxFactory.TriviaList(
            SyntaxFactory.Comment("/// <summary>"),
            SyntaxFactory.CarriageReturnLineFeed,
            SyntaxFactory.Comment(
                $"/// {metadata.ClassName}Exception is an exception that is thrown when an error occurs in the {metadata.ClassName} client."
            ),
            SyntaxFactory.Comment("/// </summary>"),
            SyntaxFactory.CarriageReturnLineFeed
        );

        return SyntaxFactory
            .ClassDeclaration(metadata.ClassName + "Exception")
            .AddModifiers(SyntaxFactory.Token(metadata.Access.ToSyntaxKind()))
            .AddAttributeLists(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Attribute(
                            SyntaxFactory.ParseName("global::System.CodeDom.Compiler.GeneratedCode"),
                            SyntaxFactory.AttributeArgumentList(
                                SyntaxFactory.SeparatedList(
                                    new[]
                                    {
                                        SyntaxFactory.AttributeArgument(
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Literal(GeneratorName)
                                            )
                                        ),
                                        SyntaxFactory.AttributeArgument(
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Literal(GeneratorVersion)
                                            )
                                        ),
                                    }
                                )
                            )
                        )
                    )
                )
            )
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("global::System.Exception"))
            )
            .WithLeadingTrivia(summaryTrivia)
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);
    }

    private MemberDeclarationSyntax ComputeInterface()
    {
        // Add a meaningful description to the interface, including information about its purpose and details from the OpenAPI document
        SyntaxTriviaList interfaceSummaryTrivia = SyntaxFactory.TriviaList(
            SyntaxFactory.Comment("/// <summary>"),
            SyntaxFactory.CarriageReturnLineFeed,
            SyntaxFactory.Comment(
                $"/// {metadata.ClassName} is an abstraction for a client to an API generated based on the OpenAPI specification."
            ),
            SyntaxFactory.CarriageReturnLineFeed,
            SyntaxFactory.Comment("/// It uses an injected HttpClient to make HTTP requests."),
            SyntaxFactory.CarriageReturnLineFeed
        );

        if (!string.IsNullOrEmpty(document.Info.Summary))
        {
            interfaceSummaryTrivia = interfaceSummaryTrivia.AddRange(
                [
                    SyntaxFactory.Comment("/// <para>"),
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment($"/// {document.Info.Summary}"),
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment("/// </para>"),
                ]
            );
        }

        if (!string.IsNullOrEmpty(document.Info.Description))
        {
            interfaceSummaryTrivia = interfaceSummaryTrivia.AddRange(
                [
                    SyntaxFactory.Comment("/// <para>"),
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment($"/// {document.Info.Description}"),
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment("/// </para>"),
                ]
            );
        }

        interfaceSummaryTrivia = interfaceSummaryTrivia.Add(SyntaxFactory.Comment("/// </summary>"));

        IEnumerable<MemberDeclarationSyntax> interfaceMembers = ComputeInterfaceMembers();

        // Generate the interface with the updated summary
        // Add GeneratedCodeAttribute to the interface
        return SyntaxFactory
            .InterfaceDeclaration('I' + metadata.ClassName)
            .AddModifiers(SyntaxFactory.Token(metadata.Access.ToSyntaxKind()))
            .AddAttributeLists(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Attribute(
                            SyntaxFactory.ParseName("global::System.CodeDom.Compiler.GeneratedCode"),
                            SyntaxFactory.AttributeArgumentList(
                                SyntaxFactory.SeparatedList(
                                    new[]
                                    {
                                        SyntaxFactory.AttributeArgument(
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Literal(GeneratorName)
                                            )
                                        ),
                                        SyntaxFactory.AttributeArgument(
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Literal(GeneratorVersion)
                                            )
                                        ),
                                    }
                                )
                            )
                        )
                    )
                )
            )
            .WithLeadingTrivia(interfaceSummaryTrivia)
            .AddMembers([.. interfaceMembers])
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);
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
                // TODO: Handle parameters, request bodies, and responses
                IdentifierNameSyntax taskType = SyntaxFactory.IdentifierName(
                    "global::System.Threading.Tasks.Task"
                );

                SyntaxTriviaList summaryTrivia = SyntaxFactory.TriviaList(
                    SyntaxFactory.Comment("/// <summary>"),
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment($"/// {openApiOperation.Value.Summary}"),
                    SyntaxFactory.CarriageReturnLineFeed
                );

                if (!string.IsNullOrEmpty(openApiOperation.Value.Description))
                {
                    summaryTrivia = summaryTrivia.AddRange(
                        [
                            SyntaxFactory.Comment("/// <para>"),
                            SyntaxFactory.CarriageReturnLineFeed,
                            SyntaxFactory.Comment($"/// {openApiOperation.Value.Description}"),
                            SyntaxFactory.CarriageReturnLineFeed,
                            SyntaxFactory.Comment("/// </para>"),
                        ]
                    );
                }

                summaryTrivia = summaryTrivia.Add(SyntaxFactory.Comment("/// </summary>"));

                yield return SyntaxFactory
                    .MethodDeclaration(
                        taskType,
                        ComputeOperationName(openApiOperation.Key, openApiOperation.Value)
                    )
                    .WithModifiers(SyntaxFactory.TokenList())
                    .WithParameterList(SyntaxFactory.ParameterList())
                    .WithLeadingTrivia(summaryTrivia) // Add the summary as leading trivia
                    .WithTrailingTrivia(
                        SyntaxFactory.CarriageReturnLineFeed,
                        SyntaxFactory.CarriageReturnLineFeed
                    )
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }
        }
    }

    private MemberDeclarationSyntax ComputeClass()
    {
        IEnumerable<MemberDeclarationSyntax> classMembers = ComputeClassMembers();

        // Add GeneratedCodeAttribute to the class
        return SyntaxFactory
            .ClassDeclaration(metadata.ClassName)
            .AddModifiers(
                SyntaxFactory.Token(metadata.Access.ToSyntaxKind()),
                SyntaxFactory.Token(SyntaxKind.PartialKeyword)
            )
            .AddAttributeLists(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Attribute(
                            SyntaxFactory.ParseName("global::System.CodeDom.Compiler.GeneratedCode"),
                            SyntaxFactory.AttributeArgumentList(
                                SyntaxFactory.SeparatedList(
                                    new[]
                                    {
                                        SyntaxFactory.AttributeArgument(
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Literal(GeneratorName)
                                            )
                                        ),
                                        SyntaxFactory.AttributeArgument(
                                            SyntaxFactory.LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                SyntaxFactory.Literal(GeneratorVersion)
                                            )
                                        ),
                                    }
                                )
                            )
                        )
                    )
                )
            )
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName('I' + metadata.ClassName))
            )
            .AddMembers([.. classMembers])
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);
    }

    private IEnumerable<MemberDeclarationSyntax> ComputeClassMembers()
    {
        foreach (KeyValuePair<string, IOpenApiPathItem> openApiPath in document.Paths)
        {
            foreach (
                KeyValuePair<HttpMethod, OpenApiOperation> openApiOperation in openApiPath.Value.Operations
                    ?? []
            )
            {
                // TODO: Handle parameters, request bodies, and responses
                IdentifierNameSyntax taskType = SyntaxFactory.IdentifierName(
                    "global::System.Threading.Tasks.Task"
                );

                yield return SyntaxFactory
                    .MethodDeclaration(
                        taskType,
                        ComputeOperationName(openApiOperation.Key, openApiOperation.Value)
                    )
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(SyntaxFactory.ParameterList())
                    .WithBody(
                        SyntaxFactory.Block(
                            SyntaxFactory.SingletonList<StatementSyntax>(
                                SyntaxFactory.ReturnStatement(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("global::System.Threading.Tasks.Task"),
                                        SyntaxFactory.IdentifierName("CompletedTask")
                                    )
                                )
                            )
                        )
                    )
                    .WithLeadingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.Comment("/// <inheritdoc />")))
                    .WithTrailingTrivia(
                        SyntaxFactory.CarriageReturnLineFeed,
                        SyntaxFactory.CarriageReturnLineFeed
                    );
            }
        }
    }

    private IEnumerable<MemberDeclarationSyntax> ComputeModels()
    {
        // TODO: Implement model generation based on OpenAPI document
        yield break;
    }

    private static string ComputeOperationName(HttpMethod httpMethod, OpenApiOperation operation)
    {
        StringBuilder nameBuilder = new();

        nameBuilder.Append(
            httpMethod.Method switch
            {
                "GET" => nameof(HttpMethod.Get),
                "POST" => nameof(HttpMethod.Post),
                "DELETE" => nameof(HttpMethod.Delete),
                "PUT" => nameof(HttpMethod.Put),
                "HEAD" => nameof(HttpMethod.Head),
                "OPTIONS" => nameof(HttpMethod.Options),
                "TRACE" => nameof(HttpMethod.Trace),
                "PATCH" => "Patch",
                _ => httpMethod.Method.ToPascalCase(),
            }
        );

        return nameBuilder.Append(operation.OperationId?.ToPascalCase()).ToString();
    }
}
