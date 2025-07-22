// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

// ReSharper disable UnusedMember.Global
namespace OpenApi.Client.Mcp.Tools;

[McpServerToolType]
internal sealed class OpenApiTools
{
#if DEBUG
    [
        McpServerTool,
        Description(
            "Generate an OpenAPI client from a URL OR a raw JSON string or file contents pointing to an OpenAPI or Swagger JSON document."
        )
    ]
    public static async Task<string?> CreateClientFromUrl(
        IOpenApiService service,
        [Description(
            "URL address of the OpenAPI or Swagger JSON document OR raw JSON contents of the OpenAPI or Swagger specification. Can be provided as text or from a file."
        )]
            string addressOrFileContents
    )
    {
        string result = await service.CreateCsharpClientAsync(addressOrFileContents);

        return result;
    }
#endif

    [
        McpServerTool,
        Description(
            "Generate a C# code snippet for a given operation ID from a URL OR a raw JSON string or file contents pointing to an OpenAPI or Swagger JSON document."
        )
    ]
    public static async Task<string> CreateCsharpSnippet(
        IOpenApiService service,
        [Description(
            "URL address of the OpenAPI or Swagger JSON document OR raw JSON contents of the OpenAPI or Swagger specification. Can be provided as text or from a file."
        )]
            string addressOrFileContents,
        [Description("Operation ID for which to create a snippet.")] string operationId,
        [Description("Optional base URL to be used in the generated cURL command.")] string? baseAddress
    )
    {
        string result = await service.CreateCsharpSnippetAsync(
            addressOrFileContents,
            operationId,
            baseAddress
        );

        return result;
    }

    [
        McpServerTool,
        Description(
            "Retrieve a list of operations (endpoints) from a URL OR a raw JSON string or file contents pointing to an OpenAPI or Swagger JSON document."
        )
    ]
    public static async Task<string> GetListOfOperations(
        IOpenApiService service,
        [Description(
            "URL address of the OpenAPI or Swagger JSON document OR raw JSON contents of the OpenAPI or Swagger specification. Can be provided as text or from a file."
        )]
            string addressOrFileContents
    )
    {
        string result = await service.GetOperationsAsync(addressOrFileContents);

        return result;
    }

    [
        McpServerTool,
        Description(
            "Generate a cURL command for a given operation ID from a URL OR a raw JSON string or file contents pointing to an OpenAPI or Swagger JSON document."
        )
    ]
    public static async Task<string?> GenerateCurlCommand(
        IOpenApiService service,
        [Description(
            "URL address of the OpenAPI or Swagger JSON document OR raw JSON contents of the OpenAPI or Swagger specification. Can be provided as text or from a file."
        )]
            string addressOrFileContents,
        [Description("Operation ID for which to generate the cURL command.")] string operationId,
        [Description("Optional base URL to be used in the generated cURL command.")] string? baseAddress
    )
    {
        string result = await service.GenerateCurlCommandAsync(
            addressOrFileContents,
            operationId,
            baseAddress
        );

        return result;
    }

    [
        McpServerTool,
        Description(
            "Analyze and validate an OpenAPI or Swagger document provided as a URL OR a raw JSON string or file contents, and list possible errors, issues and problems with specification for the specified operation ID."
        )
    ]
    public static async Task<string> ValidateDocument(
        IOpenApiService service,
        [Description(
            "URL address of the OpenAPI or Swagger JSON document OR raw JSON contents of the OpenAPI or Swagger specification. Can be provided as text or from a file."
        )]
            string addressOrFileContents
    )
    {
        string result = await service.ValidateDocumentAsync(addressOrFileContents);

        return result;
    }

    [
        McpServerTool,
        Description(
            "Analyze an OpenAPI or Swagger document provided as a URL OR a raw JSON string or file contents, and list possible HTTP responses (status codes and descriptions) for the specified operation ID."
        )
    ]
    public static async Task<string> GetKnownResponses(
        IOpenApiService service,
        [Description(
            "URL address of the OpenAPI or Swagger JSON document OR raw JSON contents of the OpenAPI or Swagger specification. Can be provided as text or from a file."
        )]
            string addressOrFileContents,
        [Description("Operation ID for which to retrieve known responses.")] string operationId
    )
    {
        string result = await service.GetKnownResponsesAsync(addressOrFileContents, operationId);

        return result;
    }
}
