// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

global using Microsoft.AspNetCore.Builder;
global using Microsoft.CodeAnalysis;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using OpenApi.Client.Mcp.Configuration;
global using OpenApi.Client.Mcp.Services;
global using OpenApi.Client.Mcp.Tools;
global using OpenTelemetry;
global using OpenTelemetry.Metrics;
global using OpenTelemetry.Trace;
global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.IO;
global using System.Linq;
global using System.Net.Http;
global using System.Threading;
global using System.Threading.Tasks;
