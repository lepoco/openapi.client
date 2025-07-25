// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Client;

/// <summary>
/// Specifies the serialization tool to be used for generating the OpenAPI client.
/// </summary>
public enum SerializationTool
{
    /// <summary>
    /// Uses System.Text.Json for serialization.
    /// </summary>
    SystemTextJson,

    /// <summary>
    /// Uses Newtonsoft.Json for serialization.
    /// </summary>
    NewtonsoftJson,
}
