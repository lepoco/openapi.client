// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using OpenApi.Client.SourceGenerators.Converters;
using OpenApi.Client.SourceGenerators.Genertion;
using OpenApi.Client.SourceGenerators.Schema;

namespace OpenApi.Client.SourceGenerators.Contracts;

public static class OpenApiContractParser
{
    public static OpenApiContract Parse(
        string namespaceName,
        string className,
        Accessibility access,
        IApiDocument document
    )
    {
        HashSet<OpenApiType> types = ConvertTypes(document.GetTypes());

        return new OpenApiContract
        {
            Namespace = namespaceName,
            ClassName = className,
            Access = GetAccessName(access),
            Title = document.GetTitle() ?? string.Empty,
            Description = document.GetDescription() ?? string.Empty,
            License = document.GetLicense() ?? string.Empty,
            Version = document.GetVersion() ?? string.Empty,
            Types = types,
            Paths = ConvertPaths(document.GetPaths(), types),
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

    private static HashSet<OpenApiType> ConvertTypes(IEnumerable<ApiDocumentType> apiDocumentTypes)
    {
        HashSet<OpenApiType> types = new();

        foreach (ApiDocumentType type in apiDocumentTypes)
        {
            types.Add(
                new OpenApiType
                {
                    Name = type.Name,
                    Summary = type.Summary,
                    Properties = type.Properties.ToDictionary(
                        apiProperty => PascalCaseConverter.Convert(apiProperty.Key), // Property name
                        apiProperty => PropertyConverter.Convert(apiProperty.Value) // Property type
                    )
                }
            );
        }

        return types;
    }

    private static HashSet<OpenApiPath> ConvertPaths(
        IEnumerable<ApiDocumentPath> apiDocumentPaths,
        HashSet<OpenApiType> types
    )
    {
        HashSet<OpenApiPath> paths = new();

        foreach (ApiDocumentPath path in apiDocumentPaths)
        {
            paths.Add(
                new OpenApiPath
                {
                    Path = path.Path,
                    Name = ComputeMethodName(path.Method, path.OperationId),
                    Summary = RemoveUnsafeWords(path.Summary),
                    RequestBodyType = path.RequestBodyType, // TODO: Convert to OpenApiType, check whether exists
                    RequestQueryType = path.RequestBodyType, // TODO: Convert to OpenApiType, check whether exists
                    ResponseType = path.ResponseType, // TODO: Convert to OpenApiType, check whether exists
                    PathElementsType = path.PathElementsType, // TODO: Convert to OpenApiType, check whether exists
                    Method = path.Method switch
                    {
                        ApiDocumentMethod.Put => OpenApiMethod.Put,
                        ApiDocumentMethod.Post => OpenApiMethod.Post,
                        ApiDocumentMethod.Delete => OpenApiMethod.Delete,
                        ApiDocumentMethod.Options => OpenApiMethod.Options,
                        ApiDocumentMethod.Head => OpenApiMethod.Head,
                        ApiDocumentMethod.Patch => OpenApiMethod.Patch,
                        _ => OpenApiMethod.Get
                    }
                }
            );
        }

        return paths;
    }

    private static string ComputeMethodName(ApiDocumentMethod method, string operationId)
    {
        return /* ComputePrefix(method) + */
            PascalCaseConverter.Convert(RemoveUnsafeWords(operationId)) + "Async";
    }

    private static string? RemoveUnsafeWords(string? input)
    {
        if (input is null)
        {
            return null;
        }

        foreach (string placeholder in ClientGenerator.Placeholders)
        {
            input = input.Replace(placeholder, string.Empty);
        }

        return input;
    }

    private static string ComputePrefix(ApiDocumentMethod method)
    {
        return method switch
        {
            ApiDocumentMethod.Put => "Put",
            ApiDocumentMethod.Post => "Post",
            ApiDocumentMethod.Delete => "Delete",
            ApiDocumentMethod.Options => "Options",
            ApiDocumentMethod.Head => "Head",
            ApiDocumentMethod.Patch => "Patch",
            _ => "Get"
        };
    }
}
