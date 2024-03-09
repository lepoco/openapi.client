// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using OpenApi.Client.SourceGenerators.Serialization;

namespace OpenApi.Client.SourceGenerators.Models;

internal sealed class RequestedClassToGenerate
{
    public required string NamespaceName { get; init; }

    public required string ClassName { get; init; }

    public required string SelectedFile { get; init; }

    public required Accessibility Access { get; init; }

    public required SerializationTool SerializationTool { get; init; }
}
