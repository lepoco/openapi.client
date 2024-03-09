// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using OpenApi.Client.SourceGenerators.Schema;
using System;

namespace OpenApi.Client.SourceGenerators.Contracts;

internal static class OpenApiContractParser
{
    public static OpenApiContract Parse(
        string namespaceName,
        string className,
        Accessibility access,
        IApiDocument document
    )
    {
        return new OpenApiContract
        {
            Namespace = namespaceName,
            ClassName = className,
            Access = GetAccessName(access),
            Title = document.GetTitle() ?? string.Empty,
            Description = document.GetDescription() ?? string.Empty,
            License = document.GetLicense() ?? string.Empty,
            Version = document.GetVersion() ?? string.Empty,
            Methods = Array.Empty<OpenApiMethod>(),
            Objects = Array.Empty<OpenApiObject>(),
        };
    }

    private static string GetAccessName(Accessibility accessibility)
    {
        return "public";
    }
}
