// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.Mcp.Logging;

internal static class StaticLoggerFactory
{
    public static ILogger New(string category = "OpenAPI.Toolkit") =>
        LoggerFactory
            .Create(loggerBuilder =>
            {
                loggerBuilder.AddConsole(consoleLogOptions =>
                {
                    // Configure all logs to go to stderr
                    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
                });
                loggerBuilder.SetMinimumLevel(LogLevel.Trace);
            })
            .CreateLogger(category);
}
