// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Converters;

namespace OpenApi.Client.SourceGenerators.UnitTests.Converters;

public sealed class CamelCaseConverterTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("hello", "hello")]
    [InlineData("Hello", "hello")]
    [InlineData("hello world", "helloWorld")]
    [InlineData("hello_world", "helloWorld")]
    [InlineData("hello@world", "helloworld")]
    public void Convert_ShouldReturnExpectedResult(string? input, string expectedResult)
    {
        string result = CamelCaseConverter.Convert(input);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Convert_ShouldRemoveNonAlphanumericCharacters()
    {
        string input = "hello@world#123";

        string result = CamelCaseConverter.Convert(input);

        result.Should().Be("helloworld123");
    }

    [Fact]
    public void Convert_ShouldConvertToAscii()
    {
        string input = "héllo";

        string result = CamelCaseConverter.Convert(input);

        result.Should().Be("hllo");
    }
}
