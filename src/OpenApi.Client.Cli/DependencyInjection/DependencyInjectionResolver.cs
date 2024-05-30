// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace OpenApi.Client.Cli.DependencyInjection;

internal sealed class DependencyInjectionResolver(IHost host) : ITypeResolver, IDisposable
{
    public object? Resolve(Type? type)
    {
        return type != null ? host.Services.GetService(type) : null;
    }

    public void Dispose()
    {
        host.Dispose();
    }
}
