// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.UnitTests.CodeAnalysis;

/// <summary>
/// Represents an additional text file containing JSON data for OpenAPI client generation.
/// </summary>
public sealed class JsonAdditionalText(string path, string contents) : AdditionalText
{
    /// <inheritdoc />
    public override string Path => path;

    /// <inheritdoc />
    public override SourceText? GetText(CancellationToken cancellationToken = new CancellationToken()) =>
        SourceText.From(contents, Encoding.UTF8);

    /// <summary>
    /// Creates an instance of <see cref="JsonAdditionalText"/> from a file.
    /// </summary>
    public static JsonAdditionalText FromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file '{filePath}' does not exist.", filePath);
        }

        string name = System.IO.Path.GetFileName(filePath);
        string contents = File.ReadAllText(filePath, Encoding.UTF8);

        return new JsonAdditionalText(name, contents);
    }
}
