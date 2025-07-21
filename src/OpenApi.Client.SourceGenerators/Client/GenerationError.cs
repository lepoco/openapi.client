// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Client;

/// <summary>
/// Represents an error that occurred during the generation of an OpenAPI client.
/// </summary>
public sealed record GenerationError
{
    /// <summary>
    /// The location of the error in the source code, if available.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Reason for the generation error.
    /// </summary>
    public required GenerationErrorReason Reason { get; init; }
}
