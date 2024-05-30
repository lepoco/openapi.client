// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Contracts;

namespace OpenApi.Client.SourceGenerators.Generation;

internal sealed partial class ClientGenerator
{
    private const string SerializerInterfaceTemplate = """
            /// <summary>Represents a JSON serializer for the API classes.</summary>
            [global::System.CodeDom.Compiler.GeneratedCode("OpenApiClient", "%VERSION%")]
            public interface IOpenApiJsonSerializer
            {
                /// <summary>Converts the provided value to JSON text.</summary>
                string Serialize<TInput>(TInput input);

                /// <summary>Reads one JSON value (including objects or arrays).</summary>
                TOutput Deserialize<TOutput>(string input);
            }

        """;

    private const string SystemTextJsonSerializerTemplate = """
            /// <summary>Represents a JSON serializer for the API classes.</summary>
            [global::System.CodeDom.Compiler.GeneratedCode("OpenApiClient", "%VERSION%")]
            internal sealed class OpenApiJsonSerializer : IOpenApiJsonSerializer
            {
                /// <summary>Options for the serializer used to create API objects.</summary>
                private readonly global::System.Text.Json.JsonSerializerOptions JsonSettings;
                
                public OpenApiJsonSerializer()
                {
                    JsonSettings = new global::System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters =
                        {
                            new global::System.Text.Json.Serialization.JsonStringEnumConverter()
                        }
                    };
                }
                
                public OpenApiJsonSerializer(global::System.Text.Json.JsonSerializerOptions jsonSettings)
                {
                    JsonSettings = jsonSettings;
                }

                /// <inheritdoc/>
                public string Serialize<TInput>(TInput input)
                {
                    return global::System.Text.Json.JsonSerializer.Serialize(input, JsonSettings);
                }

                /// <inheritdoc/>
                public TOutput Deserialize<TOutput>(string input)
                {
                    return global::System.Text.Json.JsonSerializer.Deserialize<TOutput>(input, JsonSettings);
                }
            }

        """;

    private const string NewtonsoftJsonSerializerTemplate = """
            /// <summary>Represents a JSON serializer for the API classes.</summary>
            [global::System.CodeDom.Compiler.GeneratedCode("OpenApiClient", "%VERSION%")]
            internal sealed class OpenApiJsonSerializer : IOpenApiJsonSerializer
            {
                private readonly global::Newtonsoft.Json.JsonSerializerSettings JsonSettings;
                
                public OpenApiJsonSerializer()
                {
                    JsonSettings = new global::Newtonsoft.Json.JsonSerializerSettings
                    {
                        Converters =
                        {
                            new global::Newtonsoft.Json.Converters.StringEnumConverter()
                        }
                    };
                }
                
                public OpenApiJsonSerializer(global::Newtonsoft.Json.JsonSerializerSettings jsonSettings)
                {
                    JsonSettings = jsonSettings;
                }
                
                /// <inheritdoc/>
                public string Serialize<TInput>(TInput input)
                {
                    return global::Newtonsoft.Json.JsonConvert.SerializeObject(input);
                }

                /// <inheritdoc/>
                public TOutput Deserialize<TOutput>(string input)
                {
                    return global::Newtonsoft.Json.JsonConvert.DeserializeObject(input);
                }
            }

        """;

    public static void AppendSerializer(
        StringBuilder builder,
        OpenApiContract contract,
        ClientGeneratorSerializer serializer
    )
    {
        builder.AppendLine(SerializerInterfaceTemplate);
        builder.AppendLine(
            serializer == ClientGeneratorSerializer.SystemTextJson
                ? SystemTextJsonSerializerTemplate
                : NewtonsoftJsonSerializerTemplate
        );
    }
}
