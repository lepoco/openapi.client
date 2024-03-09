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

    public ApiDocumentVersion? GetOpenApiVersion()
    {
        return ApiDocumentVersion.v2_0_0;
    }

    public string? GetTitle() => Info?.Title;

    public string? GetDescription() => Info?.Description;

    public string? GetLicense() => Info?.License?.Name;

    public string? GetVersion() => Info?.Version;
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
