// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.Mcp.Services;

internal interface IOpenApiService
{
    /// <summary>
    /// Creates an OpenAPI client from the specified URL address of the OpenAPI JSON file or Swagger-generated JSON.
    /// </summary>
    Task<string> CreateFromFileAsync(string address, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of operations from the specified OpenAPI JSON file or Swagger-generated JSON.
    /// </summary>
    Task<string> GetOperationsAsync(string address, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a curl command for a given operation ID from the OpenAPI specification.
    /// </summary>
    Task<string> GenerateCurlCommandAsync(
        string address,
        string operationId,
        string? baseAddress,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Validates the OpenAPI document at the specified address and returns a string indicating the validation result.
    /// </summary>
    Task<string> ValidateDocumentAsync(string address, CancellationToken cancellationToken = default);
}
