// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Text.RegularExpressions;

namespace OpenApi.Client.SourceGenerators.Converters;

public static class CamelCaseConverter
{
    private static readonly Regex regex = new Regex(@"[^a-zA-Z0-9\s]", RegexOptions.Compiled);

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

        for (int i = 0; i < words.Length; i++)
        {
            if (i == 0)
            {
                words[i] = char.ToLowerInvariant(words[i][0]) + words[i].Substring(1);
            }
            else
            {
                words[i] = char.ToUpperInvariant(words[i][0]) + words[i].Substring(1);
            }
        }

        return string.Join("", words);
    }
}
