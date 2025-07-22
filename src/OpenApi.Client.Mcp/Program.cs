// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

ILogger logger = StaticLoggerFactory.New();
ServerOptions serverOptions = ConfigurationFactory.CreateServerOptions(args);

logger.LogInformation("Configuring MCP Server with mode: {Mode}", serverOptions.Mode);

// NOTE: Do not use whole ASP.NET stack with just interactive console
if (serverOptions.Mode == McpMode.Stdio)
{
    logger.LogInformation("Adding Stdio transport to MCP Server.");

    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
    AddMcpServices(builder, serverOptions.Mode).WithStdioServerTransport();

    logger.LogInformation("Starting the application.");

    await builder.Build().RunAsync();

    // NOTE: Stop here, as we are running in Stdio mode.
    return;
}

logger.LogInformation("Adding HTTP transport to MCP Server.");

WebApplicationBuilder webBuilder = WebApplication.CreateBuilder(args);
IMcpServerBuilder mcpBuilder = AddMcpServices(webBuilder, serverOptions.Mode).WithHttpTransport();

if (serverOptions.Mode == McpMode.Both)
{
    logger.LogInformation("Adding Stdio transport to MCP Server.");
    _ = mcpBuilder.WithStdioServerTransport();
}

#if !DEBUG
webBuilder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
    options.ListenAnyIP(8080);
    options.ListenAnyIP(8000);
});
#endif

await using WebApplication app = webBuilder.Build();

app.MapMcp();
app.MapHealthChecks("/healthz");

await app.RunAsync();

return;

static IMcpServerBuilder AddMcpServices(IHostApplicationBuilder builder, McpMode mode)
{
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

    builder
        .Services.AddHttpClient<IOpenApiService, OpenApiService>()
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                AllowAutoRedirect = true,
            };
        });

    if (mode != McpMode.Stdio)
    {
        builder.Services.AddHealthChecks();
    }

    IMcpServerBuilder mcpBuilder = builder
        .Services.AddMcpServer(mcp =>
        {
            mcp.ServerInfo = new Implementation { Name = "OpenAPI Toolkit MCP Server", Version = "1.0.0" };
            mcp.InitializationTimeout = TimeSpan.FromHours(1);
            mcp.ScopeRequests = true;
        })
        .WithTools<OpenApiTools>();

    return mcpBuilder;
}
