// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Schema.v1_2_0;

// NOTE: https://github.com/OAI/OpenAPI-Specification/blob/main/versions/1.2.md

public sealed class ApiDocument : IApiDocument
{
    public string? SwaggerVersion { get; set; }

    public string? ApiVersion { get; set; }

    public Resource[]? Apis { get; set; }

    public Info? Info { get; set; }

    public Dictionary<string, Authorization>? Authorizations { get; set; }

    public ApiDocumentVersion? GetOpenApiVersion()
    {
        return ApiDocumentVersion.v1_2_0;
    }

    public string? GetTitle() => Info?.Title;

    public string? GetDescription() => Info?.Description;

    public string? GetLicense() => Info?.License;

    public string? GetVersion() => ApiVersion;

    public IEnumerable<ApiDocumentPath> GetPaths()
    {
        yield break;
    }

    public IEnumerable<ApiDocumentType> GetTypes()
    {
        yield break;
    }
}

public sealed class Info
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? TermsOfServiceUrl { get; set; }

    public string? Contact { get; set; }

    public string? License { get; set; }

    public string? LicenseUrl { get; set; }
}

public sealed class Resource
{
    public string? Path { get; set; }

    public string? Description { get; set; }
}

public sealed class Authorization
{
    public string? Type { get; set; }

    public string? PassAss { get; set; }

    public string? KeyName { get; set; }

    public AuthorizationScope[]? Scopes { get; set; }
}

public sealed class AuthorizationScope
{
    public string? Scope { get; set; }

    public string? Description { get; set; }
}
