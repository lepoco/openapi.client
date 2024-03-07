// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Collections.Generic;

namespace OpenApi.Client.SourceGenerators.Schema;

public class ApiDocument
{
    public string Swagger { get; set; }
    public Info Info { get; set; }
    public Dictionary<string, PathItem> Paths { get; set; }
    public List<string> Consumes { get; set; }
}

public class Info
{
    public string Title { get; set; }
    public string Version { get; set; }
}

public class PathItem
{
    public Operation Get { get; set; }
}

public class Operation
{
    public string OperationId { get; set; }
    public string Summary { get; set; }
    public List<string> Produces { get; set; }
    public Dictionary<string, Response> Responses { get; set; }
}

public class Response
{
    public string Description { get; set; }
    public Dictionary<string, string> Examples { get; set; }
}