// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Contracts;

internal sealed class OpenApiContract
{
    public required string Summary { get; init; }

    public required string Version { get; init; }

    public required string License { get; init; }

    public required string Access { get; init; }

    public required string Namespace { get; init; }

    public required string ClassName { get; init; }

    public required string InterfaceName { get; init; }

    public required string ResultClassName { get; init; }

    public required OpenApiMethod[] Methods { get; init; }
}
