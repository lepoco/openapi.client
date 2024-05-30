// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Generation;

public sealed class GenerationResult<TResult>
{
    public GenerationResult(params GenerationResultError[] errors)
    {
        Result = default;
        Errors = errors;
    }

    public GenerationResult(TResult? result)
    {
        Result = result;
        Errors = Array.Empty<GenerationResultError>();
    }

    public TResult? Result { get; }

    public GenerationResultError[] Errors { get; }

    public bool HasErrors => Errors.Length > 0;

    public static implicit operator GenerationResult<TResult>(GenerationResultError error)
    {
        return new GenerationResult<TResult>(error);
    }

    public static implicit operator GenerationResult<TResult>(TResult? result)
    {
        return new GenerationResult<TResult>(result);
    }
}
