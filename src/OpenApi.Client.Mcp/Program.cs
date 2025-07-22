// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

#if DEBUG
builder
    .Services.AddOpenTelemetry()
    .WithTracing(b => b.AddSource("*").AddAspNetCoreInstrumentation().AddHttpClientInstrumentation())
    .WithMetrics(b => b.AddMeter("*").AddAspNetCoreInstrumentation().AddHttpClientInstrumentation())
    .WithLogging()
    .UseOtlpExporter();
#endif

builder.Services.AddTransient<IOpenApiService, OpenApiService>();

builder.Services.AddHttpClient();

ServerOptions serverOptions = new();
IConfigurationSection section = builder.Configuration.GetSection("Server");
section.Bind(serverOptions);

string? mode = Environment.GetEnvironmentVariable("mode") ?? Environment.GetEnvironmentVariable("MODE");

if (mode?.Contains("both", StringComparison.InvariantCultureIgnoreCase) ?? false)
{
    serverOptions.Mode = McpMode.Both;
}
else if (mode?.Contains("http", StringComparison.InvariantCultureIgnoreCase) ?? false)
{
    serverOptions.Mode = McpMode.Http;
}

IMcpServerBuilder mcpBuilder = builder.Services.AddMcpServer();

if (serverOptions.Mode == McpMode.Both)
{
    _ = mcpBuilder.WithHttpTransport().WithStdioServerTransport();
}
else if (serverOptions.Mode == McpMode.Stdio)
{
    _ = mcpBuilder.WithStdioServerTransport();
}
else
{
    _ = mcpBuilder.WithHttpTransport();
}

_ = mcpBuilder.WithTools<OpenApiTools>();

await using WebApplication app = builder.Build();

if (serverOptions.Mode is McpMode.Http or McpMode.Both)
{
    app.MapMcp();
}

await app.RunAsync();

return;
