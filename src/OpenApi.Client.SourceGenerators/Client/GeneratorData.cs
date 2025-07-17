// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Client;

/// <summary>
/// Represents the metadata required for generating an OpenAPI client.
/// </summary>
public sealed record GeneratorData
{
    public required string NamespaceName { get; init; }

    public required string ClassName { get; init; }

    public required Accessibility Access { get; init; }

    public required SerializationTool SerializationTool { get; init; }

    public Stream? Source { get; init; }

    public string? Contents { get; init; }

    public required string? Templates { get; init; }
}
