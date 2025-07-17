// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Reflection;

namespace OpenApi.Client.SourceGenerators.Reflection;

internal static class AssemblyVersionProvider
{
    public static string GetInformationalVersion()
    {
        AssemblyInformationalVersionAttribute? informationalVersionAttribute = typeof(AssemblyVersionProvider).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        return informationalVersionAttribute?.InformationalVersion ?? "1.0.0";
    }
}

