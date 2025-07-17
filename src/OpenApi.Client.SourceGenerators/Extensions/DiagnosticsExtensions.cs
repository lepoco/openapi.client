// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file is ported and adapted from ComputeSharp (Sergio0694/ComputeSharp),
// more info in ThirdPartyNotices.txt in the root of the project.

using Microsoft.CodeAnalysis;
using OpenApi.Client.SourceGenerators.Collections;
using OpenApi.Client.SourceGenerators.Diagnostics;

namespace OpenApi.Client.SourceGenerators.Extensions;

internal static class DiagnosticsExtensions
{
    /// <summary>
    /// Adds a new diagnostics to the target builder.
    /// </summary>
    /// <param name="diagnostics">The collection of produced <see cref="Microsoft.CodeAnalysis.DiagnosticInfo"/> instances.</param>
    /// <param name="descriptor">The input <see cref="DiagnosticDescriptor"/> for the diagnostics to create.</param>
    /// <param name="symbol">The source <see cref="ISymbol"/> to attach the diagnostics to.</param>
    /// <param name="args">The optional arguments for the formatted message to include.</param>
    public static void Add(
        this in ImmutableArrayBuilder<DiagnosticInfo> diagnostics,
        DiagnosticDescriptor descriptor,
        ISymbol symbol,
        params object[] args
    )
    {
        diagnostics.Add(DiagnosticInfo.Create(descriptor, symbol, args));
    }

    /// <summary>
    /// Adds a new diagnostics to the target builder.
    /// </summary>
    /// <param name="diagnostics">The collection of produced <see cref="Microsoft.CodeAnalysis.DiagnosticInfo"/> instances.</param>
    /// <param name="descriptor">The input <see cref="DiagnosticDescriptor"/> for the diagnostics to create.</param>
    /// <param name="node">The source <see cref="SyntaxNode"/> to attach the diagnostics to.</param>
    /// <param name="args">The optional arguments for the formatted message to include.</param>
    public static void Add(
        this in ImmutableArrayBuilder<DiagnosticInfo> diagnostics,
        DiagnosticDescriptor descriptor,
        SyntaxNode node,
        params object[] args
    )
    {
        diagnostics.Add(DiagnosticInfo.Create(descriptor, node, args));
    }

    /// <summary>
    /// Registers an output node into an <see cref="IncrementalGeneratorInitializationContext"/> to output diagnostics.
    /// </summary>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> instance.</param>
    /// <param name="diagnostics">The input <see cref="IncrementalValuesProvider{TValues}"/> sequence of diagnostics.</param>
    public static void ReportDiagnostics(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<DiagnosticInfo> diagnostics
    )
    {
        context.RegisterSourceOutput(
            diagnostics,
            static (context, diagnostic) =>
            {
                context.ReportDiagnostic(diagnostic.ToDiagnostic());
            }
        );
    }

    /// <summary>
    /// Registers an output node into an <see cref="IncrementalGeneratorInitializationContext"/> to output diagnostics.
    /// </summary>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> instance.</param>
    /// <param name="diagnostics">The input <see cref="IncrementalValuesProvider{TValues}"/> sequence of diagnostics.</param>
    public static void ReportDiagnostics(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<EquatableArray<DiagnosticInfo>> diagnostics
    )
    {
        context.RegisterSourceOutput(
            diagnostics,
            static (context, diagnostics) =>
            {
                foreach (DiagnosticInfo diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic.ToDiagnostic());
                }
            }
        );
    }
}
