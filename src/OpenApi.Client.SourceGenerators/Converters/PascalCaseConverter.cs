// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Text;
using System.Text.RegularExpressions;

namespace OpenApi.Client.SourceGenerators.Converters;

internal static class PascalCaseConverter
{
    private static readonly Regex regex = new Regex(@"[^a-zA-Z0-9\s]", RegexOptions.Compiled);

    public static string Convert(string? value)
    {
        if (string.IsNullOrEmpty(value) || value!.Length == 0)
        {
            return string.Empty;
        }

        value = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));

        value = regex.Replace(value, string.Empty);

        return char.ToUpperInvariant(value[0]) + value.Substring(1);
    }
}
