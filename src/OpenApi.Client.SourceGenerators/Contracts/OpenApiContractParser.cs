// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using OpenApi.Client.SourceGenerators.Schema;

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
            Methods = new HashSet<OpenApiMethod>(),
            Objects = new HashSet<OpenApiObject>(),
        };
    }

    private static string GetAccessName(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Internal => "internal",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedAndInternal => "protected internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            _ => "public"
        };
    }
}
