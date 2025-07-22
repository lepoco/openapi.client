// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using OpenApi.Client.SourceGenerators.Client;
using OpenApi.Client.SourceGenerators.Converters;
using System.Net;

namespace OpenApi.Client.Mcp.Services;

/**
 * @see https://docs.anthropic.com/en/docs/build-with-claude/prompt-engineering/use-xml-tags
 * @see https://swagger.io/specification/
 */
internal sealed class OpenApiService(HttpClient client, ILogger<OpenApiService> logger) : IOpenApiService
{
    /// <inheritdoc />
    public async Task<string> CreateCsharpClientAsync(
        string addressOrFileContents,
        CancellationToken cancellationToken = default
    )
    {
        string? specification = await PrepareContents(addressOrFileContents, cancellationToken);

        if (specification is null)
        {
            return "Error fetching OpenAPI document";
        }

        ClientGenerator generator = new(
            new GeneratorData
            {
                NamespaceName = "OpenApi.Client.Generated",
                ClassName = "OpenApiClient",
                Access = Accessibility.Public,
                SerializationTool = SerializationTool.SystemTextJson,
                Contents = specification,
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

    public async Task<string> GetOperationsAsync(
        string addressOrFileContents,
        CancellationToken cancellationToken = default
    )
    {
        string? specification = await PrepareContents(addressOrFileContents, cancellationToken);

        if (specification is null)
        {
            return "Error fetching OpenAPI document";
        }

        ReadResult readResult = OpenApiModelFactory.Parse(input: specification);

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

        foreach (KeyValuePair<string, IOpenApiPathItem> singlePath in readResult.Document?.Paths ?? [])
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

        await Task.CompletedTask;

        return result + "\n</operations>";
    }

    public async Task<string> GetKnownResponsesAsync(
        string addressOrFileContents,
        string operationId,
        CancellationToken cancellationToken = default
    )
    {
        string? specification = await PrepareContents(addressOrFileContents, cancellationToken);

        if (specification is null)
        {
            return "Error fetching OpenAPI document";
        }

        ReadResult readResult = OpenApiModelFactory.Parse(input: specification);

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
            The following is a list of HTTP responses for the specified operation.
            <operation_responses>
            """;

        foreach (KeyValuePair<string, IOpenApiPathItem> singlePath in readResult.Document.Paths)
        {
            foreach (
                KeyValuePair<HttpMethod, OpenApiOperation> operation in singlePath.Value.Operations ?? []
            )
            {
                if (operation.Value.OperationId != operationId)
                {
                    continue;
                }

                foreach (KeyValuePair<string, IOpenApiResponse> response in operation.Value.Responses ?? [])
                {
                    string? statusCodeName = null;
                    if (int.TryParse(response.Key, out int statusCode))
                    {
                        HttpStatusCode statusCodeEnum = (System.Net.HttpStatusCode)statusCode;
                        statusCodeName = statusCodeEnum.ToString();
                    }

                    result += $$$"""
                        <single_operation_response>
                        <id>{{{operation.Value.OperationId}}}</id>
                        <path>{{{singlePath.Key}}}</path>
                        <method>{{{operation.Key.Method}}}</method>
                        <response_description>{{{response.Value.Description}}}</response_description>
                        <http_status_code>{{{response.Key}}}</http_status_code>
                        {{{(
                            statusCodeName is not null
                                ? $"<http_status_name>{statusCodeName}</http_status_name>"
                                : string.Empty
                        )}}}
                        </single_operation_response>
                        """;
                }
            }
        }

        return result + "\n</operation_responses>";
    }

    public async Task<string> GenerateCurlCommandAsync(
        string addressOrFileContents,
        string operationId,
        string? baseAddress,
        CancellationToken cancellationToken = default
    )
    {
        string? specification = await PrepareContents(addressOrFileContents, cancellationToken);

        if (specification is null)
        {
            return "Error fetching OpenAPI document";
        }

        ReadResult readResult = OpenApiModelFactory.Parse(input: specification);

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
        string addressOrFileContents,
        CancellationToken cancellationToken = default
    )
    {
        string? specification = await PrepareContents(addressOrFileContents, cancellationToken);

        if (specification is null)
        {
            return "Error fetching OpenAPI document";
        }

        try
        {
            ReadResult readResult = OpenApiModelFactory.Parse(input: specification);

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

    public async Task<string> CreateCsharpSnippetAsync(
        string addressOrFileContents,
        string operationId,
        string? baseAddress,
        CancellationToken cancellationToken = default
    )
    {
        string? specification = await PrepareContents(addressOrFileContents, cancellationToken);

        if (specification is null)
        {
            return "Error fetching OpenAPI document";
        }

        ReadResult readResult = OpenApiModelFactory.Parse(input: specification);

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
            <snippets>
            """;

        foreach (KeyValuePair<string, IOpenApiPathItem> singlePath in readResult.Document.Paths)
        {
            foreach (
                KeyValuePair<HttpMethod, OpenApiOperation> operation in singlePath.Value.Operations ?? []
            )
            {
                if (operation.Value.OperationId == operationId)
                {
                    string method = operation.Key.Method switch
                    {
                        "GET" => nameof(HttpMethod.Get),
                        "POST" => nameof(HttpMethod.Post),
                        "DELETE" => nameof(HttpMethod.Delete),
                        "PUT" => nameof(HttpMethod.Put),
                        "HEAD" => nameof(HttpMethod.Head),
                        "OPTIONS" => nameof(HttpMethod.Options),
                        "TRACE" => nameof(HttpMethod.Trace),
                        "PATCH" => "Patch",
                        _ => operation.Key.Method.ToPascalCase(),
                    };

                    string? baseUrl = baseAddress ?? readResult.Document.Servers?.FirstOrDefault()?.Url;
                    string fullUrl = (baseUrl != null ? baseUrl.TrimEnd('/') : string.Empty) + singlePath.Key;

                    // TODO: Query parameters and content, likeContent = new StringContent("", Encoding.UTF8, "application/json"),
                    string snippet = $$$"""
                        <snippet>
                        using System.Net.Http;
                        using System.Threading.Tasks;

                        using HttpClient client = services.GetRequiredService<IHttpClientFactory>().CreateClient("{{{operation.Value.OperationId}}}");

                        using HttpResponseMessage response = await client.SendAsync(
                            new HttpRequestMessage
                            {
                                RequestUri = new Uri("{{{fullUrl}}}", UriKind.Absolute),
                                Method = HttpMethod.{{{method}}}
                            },
                            cancellationToken
                        );

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Failed to execute operation {{{operation.Value.OperationId}}} with status code: {response.StatusCode}");
                        }
                        </snippet>
                        """;

                    return snippet;
                }
            }
        }

        return result + "\n</snippets>";
    }

    private async Task<string?> PrepareContents(
        string addressOrFileContents,
        CancellationToken cancellationToken
    )
    {
        if (IsUrl(addressOrFileContents))
        {
            return await FetchContents(addressOrFileContents, cancellationToken);
        }

        return addressOrFileContents;
    }

    private async Task<string?> FetchContents(string address, CancellationToken cancellationToken)
    {
        string contents;

        try
        {
            using HttpResponseMessage response = await client.GetAsync(address, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogFetchingFailed(address);

                return null;
            }

            contents = await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogFetchingFailedWithError(e, address);

            return null;
        }

        return contents;
    }

    private static bool IsUrl(string input)
    {
        return Uri.TryCreate(input, UriKind.Absolute, out Uri? uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
