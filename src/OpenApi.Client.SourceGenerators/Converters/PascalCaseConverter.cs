// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Text.RegularExpressions;

namespace OpenApi.Client.SourceGenerators.Converters;

public static class PascalCaseConverter
{
    private static readonly Regex regex = new Regex(@"[^a-zA-Z0-9\s]", RegexOptions.Compiled);

    /// <summary>
    /// Converts a string to PascalCase.
    /// </summary>
    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        // Convert to PascalCase using the Convert method
        return Convert(value);
    }

    /// <summary>
    /// Converts a string to PascalCase by removing special characters and capitalizing the first letter of each word.
    /// </summary>
    public static string Convert(string? value)
    {
        if (string.IsNullOrEmpty(value) || value!.Length == 0)
        {
            return string.Empty;
        }

        value = value.Replace('_', ' ').Replace('-', ' ');

        value = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));

        value = regex.Replace(value, string.Empty);

        string[] words = value.Split(' ');
        IEnumerable<string> capitalizedWords = words.Select(word =>
            char.ToUpperInvariant(word[0]) + word.Substring(1)
        );

        return string.Join("", capitalizedWords);
    }
}
