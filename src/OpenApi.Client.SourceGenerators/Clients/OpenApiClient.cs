// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Clients;

internal sealed class OpenApiClient
{
    public readonly string Namespace;

    public readonly string ClassName;

    public readonly string Specification;

    public readonly bool UseDependencyInjection;

    public readonly string JsonData;

    public OpenApiClient(string @namespace, string className, string specification, bool useDependencyInjection, string jsonData)
    {
        Namespace = @namespace;
        ClassName = className;
        Specification = specification;
        UseDependencyInjection = useDependencyInjection;
        JsonData = jsonData;
    }
}