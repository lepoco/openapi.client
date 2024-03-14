// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Converters;

public static class PropertyConverter
{
    public static string Convert(string? value)
    {
        if (string.IsNullOrEmpty(value) || value!.Length == 0)
        {
            return "object?";
        }

        value = value.ToLowerInvariant();

        if (value == "array")
        {
            return "object?[]?";
        }

        if (value == "integer")
        {
            return "int";
        }

        if (value == "number")
        {
            return "double";
        }

        if (value == "boolean")
        {
            return "bool";
        }

        return "object?";
    }
}
