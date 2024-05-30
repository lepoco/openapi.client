// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.Cli.Settings;

/// <summary>
/// Specifies the type of JSON serializer to use.
/// </summary>
/// <remarks>
/// This enum is used in the GenerateCommandSettings to allow the user to choose the JSON serializer.
/// SystemTextJson represents the System.Text.Json serializer.
/// NewtonsoftJson represents the Newtonsoft.Json serializer.
/// </remarks>
public enum JsonSerializerType
{
    /// <summary>
    /// Represents the System.Text.Json serializer.
    /// </summary>
    SystemTextJson,

    /// <summary>
    /// Represents the Newtonsoft.Json serializer.
    /// </summary>
    NewtonsoftJson
}
