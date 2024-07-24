# ☄️ OpenAPI Client

[Created with ❤ in Poland by lepo.co](https://dev.lepo.co/)  
OpenAPI Client is a toolkit that helps you create HTTP clients for external APIs based on their OpenAPI specifications. It simplifies the process of consuming and interacting with various web services. The project is developed and maintained by lepo.co and other community contributors.

## 👀 What does this repo contain?

The repository contains NuGet package source code, which uses C# code generators that can be used to generate native C# API clients from YAML or JSON files.

## Gettings started

OpenApiClient is available as NuGet package on NuGet.org:  
https://www.nuget.org/packages/OpenApiClient

You can add it to your project using .NET CLI:

```powershell
dotnet add package OpenApiClient
```

, or package manager console:

```powershell
NuGet\Install-Package OpenApiClient
```

, you can also use the .NET CLI tool to generate the classes using commands

```powershell
dotnet tool install --global OpenApiClient.Cli
```

Define an Open API file as content in your  **.csproj** file.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net8.0</TargetFrameworks>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="OpenApiClient" Version="1.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>compile; build; analyzers</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <OpenApiContract Include="google.youtube.api.json" />
  </ItemGroup>

</Project>
```

Define your partial class as open api client

```csharp
/// <summary>
/// My YouTube Client.
/// </summary>
[OpenApiClient("google.youtube.api")]
public partial class YouTubeClient;
```

You can now use your generated client!

```csharp
IYouTubeClient client = new YouTubeClient(new HttpClient());

var subscribersCount = client.SubscribersCountAsync("mychannel", CancellationToken.None);
```

## Known limitations

Since we are using the generated internal `OpenApiAttribute` as a marker, conflicts may occur when we use `InternalsVisibleTo`.

We found the use of nullable essential, so C# 8.0 is required.

## OpenAPI

OpenAPI specification is available at:  
https://github.com/OAI/OpenAPI-Specification

## Community Toolkit

The OpenAPI Client is inspired by the MVVM Community Toolkit:  
https://github.com/CommunityToolkit/dotnet

## Special thanks

JetBrains was kind enough to lend a license for the open-source **dotUltimate** for Open API Client development. Learn more here:

- https://www.jetbrains.com/dotnet/
- https://www.jetbrains.com/opensource/

## Compilation

Use Visual Studio 2022 and invoke the .sln.

Visual Studio  
**OpenAPI Client** is an Open Source project. You are entitled to download and use the freely available Visual Studio Community Edition to build, run or develop for OpenAPI Client. As per the Visual Studio Community Edition license, this applies regardless of whether you are an individual or a corporate user.

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.

## License

**OpenAPI Client** is free and open source software licensed under **MIT License**. You can use it in private and commercial projects.  
Keep in mind that you must include a copy of the license in your project.
