// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using OpenApi.Client.SourceGenerators.Client;

namespace OpenApi.Client.Mcp.Services;

internal sealed class OpenApiService(IServiceProvider serviceProvider, ILogger<OpenApiService> logger)
    : IOpenApiService
{
#if DEBUG
    /// <inheritdoc />
    public async Task<string> CreateFromFileAsync(
        string address,
        CancellationToken cancellationToken = default
    )
    {
        IHttpClientFactory factory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        using HttpClient client = factory.CreateClient("openapi-fetcher");

        Stream contents;

        try
        {
            using HttpResponseMessage response = await client.GetAsync(address, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogFetchingFailed(address);

                return "Failed to fetch OpenAPI document: " + response.ReasonPhrase;
            }

            contents = await response.Content.ReadAsStreamAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogFetchingFailedWithError(e, address);

            return "Error fetching OpenAPI document: " + e.Message;
        }

        ClientGenerator generator = new ClientGenerator(
            new GeneratorData
            {
                NamespaceName = "OpenApi.Client.Generated",
                ClassName = "OpenApiClient",
                Access = Accessibility.Public,
                SerializationTool = SerializationTool.SystemTextJson,
                Source = contents,
            }
        );

        GenerationResult result = await generator.GenerateAsync(cancellationToken);

        if (result.HasErrors)
        {
            foreach (GenerationError error in result.Errors)
            {
                logger.LogError(error.Message);
            }

            return "Errors occurred during client generation, first one was: "
                + result.Errors.First().Message;
        }

        return result.GeneratedClient ?? "Client generation failed, no client was generated.";
    }
#endif

    public async Task<string> GetOperationsAsync(
        string address,
        CancellationToken cancellationToken = default
    )
    {
        ReadResult readResult = await OpenApiDocument.LoadAsync(address, token: cancellationToken);

        if (readResult.Diagnostic?.Errors.Count > 0)
        {
            foreach (OpenApiError error in readResult.Diagnostic?.Errors ?? [])
            {
                logger.LogError(error.Message);
            }

            return "Failed to read OpenAPI document: "
                + string.Join(", ", readResult.Diagnostic?.Errors.Select(e => e.Message) ?? []);
        }

        string result = """
            The following is a list of OpenAPI operations extracted from the specification. Each operation includes its ID, summary, description, HTTP method, and endpoint path. Parameters and responses are included when available. The data is formatted as XML for easier parsing and manipulation.
            <operations>
            """;

        foreach (KeyValuePair<string, IOpenApiPathItem> singlePath in readResult.Document.Paths)
        {
            foreach (
                KeyValuePair<HttpMethod, OpenApiOperation> operation in singlePath.Value.Operations ?? []
            )
            {
                result += $$$"""
                    <operation>
                      <id>{{{operation.Value.OperationId}}}</id>
                      <summary>{{{operation.Value.Summary}}}</summary>
                      <description>{{{operation.Value.Description}}}</description>
                      <path>{{{singlePath.Key}}}</path>
                      <method>{{{operation.Key.Method}}}</method>
                    </operation>
                    """;
            }
        }

        return result + "\n</operations>";
    }

    public async Task<string> GenerateCurlCommandAsync(
        string address,
        string operationId,
        string? baseAddress,
        CancellationToken cancellationToken = default
    )
    {
        ReadResult readResult = await OpenApiDocument.LoadAsync(address, token: cancellationToken);

        if (readResult.Diagnostic?.Errors.Count > 0)
        {
            foreach (OpenApiError error in readResult.Diagnostic?.Errors ?? [])
            {
                logger.LogError(error.Message);
            }

            return "Failed to read OpenAPI document: "
                + string.Join(", ", readResult.Diagnostic?.Errors.Select(e => e.Message) ?? []);
        }

        if (readResult.Document is null)
        {
            return "OpenAPI document is empty or not loaded properly.";
        }

        string result = """
            The following is a list of CURL commands for operations from the OpenAPI specification.
            Each request includes its operation ID and a sample CURL command. 
            The data is formatted in XML for structured parsing by language models.
            <curl_requests>
            """;

        foreach (KeyValuePair<string, IOpenApiPathItem> singlePath in readResult.Document.Paths)
        {
            foreach (
                KeyValuePair<HttpMethod, OpenApiOperation> operation in singlePath.Value.Operations ?? []
            )
            {
                if (operation.Value.OperationId == operationId)
                {
                    string? baseUrl = baseAddress ?? readResult.Document.Servers?.FirstOrDefault()?.Url;
                    string fullUrl = (baseUrl != null ? baseUrl.TrimEnd('/') : string.Empty) + singlePath.Key;

                    result += $$$"""
                        <single_curl_request>
                          <id>{{{operation.Value.OperationId}}}</id>
                          <command>curl -X {{{operation.Key.Method}}} "{{{fullUrl}}}"</command>
                        </single_curl_request>
                        """;
                }
            }
        }

        return result + "\n</curl_requests>";
    }

    public async Task<string> ValidateDocumentAsync(
        string address,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ReadResult readResult = await OpenApiDocument.LoadAsync(address, token: cancellationToken);

            if (readResult.Diagnostic?.Errors.Count > 0)
            {
                string errorsXml = """
                    The following validation errors were found in the OpenAPI document.
                    The data is formatted in XML for structured parsing by language models.
                    <validationErrors>
                    """;

                foreach (OpenApiError error in readResult.Diagnostic.Errors)
                {
                    errorsXml += $$$"""
                            <error>
                                <message>{{{error.Message}}}</message>
                                <pointer>{{{error.Pointer}}}</pointer>
                            </error>
                        """;
                }

                errorsXml += "\n</validationErrors>";

                return errorsXml;
            }

            return "<validationResult>Validation successful: The OpenAPI document is valid.</validationResult>";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during OpenAPI document validation");

            return $$$"""
                    <validationResult>
                        <error>
                            <message>Validation error: {{{ex.Message}}}</message>
                        </error>
                    </validationResult>
                """;
        }
    }
}
