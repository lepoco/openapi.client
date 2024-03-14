// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Contracts;

public readonly struct OpenApiType
{
    public readonly required string Name { get; init; }

    public readonly string? Summary { get; init; }

    public required IDictionary<string, string> Properties { get; init; }
}
