// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Schema;

public sealed class ApiDocumentPath
{
    public required string Path { get; init; }

    public required string OperationId { get; init; }

    public required string? Summary { get; init; }

    public required ApiDocumentMethod Method { get; init; }

    public required string? RequestBodyType { get; init; }

    public required string? RequestQueryType { get; init; }

    public required string? PathElementsType { get; init; }

    public required string? ResponseType { get; init; }
}
