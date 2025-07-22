// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using ModelContextProtocol.Server;

// ReSharper disable UnusedMember.Global
namespace OpenApi.Client.Mcp.Tools;

[McpServerToolType]
internal sealed class OpenApiTools
{
    [
        McpServerTool,
        Description(
            "Create OpenAPI client from the given URL address of the OpenAPI json file or swagger generated json"
        )
    ]
    public static async Task<string?> CreateClientFromUrl(
        IOpenApiService service,
        [Description("Url address of the OpenAPI json file or swagger generated json")] string address
    )
    {
        string result = await service.CreateFromFileAsync(address);

        return result;
    }

    [
        McpServerTool,
        Description(
            "Get list of operations from the given URL address of the OpenAPI json file or swagger generated json"
        )
    ]
    public static async Task<string> GetListOfOperations(
        IOpenApiService service,
        [Description("Url address of the OpenAPI json file or swagger generated json")] string address
    )
    {
        string result = await service.GetOperationsAsync(address);

        return result;
    }

    [
        McpServerTool,
        Description("Generate a curl command for a given operation ID from the OpenAPI specification")
    ]
    public static async Task<string?> GenerateCurlCommand(
        IOpenApiService service,
        [Description("Url address of the OpenAPI json file or swagger generated json")] string address,
        [Description("Operation ID for which to generate the curl command")] string operationId,
        [Description("Base address for the curl command, if any (optional)")] string? baseAddress
    )
    {
        string result = await service.GenerateCurlCommandAsync(address, operationId, baseAddress);

        return result;
    }

    [
        McpServerTool,
        Description(
            "Validate the structure and syntax of an OpenAPI document to ensure it adheres to the OpenAPI specification"
        )
    ]
    public static async Task<string> ValidateOpenApiDocument(
        IOpenApiService service,
        [Description("Url address of the OpenAPI json file or swagger generated json")] string address
    )
    {
        string result = await service.ValidateDocumentAsync(address);

        return result;
    }
}
