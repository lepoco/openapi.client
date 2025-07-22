// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.Mcp.Configuration;

internal static class ConfigurationFactory
{
    public static ServerOptions CreateServerOptions(string[] args)
    {
        IConfigurationRoot configuration = CreateConfiguration(args);

        string? mode =
            configuration["Mode"]
            ?? configuration["MODE"]
            ?? configuration["mode"]
            ?? configuration["Server:Mode"];

        return new ServerOptions { Mode = ParseMode(mode) };
    }

    private static IConfigurationRoot CreateConfiguration(string[] args)
    {
        string environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Production";

        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        return builder.Build();
    }

    private static McpMode ParseMode(string? mode)
    {
        if (mode is null)
        {
            return McpMode.Stdio;
        }

        if (mode.Contains("both", StringComparison.InvariantCultureIgnoreCase))
        {
            return McpMode.Both;
        }

        if (mode.Contains("http", StringComparison.InvariantCultureIgnoreCase))
        {
            return McpMode.Http;
        }

        return McpMode.Stdio;
    }
}
