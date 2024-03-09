// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Contracts;

internal readonly struct OpenApiMethod
{
    public readonly required string Summary { get; init; }

    public readonly required string? Description { get; init; }

    public readonly required string DotNetMethodName { get; init; }

    public readonly required string? RequestObjectType { get; init; }

    public readonly required string? ResponseObjectType { get; init; }

    public readonly required OpenApiMethodVerb Verb { get; init; }
}
