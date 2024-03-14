// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using Microsoft.CodeAnalysis;
using OpenApi.Client.SourceGenerators.Contracts;
using OpenApi.Client.SourceGenerators.Schema;
using OpenApi.Client.SourceGenerators.Serialization;

namespace OpenApi.Client.SourceGenerators.Roslyn431.UnitTests.Contracts;

public sealed class OpenApiContractParserTests
{
    [Fact]
    public void Parse_ShouldReturnOpenApiContractWithExpectedValues()
    {
        IApiDocument document = Substitute.For<IApiDocument>();
        document.GetTitle().Returns("TestTitle");
        document.GetDescription().Returns("TestDescription");
        document.GetLicense().Returns("TestLicense");
        document.GetVersion().Returns("TestVersion");

        OpenApiContract result = OpenApiContractParser.Parse(
            "TestNamespace",
            "TestClass",
            Accessibility.Public,
            document
        );

        result.Namespace.Should().Be("TestNamespace");
        result.ClassName.Should().Be("TestClass");
        result.Access.Should().Be("public");
        result.Title.Should().Be("TestTitle");
        result.Description.Should().Be("TestDescription");
        result.License.Should().Be("TestLicense");
        result.Version.Should().Be("TestVersion");
        result.Paths.Should().BeEmpty();
        result.Types.Should().BeEmpty();
    }

    [Fact]
    public void Parse_ShouldReturnOpenApiContractWithPaths()
    {
        string jsonString = File.ReadAllText("OpenApis/openapi-3.0.1.extended.json");
        OpenApiSerializer serializer = new();

        IApiDocument apiDocument = serializer.Deserialize("test-name", jsonString).Result!;

        OpenApiContract result = OpenApiContractParser.Parse(
            "TestNamespace",
            "TestClass",
            Accessibility.Public,
            apiDocument
        );

        result.Paths.Should().HaveCount(2);
    }

    [Fact]
    public void Parse_ShouldReturnOpenApiContractWithTypes()
    {
        string jsonString = File.ReadAllText("OpenApis/openapi-3.0.1.extended.json");
        OpenApiSerializer serializer = new();

        IApiDocument apiDocument = serializer.Deserialize("test-name", jsonString).Result!;

        OpenApiContract result = OpenApiContractParser.Parse(
            "TestNamespace",
            "TestClass",
            Accessibility.Public,
            apiDocument
        );

        result.Types.Should().HaveCount(3);
    }
}
