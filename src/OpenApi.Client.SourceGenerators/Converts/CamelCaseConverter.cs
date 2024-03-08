// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Converters;

internal static class CamelCaseConverter
{
    public static string Convert(string? value)
    {
        if (!string.IsNullOrEmpty(value) && value.Length > 1)
        {
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        return value ?? string.Empty;
    }
}