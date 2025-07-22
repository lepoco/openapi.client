// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.Mcp.Services;

internal interface IOpenApiService
{
    /// <summary>
    /// Generates an OpenAPI client from a URL pointing to an OpenAPI or Swagger JSON document.
    /// </summary>
    Task<string> CreateCsharpClientAsync(
        string addressOrFileContents,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Generates a C# code snippet for a given operation ID from the specified OpenAPI document or Swagger-generated JSON.
    /// </summary>
    Task<string> CreateCsharpSnippetAsync(
        string addressOrFileContents,
        string operationId,
        string? baseAddress,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a list of operations (endpoints) from the specified OpenAPI document or Swagger-generated JSON.
    /// </summary>
    Task<string> GetOperationsAsync(
        string addressOrFileContents,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Validates the OpenAPI document or Swagger-generated JSON.
    /// </summary>
    Task<string> ValidateDocumentAsync(
        string addressOrFileContents,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves known responses for the specified OpenAPI document or Swagger-generated JSON.
    /// </summary>
    Task<string> GetKnownResponsesAsync(
        string addressOrFileContents,
        string operationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Generates a cURL command for a given operation ID from the specified OpenAPI JSON file or Swagger-generated JSON.
    /// </summary>
    Task<string> GenerateCurlCommandAsync(
        string addressOrFileContents,
        string operationId,
        string? baseAddress,
        CancellationToken cancellationToken = default
    );
}
