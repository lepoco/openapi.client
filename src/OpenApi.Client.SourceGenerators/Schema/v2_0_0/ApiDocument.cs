// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Schema.v2_0_0;

// NOTE: https://github.com/OAI/OpenAPI-Specification/blob/main/versions/2.0.md

internal sealed class ApiDocument : IApiDocument
{
    public string? Swagger { get; set; }

    public Info? Info { get; set; }

    public Dictionary<string, PathItem>? Paths { get; set; }

    public ApiDocumentVersion? GetOpenApiVersion()
    {
        return ApiDocumentVersion.v2_0_0;
    }

    public string? GetTitle() => Info?.Title;

    public string? GetDescription() => Info?.Description;

    public string? GetLicense() => Info?.License?.Name;

    public string? GetVersion() => Info?.Version;

    public IEnumerable<ApiDocumentPath> GetPaths()
    {
        if (Paths is null)
        {
            yield break;
        }

        foreach (KeyValuePair<string, PathItem> path in Paths)
        {
            if (path.Value.Get?.OperationId is not null)
            {
                yield return ComputePath(ApiDocumentMethod.Get, path.Key, path.Value.Get);
            }

            if (path.Value.Put?.OperationId is not null)
            {
                yield return ComputePath(ApiDocumentMethod.Put, path.Key, path.Value.Put);
            }

            if (path.Value.Post?.OperationId is not null)
            {
                yield return ComputePath(ApiDocumentMethod.Post, path.Key, path.Value.Post);
            }

            if (path.Value.Delete?.OperationId is not null)
            {
                yield return ComputePath(ApiDocumentMethod.Delete, path.Key, path.Value.Delete);
            }

            if (path.Value.Options?.OperationId is not null)
            {
                yield return ComputePath(ApiDocumentMethod.Options, path.Key, path.Value.Options);
            }

            if (path.Value.Head?.OperationId is not null)
            {
                yield return ComputePath(ApiDocumentMethod.Head, path.Key, path.Value.Head);
            }

            if (path.Value.Patch?.OperationId is not null)
            {
                yield return ComputePath(ApiDocumentMethod.Patch, path.Key, path.Value.Patch);
            }
        }
    }

    public IEnumerable<ApiDocumentType> GetTypes()
    {
        yield return new ApiDocumentType
        {
            Name = "User",
            Properties = new Dictionary<string, string>
            {
                { "string", "Name" },
                { "int", "Age" },
                { "bool", "IsValid" }
            }
        };
        yield break;
    }

    private ApiDocumentPath ComputePath(ApiDocumentMethod method, string path, Operation operation)
    {
        return new ApiDocumentPath
        {
            Method = method,
            Path = path,
            OperationId = operation.OperationId!,
            Summary = operation.Summary,
            RequestBodyType = string.Empty,
            RequestQueryType = string.Empty,
            PathElementsType = string.Empty,
            ResponseType = string.Empty
        };
    }
}

internal sealed class Info
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? TermsOfService { get; set; }

    public Contact? Contact { get; set; }

    public License? License { get; set; }

    public string? Version { get; set; }
}

internal sealed class Contact
{
    public string? Name { get; set; }

    public string? Url { get; set; }

    public string? Email { get; set; }
}

internal sealed class License
{
    public string? Name { get; set; }

    public string? Url { get; set; }
}

public sealed class PathItem
{
    public Operation? Get { get; set; }

    public Operation? Put { get; set; }

    public Operation? Post { get; set; }

    public Operation? Delete { get; set; }

    public Operation? Options { get; set; }

    public Operation? Head { get; set; }

    public Operation? Patch { get; set; }

    public IParameter[]? Parameters { get; set; }
}

public interface IParameter;

public sealed class Parameter : IParameter
{
    public string? Name { get; set; }

    public string? In { get; set; }

    public string? Description { get; set; }

    public bool Required { get; set; }
}

public sealed class ParameterReference : IParameter
{
    [JsonPropertyName("$ref")]
    public string? Ref { get; set; }
}

public sealed class Operation
{
    public string[]? Tags { get; set; }

    public string? Summary { get; set; }

    public string? Description { get; set; }

    public ExternalDocumentation? ExternalDocs { get; set; }

    public string? OperationId { get; set; }

    public string[]? Consumes { get; set; }

    public string[]? Produces { get; set; }

    public IParameter[]? Parameters { get; set; }

    public Dictionary<string, IResponse>? Responses { get; set; }

    public string[]? Schemes { get; set; }

    public bool Deprecated { get; set; }
}

public sealed class ExternalDocumentation
{
    public string? Description { get; set; }

    public string? Url { get; set; }
}

public interface IResponse;

public sealed class Response : IResponse
{
    public string? Description { get; set; }
}

public sealed class ResponseReference : IResponse
{
    [JsonPropertyName("$ref")]
    public string? Ref { get; set; }
}
