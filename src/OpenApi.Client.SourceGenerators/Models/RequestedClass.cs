// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Contracts;

internal sealed class RequestedClass
{
    public required string NamespaceName { get; init; }

    public required string ClassName { get; init; }

    public required RequestedClassAccess Access { get; init; }

    public required SerializationTool SerializationTool { get; init; }
}
