// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators;

/// <summary>
/// Utilities methods
/// </summary>
internal static class Utils
{
    /// <summary>
    /// Check whether the input argument value is null or not.
    /// </summary>
    /// <typeparam name="T">The input value type.</typeparam>
    /// <param name="value">The input value.</param>
    /// <param name="parameterName">The input parameter name.</param>
    /// <returns>The input value.</returns>
    internal static T CheckArgumentNull<T>(
#if NET5_0_OR_GREATER
        [NotNull] T value,
#else
        T value,
#endif
        [CallerArgumentExpression(nameof(value))] string parameterName = ""
    )
    {
        return value
            ?? throw new ArgumentNullException(parameterName, $"Value cannot be null: {parameterName}");
    }

    /// <summary>
    /// Check whether the input string value is null or empty.
    /// </summary>
    /// <param name="value">The input string value.</param>
    /// <param name="parameterName">The input parameter name.</param>
    /// <returns>The input value.</returns>
    internal static string CheckArgumentNullOrEmpty(
#if NET5_0_OR_GREATER
        [NotNull] string value,
#else
        string value,
#endif
        [CallerArgumentExpression(nameof(value))] string parameterName = ""
    )
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentNullException(
                parameterName,
                $"Value cannot be null or empty: {parameterName}"
            )
            : value;
    }
}
