// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Converters;

namespace OpenApi.Client.SourceGenerators.UnitTests;

public sealed class PascalCaseConverterTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("hello", "Hello")]
    [InlineData("Hello", "Hello")]
    [InlineData("hello world", "HelloWorld")]
    [InlineData("hello_world", "HelloWorld")]
    [InlineData("hello@world", "Helloworld")]
    public void Convert_ShouldReturnExpectedResult(string? input, string expectedResult)
    {
        string result = PascalCaseConverter.Convert(input);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Convert_ShouldRemoveNonAlphanumericCharacters()
    {
        string input = "hello@world#123";

        string result = PascalCaseConverter.Convert(input);

        result.Should().Be("Helloworld123");
    }

    [Fact]
    public void Convert_ShouldConvertToAscii()
    {
        string input = "héllo";

        string result = PascalCaseConverter.Convert(input);

        result.Should().Be("Hllo");
    }
}
