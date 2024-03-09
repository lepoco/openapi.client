// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Collections.Generic;

namespace OpenApi.Client.SourceGenerators.Schema;

// NOTE: https://github.com/OAI/OpenAPI-Specification/blob/main/versions/3.1.0.md#tag-object

/// <summary>
/// This is the root object of the OpenAPI document.
/// </summary>
internal sealed class ApiDocument
{
    /// <summary>
    /// REQUIRED. This string MUST be the version number of the OpenAPI Specification
    /// that the OpenAPI document uses. The openapi field SHOULD be used by tooling to
    /// interpret the OpenAPI document. This is not related to the API info.version string.
    /// </summary>
    public string? OpenApi { get; set; }

    /// <summary>
    /// REQUIRED. Provides metadata about the API. The metadata MAY be used by tooling as required.
    /// </summary>
    public Info Info { get; set; }

    /// <summary>
    /// The default value for the $schema keyword within Schema Objects contained within
    /// this OAS document. This MUST be in the form of a URI.
    /// </summary>
    public string? JsonSchemaDialect { get; set; }

    /// <summary>
    /// The available paths and operations for the API.
    /// </summary>
    public Dictionary<string, PathItem> Paths { get; set; }

    /// <summary>
    /// A list of MIME types the APIs can consume. This is global to all APIs but can be overridden on specific API calls. Value MUST be as described under Mime Types.
    /// </summary>
    public List<string> Consumes { get; set; } // V2

    /// <summary>
    /// A list of MIME types the APIs can produce. This is global to all APIs but can be overridden on specific API calls. Value MUST be as described under Mime Types.
    /// </summary>
    public List<string> Produces { get; set; } // V2

    /// <summary>
    /// A list of tags used by the document with additional metadata. The order of the tags can be used to reflect on their order by the parsing tools. Not all tags that are used by the Operation Object must be declared. The tags that are not declared MAY be organized randomly or based on the tools' logic. Each tag name in the list MUST be unique.
    /// </summary>
    public List<Tag> Tags { get; set; }
}

internal sealed class Info
{
    public string? Title { get; set; }

    public string? Summary { get; set; }

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

    public string? Identifier { get; set; }

    public string? Url { get; set; }
}

internal sealed class Tag
{
    public string? Name { get; set; }

    public string? Description { get; set; }
}

internal sealed class PathItem
{
    public Operation? Get { get; set; }

    public Operation? Put { get; set; }

    public Operation? Post { get; set; }

    public Operation? Delete { get; set; }

    public Operation? Options { get; set; }

    public Operation? Head { get; set; }

    public Operation? Patch { get; set; }

    public Operation? Trace { get; set; }
}

internal sealed class Operation
{
    public string OperationId { get; set; }

    public string? Summary { get; set; }

    public List<string> Produces { get; set; }

    public Dictionary<string, Response> Responses { get; set; }
}

internal sealed class Response
{
    public string Description { get; set; }

    public Dictionary<string, string> Examples { get; set; }
}
