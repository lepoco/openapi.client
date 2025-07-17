// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;

namespace OpenApi.Client.SourceGenerators.Client;

public sealed record GeneratorData
{
    public required string NamespaceName { get; init; }

    public required string ClassName { get; init; }

    public required string SelectedFile { get; init; }

    public required Accessibility Access { get; init; }

    public required SerializationTool SerializationTool { get; init; }

    public required Location? Location { get; init; }

    public required string? Templates { get; init; }
}
