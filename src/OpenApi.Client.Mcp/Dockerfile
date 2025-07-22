FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY . .

RUN find . -type f -name 'appsettings.*.json' ! -name 'appsettings.json' -exec rm -f {} +
RUN find . -name 'launchSettings.json' -exec rm -f {} +

RUN dotnet publish "src/OpenApi.Client.Mcp/OpenApi.Client.Mcp.csproj" -c Release -o /app/ --nologo

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final

WORKDIR /app
COPY --from=build /app/ .

ENTRYPOINT ["dotnet", "./OpenApi.Client.Mcp.dll"]
