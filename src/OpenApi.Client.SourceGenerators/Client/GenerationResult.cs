// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Client;

/// <summary>
/// Represents the result of generating an OpenAPI client from a document.
/// </summary>
public sealed record GenerationResult
{
    /// <summary>
    /// The generated client source code.
    /// </summary>
    public required string? GeneratedClient { get; init; }

    /// <summary>
    /// A collection of errors that occurred during the generation process.
    /// </summary>
    public required ImmutableArray<GenerationError> Errors { get; init; }

    /// <summary>
    /// Indicates whether there were any errors during the generation process.
    /// </summary>
    public bool HasErrors => Errors.Length > 0;
}
