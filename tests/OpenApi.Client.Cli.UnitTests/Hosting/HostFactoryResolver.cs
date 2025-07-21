// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System;
using System.Reflection;

namespace OpenApi.Client.Cli.UnitTests.Hosting;

internal static class HostFactoryResolver
{
    /// <summary>
    /// Runs the specified program with the provided arguments.
    /// </summary>
    public static async Task<string> Run<TProgram>(params string[] args)
    {
        Assembly entryAssembly = typeof(TProgram).Assembly;

        MethodInfo? entryPoint = entryAssembly.EntryPoint;

        if (entryPoint is null)
        {
            throw new InvalidOperationException(
                $"The entry point for the assembly '{entryAssembly.FullName}' is not defined."
            );
        }

        Type? entryType = entryPoint.DeclaringType;

        MethodInfo? asyncMainMethod = entryType?.GetMethod(
            "<Main>$",
            BindingFlags.NonPublic | BindingFlags.Static
        );

        await using StringWriter consoleOutput = new();
        Console.SetOut(consoleOutput);

        if (asyncMainMethod is null)
        {
            entryPoint.Invoke(null, [args]);
        }
        else
        {
            Task task = (Task)asyncMainMethod.Invoke(null, [args])!;
            await task;
        }

        string output = consoleOutput.ToString();

        return output;
    }
}
