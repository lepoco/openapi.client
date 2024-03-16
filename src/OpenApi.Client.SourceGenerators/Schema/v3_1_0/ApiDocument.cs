// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Schema.v3_1_0;

// NOTE: https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.0.3.md

/// <summary>
/// This is the root object of the OpenAPI document.
/// </summary>
public sealed class ApiDocument : IApiDocument
{
    public string? OpenApi { get; set; }

    public Info? Info { get; set; }

    public Dictionary<string, PathItem>? Paths { get; set; }

    public Components? Components { get; set; }

    public ApiDocumentVersion? GetOpenApiVersion()
    {
        return ApiDocumentVersion.v3_1_0;
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
        yield break;
        //foreach (KeyValuePair<string, ISchemaType> schema in Components.Schemas)
        //{
        //    if (schema.Value is SchemaType typedSchema)
        //    {
        //        Dictionary<string, string> properties = new Dictionary<string, string>();

        //        string typeName = schema.Key;
        //        string summary = typedSchema.Description;

        //        // 5. Yield ApiDocumentType
        //        yield return new ApiDocumentType
        //        {
        //            Name = typeName,
        //            Summary = summary,
        //            Properties = properties
        //        };
        //    }
        //}
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

public sealed class Info
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? TermsOfService { get; set; }

    public Contact? Contact { get; set; }

    public License? License { get; set; }

    public string? Version { get; set; }
}

public sealed class Contact
{
    public string? Name { get; set; }

    public string? Url { get; set; }

    public string? Email { get; set; }
}

public sealed class License
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
}

public sealed class Operation
{
    public string[]? Tags { get; set; }

    public string? Summary { get; set; }

    public string? Description { get; set; }

    public ExternalDocumentation? ExternalDocs { get; set; }

    public string? OperationId { get; set; }

    public IParameter[]? Parameters { get; set; }

    public IRequestBody? RequestBody { get; set; }

    public Dictionary<string, IResponse>? Responses { get; set; }

    public bool Deprecated { get; set; }
}

public sealed class ExternalDocumentation
{
    public string? Description { get; set; }

    public string? Url { get; set; }
}

public interface IParameter;

public sealed class Parameter : IParameter
{
    public string? Name { get; set; }

    public string? In { get; set; }

    public string? Description { get; set; }

    public bool Required { get; set; }

    public bool Deprecated { get; set; }

    public bool AllowEmptyValue { get; set; }

    public string? Style { get; set; }

    public bool Explode { get; set; }

    public ISchemaType? Schema { get; set; }
}

public sealed class ParameterReference : IParameter
{
    [JsonPropertyName("$ref")]
    public string? Ref { get; set; }
}

public interface IRequestBody;

public sealed class RequestBody : IRequestBody
{
    public string? Description { get; set; }

    public Dictionary<string, MediaType>? Content { get; set; }

    public bool Required { get; set; }
}

public sealed class RequestBodyReference : IRequestBody
{
    [JsonPropertyName("$ref")]
    public string? Ref { get; set; }
}

public sealed class MediaType
{
    public ISchemaType? Schema { get; set; }
}

public interface ISchemaType;

public sealed class SchemaType : ISchemaType
{
    public bool Nullable { get; set; }

    public bool ReadOnly { get; set; }

    public bool WriteOnly { get; set; }

    public bool Deprecated { get; set; }

    public decimal? Minimum { get; set; }

    public decimal? Maximum { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public string? Format { get; set; }

    public ISchemaType? Items { get; set; }

    public Dictionary<string, ISchemaType>? Properties { get; set; }
}

public sealed class SchemaTypeReference : ISchemaType
{
    [JsonPropertyName("$ref")]
    public string? Ref { get; set; }
}

public interface IResponse;

public sealed class Response : IResponse
{
    public string? Description { get; set; }

    public Dictionary<string, MediaType>? Content { get; set; }
}

public sealed class ResponseReference : IResponse
{
    [JsonPropertyName("$ref")]
    public string? Ref { get; set; }
}

public sealed class Components
{
    public Dictionary<string, ISchemaType>? Schemas { get; set; }

    public Dictionary<string, IResponse>? Responses { get; set; }

    public Dictionary<string, IRequestBody>? RequestBodies { get; set; }

    public Dictionary<string, IParameter>? Parameters { get; set; }

    public Dictionary<string, PathItem>? PathItems { get; set; }
}
