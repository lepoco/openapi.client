// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using Microsoft.OpenApi.Models;
using OpenApi.Client.SourceGenerators.Converters;
using OpenApi.Client.SourceGenerators.Generation;

namespace OpenApi.Client.SourceGenerators.Contracts;

public static class OpenApiContractParser
{
    public static OpenApiContract Parse(
        string namespaceName,
        string className,
        Accessibility access,
        OpenApiDocument document
    )
    {
        HashSet<OpenApiType> types = ConvertTypes(document.Components.Schemas);

        return new OpenApiContract
        {
            Namespace = namespaceName,
            ClassName = className,
            Access = GetAccessName(access),
            Title = document.Info?.Title ?? string.Empty,
            Description = document.Info?.Description ?? string.Empty,
            License = document.Info?.License?.Name ?? string.Empty,
            Version = document.Info?.Version ?? string.Empty,
            Types = types,
            Paths = ConvertPaths(document.Paths, types),
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

    private static HashSet<OpenApiType> ConvertTypes(IDictionary<string, OpenApiSchema> schemas)
    {
        HashSet<OpenApiType> types = new();

        //foreach (ApiDocumentType type in apiDocumentTypes)
        //{
        //    types.Add(
        //        new OpenApiType
        //        {
        //            Name = type.Name,
        //            Summary = type.Summary?.Replace("\r\n", " ").Replace("\n", " ").Trim(),
        //            Properties = type.Properties.ToDictionary(
        //                apiProperty => PascalCaseConverter.Convert(apiProperty.Key), // Property name
        //                apiProperty => PropertyConverter.Convert(apiProperty.Value) // Property type
        //            )
        //        }
        //    );
        //}

        return types;
    }

    private static HashSet<OpenApiPath> ConvertPaths(
        OpenApiPaths openApiPaths,
        HashSet<OpenApiType> types
    )
    {
        HashSet<OpenApiPath> computedPaths = new();

        foreach (KeyValuePair<string, OpenApiPathItem> path in openApiPaths)
        {
            OpenApiPathItem pathItem = path.Value;

            foreach (KeyValuePair<OperationType, OpenApiOperation> operation in pathItem.Operations)
            {
                computedPaths.Add(new OpenApiPath
                {
                    Path = path.Key,
                    Name = ComputeMethodName(operation.Key, operation.Value.OperationId),
                    Summary = RemoveUnsafeWords(operation.Value?.Description),
                    RequestBodyType = string.Empty, // TODO: Convert to OpenApiType, check whether exists
                    RequestQueryType = string.Empty, // TODO: Convert to OpenApiType, check whether exists
                    ResponseType = string.Empty, // TODO: Convert to OpenApiType, check whether exists
                    PathElementsType = string.Empty, // TODO: Convert to OpenApiType, check whether exists
                    Method = operation.Key switch
                    {
                        OperationType.Put => OpenApiMethod.Put,
                        OperationType.Post => OpenApiMethod.Post,
                        OperationType.Delete => OpenApiMethod.Delete,
                        OperationType.Options => OpenApiMethod.Options,
                        OperationType.Head => OpenApiMethod.Head,
                        OperationType.Patch => OpenApiMethod.Patch,
                        _ => OpenApiMethod.Get
                    }
                });
            }
        }

        return computedPaths;
    }

    private static string ComputeMethodName(OperationType method, string operationId)
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

    private static string ComputePrefix(OperationType method)
    {
        return method switch
        {
            OperationType.Put => "Put",
            OperationType.Post => "Post",
            OperationType.Delete => "Delete",
            OperationType.Options => "Options",
            OperationType.Head => "Head",
            OperationType.Patch => "Patch",
            _ => "Get"
        };
    }
}
