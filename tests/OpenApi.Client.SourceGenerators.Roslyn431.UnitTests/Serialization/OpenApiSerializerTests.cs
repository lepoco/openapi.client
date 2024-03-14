// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using System.Linq;
using OpenApi.Client.SourceGenerators.Schema;
using OpenApi.Client.SourceGenerators.Serialization;

namespace OpenApi.Client.SourceGenerators.Roslyn431.UnitTests.Serialization;

public sealed class OpenApiSerializerTests
{
    [Theory]
    [InlineData("OpenApis/openapi-1.2.0.json", "1.3.0")]
    [InlineData("OpenApis/openapi-2.0.0.json", "v2")]
    [InlineData("OpenApis/openapi-3.0.3.json", "2.0.0")]
    [InlineData("OpenApis/openapi-3.1.0.json", "1.5.0")]
    public void Deserialize_ShouldReturnApiVersion(string jsonFile, string expectedVersion)
    {
        string jsonString = File.ReadAllText(jsonFile);
        OpenApiSerializer serializer = new();

        SerializationResult<IApiDocument> serializationResult = serializer.Deserialize(
            "test-name",
            jsonString
        );

        serializationResult.HasErrors.Should().BeFalse();
        serializationResult.Result?.GetVersion().Should().BeEquivalentTo(expectedVersion);
    }

    [Fact]
    public void Deserialize_ShouldReturnPaths()
    {
        string jsonString = File.ReadAllText("OpenApis/openapi-3.0.1.extended.json");
        OpenApiSerializer serializer = new();

        SerializationResult<IApiDocument> serializationResult = serializer.Deserialize(
            "test-name",
            jsonString
        );

        ApiDocumentPath[] paths = serializationResult.Result!.GetPaths().ToArray();

        paths.Should().HaveCount(2);
    }

    [Fact]
    public void Deserialize_ShouldReturnTypes()
    {
        string jsonString = File.ReadAllText("OpenApis/openapi-3.0.1.extended.json");
        OpenApiSerializer serializer = new();

        SerializationResult<IApiDocument> serializationResult = serializer.Deserialize(
            "test-name",
            jsonString
        );

        ApiDocumentType[] types = serializationResult.Result!.GetTypes().ToArray();

        types.Should().HaveCount(3);
    }
}
