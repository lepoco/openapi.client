// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Serialization.Converters;

namespace OpenApi.Client.SourceGenerators.Serialization.Attributes;

public sealed class NumberToStringConverterAttribute : JsonConverterAttribute
{
    public override JsonConverter CreateConverter(Type typeToConvert)
    {
        return new NumberToStringConverter();
    }
}
