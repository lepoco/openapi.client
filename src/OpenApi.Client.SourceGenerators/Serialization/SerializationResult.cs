// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Serialization;

public sealed class SerializationResult<TResult>
{
    public SerializationResult(params SerializationResultError[] errors)
    {
        Result = default;
        Errors = errors;
    }

    public SerializationResult(TResult? result)
    {
        Result = result;
        Errors = Array.Empty<SerializationResultError>();
    }

    public TResult? Result { get; }

    public SerializationResultError[] Errors { get; }

    public bool HasErrors => Errors.Length > 0;

    public static implicit operator SerializationResult<TResult>(SerializationResultError error)
    {
        return new SerializationResult<TResult>(error);
    }
}
