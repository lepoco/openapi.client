# ☄️ OpenAPI Client

[Created with ❤ in Poland by lepo.co](https://lepo.co/) and [the awesome open-source community](https://github.com/lepoco/openapi.client/graphs/contributors).  
OpenAPI Client is a modern toolkit for developers working with OpenAPI/Swagger specifications. It provides multiple tools to simplify the creation, inspection, validation, and integration of OpenAPI clients across .NET projects and MCP (Model Context Protocol) environments.

## 🧰 What's in this repo?

This repository contains a suite of OpenAPI tools designed to support different workflows:

| Tool                     | Description                                                                    |
| ------------------------ | ------------------------------------------------------------------------------ |
| 🧠 .NET Source Generator | Generates C# HTTP clients directly from OpenAPI JSON/YAML files at build time. |
| 💻 .NET CLI Tool         | Command-line tool to generate clients from OpenAPI definitions manually.       |
| 🛰️ MCP Server            | Containerized server with built-in tools for working with OpenAPI specs.       |

## 🚀 MCP Server Tools

The MCP Server is the central piece of this toolkit. It runs as a standalone container or inside an MCP setup and exposes tools for working with OpenAPI documents.

### 🛠️ Available Server Tools

| Tool                     | Description                                                                                |
| ------------------------ | ------------------------------------------------------------------------------------------ |
| `get_list_of_operations` | Lists all operation IDs from a given OpenAPI or Swagger JSON document.                     |
| `get_known_responses`    | Lists all known responses for given operation IDs from a OpenAPI or Swagger JSON document. |
| `validate_document`      | Validates the structure and syntax of an OpenAPI JSON document.                            |
| `generate_curl_command`  | Generates a cURL command for a specific operation ID.                                      |
| `create_csharp_snippet`  | Creates a simple HTTP request for a given operation ID.                                    |

### 🐳 Run the server with Docker

Build the image:

```bash
docker buildx build ./ -t mcp/openapi --no-cache
# or
dotnet publish ./src/OpenApi.Client.Mcp/OpenApi.Client.Mcp.csproj -c Release /t:PublishContainer
```

Run the container:

```bash
docker run -d -i --rm --name mcp-openapi mcp/openapi
# or for HTTP mode:
docker run -d -i --rm --name mcp-openapi mcp/openapi -e MODE=Http -p 64622:8080
```

Example MCP config (`.mcp.json`):

```json
{
  "servers": {
    "openapi": {
      "type": "stdio",
      "command": "docker",
      "args": ["run", "-i", "--rm", "mcp/openapi"]
    }
  },
  "inputs": []
}
```

## 📦 NuGet Package (Source Generator)

OpenApiClient is available as NuGet package on NuGet.org:  
<https://www.nuget.org/packages/OpenApiClient>

Install the package to enable automatic OpenAPI client generation:

```powershell
dotnet add package OpenApiClient
```

, or package manager console:

```powershell
NuGet\Install-Package OpenApiClient
```

In your .csproj:

```xml
<ItemGroup>
  <AdditionalFiles Include="google.youtube.api.json" />
</ItemGroup>
```

Define a client:

```csharp
[OpenApiClient("google.youtube.api")]
public partial class YouTubeClient;
```

Use it:

```csharp
var client = new YouTubeClient(new HttpClient());
var subs = await client.SubscribersCountAsync("mychannel", CancellationToken.None);
```

### Known limitations

Since we are using the generated internal `OpenApiAttribute` as a marker, conflicts may occur when we use `InternalsVisibleTo`.

We found the use of nullable essential, so C# 8.0 is required.

## 💻 .NET CLI Tool

Generate OpenAPI clients from the terminal:

```bash
dotnet tool install --global OpenApiClient.Cli
```

```bash
dotnet openapi generate ./google.youtube.api.json --output ./clients/YouTubeClient.cs --namespace Google.YouTube --classname YouTubeClient
```

## OpenAPI

OpenAPI specification is available at:  
<https://github.com/OAI/OpenAPI-Specification>

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.

## License

**OpenAPI Client** is free and open source software licensed under **MIT License**. You can use it in private and commercial projects.  
Keep in mind that you must include a copy of the license in your project.
