// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.Mcp;

internal static partial class Log
{
    private enum Event
    {
        FailedToFetchOpenApi = 042137,
        FatalErrorWhileFetchingOpenApi,
    }

    [LoggerMessage(
        EventId = (int)Event.FailedToFetchOpenApi,
        EventName = nameof(Event.FailedToFetchOpenApi),
        Level = LogLevel.Warning,
        Message = "Failed to fetch OpenAPI document from \"{Url}\". Please check the URL and your network connection."
    )]
    public static partial void LogFetchingFailed(this ILogger logger, string url);

    [LoggerMessage(
        EventId = (int)Event.FatalErrorWhileFetchingOpenApi,
        EventName = nameof(Event.FatalErrorWhileFetchingOpenApi),
        Level = LogLevel.Error,
        Message = "Fatal error while fetching OpenAPI document from \"{Url}\". Please check the URL and your network connection."
    )]
    public static partial void LogFetchingFailedWithError(
        this ILogger logger,
        Exception exception,
        string url
    );
}
