// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace OpenApi.Client.Cli.DependencyInjection;

internal sealed class DependencyInjectionRegistrar(IHostBuilder builder) : ITypeRegistrar
{
    public ITypeResolver Build()
    {
        return new DependencyInjectionResolver(builder.Build());
    }

    public void Register(Type service, Type implementation)
    {
        builder.ConfigureServices((_, services) => services.AddSingleton(service, implementation));
    }

    public void RegisterInstance(Type service, object implementation)
    {
        builder.ConfigureServices((_, services) => services.AddSingleton(service, implementation));
    }

    public void RegisterLazy(Type service, Func<object> func)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        builder.ConfigureServices((_, services) => services.AddSingleton(service, _ => func()));
    }
}
