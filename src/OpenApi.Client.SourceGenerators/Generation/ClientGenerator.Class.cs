// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Contracts;

namespace OpenApi.Client.SourceGenerators.Generation;

internal sealed partial class ClientGenerator
{
    private const string SystemTextJsonClassHeader = """
            [global::System.CodeDom.Compiler.GeneratedCode("OpenApiClient", "%VERSION%")]
            %ACCESS% partial class %CLASS% : I%CLASS%
            {
                private global::System.Net.HttpStatusCode? lastStatusCode;

                /// <summary>The JSON serializer used for the Open API Client.</summary>
                protected IOpenApiJsonSerializer serializer;

                /// <summary>The HTTP Client used for the Open API Client.</summary>
                protected global::System.Net.Http.HttpClient HttpClient;

                /// <summary>Initializes a new instance of the <see cref="OpenApiTest"/> class using the provided HttpClient. Uses a default instance of <see cref="IOpenApiJsonSerializer"/> for serialization.</summary>
                public %CLASS%(global::System.Net.Http.HttpClient httpClient)
                {
                    HttpClient = httpClient;
                    serializer = new OpenApiJsonSerializer();
                }

                /// <summary>Initializes a new instance of the <see cref="OpenApiTest"/> class using the provided HttpClient and JsonSerializerOptions. Uses an instance of <see cref="IOpenApiJsonSerializer"/> configured with the provided <see cref="System.Text.Json.JsonSerializerOptions"/> for serialization.</summary>
                public %CLASS%(global::System.Net.Http.HttpClient httpClient, global::System.Text.Json.JsonSerializerOptions jsonSettings)
                {
                    HttpClient = httpClient;
                    serializer = new OpenApiJsonSerializer(jsonSettings);
                }

                /// <summary>Initializes a new instance of the <see cref="OpenApiTest"/> class using the provided HttpClient and IOpenApiJsonSerializer.</summary>
                public %CLASS%(global::System.Net.Http.HttpClient httpClient, IOpenApiJsonSerializer serializer)
                {
                    HttpClient = httpClient;
                    serializer = serializer;
                }

        """;

    private const string NewtonsoftJsonClassHeader = """
            [global::System.CodeDom.Compiler.GeneratedCode("OpenApiClient", "%VERSION%")]
            %ACCESS% partial class %CLASS% : I%CLASS%
            {
                private global::System.Net.HttpStatusCode? lastStatusCode;

                private OpenApiJsonSerializer serializer;

                /// <summary>The HTTP Client used for the Open API Client.</summary>
                protected readonly global::System.Net.Http.HttpClient HttpClient;

                /// <summary>Initializes a new instance of the <see cref="OpenApiTest"/> class using the provided HttpClient. Uses a default instance of <see cref="IOpenApiJsonSerializer"/> for serialization.</summary>
                public %CLASS%(global::System.Net.Http.HttpClient httpClient)
                {
                    HttpClient = httpClient;
                    serializer = new OpenApiJsonSerializer();
                }

                /// <summary>Initializes a new instance of the <see cref="OpenApiTest"/> class using the provided HttpClient and JsonSerializerOptions. Uses an instance of <see cref="IOpenApiJsonSerializer"/> configured with the provided <see cref="Newtonsoft.Json.JsonSerializerSettings"/> for serialization.</summary>
                public %CLASS%(global::System.Net.Http.HttpClient httpClient, global::Newtonsoft.Json.JsonSerializerSettings jsonSettings)
                {
                    HttpClient = httpClient;
                    serializer = new OpenApiJsonSerializer(jsonSettings);
                }

                /// <summary>Initializes a new instance of the <see cref="OpenApiTest"/> class using the provided HttpClient and IOpenApiJsonSerializer.</summary>
                public %CLASS%(global::System.Net.Http.HttpClient httpClient, IOpenApiJsonSerializer serializer)
                {
                    HttpClient = httpClient;
                    serializer = serializer;
                }

        """;

    private const string ClassFooter = """
                /// <inheritdoc/>
                public virtual global::System.Net.HttpStatusCode? GetLastStatusCode()
                {
                    return lastStatusCode;
                }

                /// <summary>Sets the last status code.</summary>
                protected virtual void SetStatusCode(global::System.Net.HttpStatusCode? statusCode)
                {
                    lastStatusCode = statusCode;
                }

                /// <summary>Resets the last status code.</summary>
                protected virtual void ResetStatusCode()
                {
                    lastStatusCode = null;
                }

                /// <summary>Executes the HTTP request.</summary>
                protected virtual async global::System.Threading.Tasks.Task<string?> ExecuteRequestAsync(global::System.Net.Http.HttpMethod method, string path, string? body, global::System.Threading.CancellationToken cancellationToken)
                {
                    lastStatusCode = null;
                    global::System.Net.Http.HttpRequestMessage requestMessage = new global::System.Net.Http.HttpRequestMessage(method, new global::System.Uri(path, global::System.UriKind.RelativeOrAbsolute));

                    if (body != null)
                    {
                        requestMessage.Content = new global::System.Net.Http.StringContent(body, global::System.Text.Encoding.UTF8, "application/json");
                    }

                    using (global::System.Net.Http.HttpResponseMessage response = await HttpClient.SendAsync(requestMessage, cancellationToken))
                    {
                        lastStatusCode = response.StatusCode;

                        return await response.Content.ReadAsStringAsync();
                    }
                }

                /// <summary>Tries to serialize body returned by HTTP request.</summary>
                protected virtual string? SerializeRequest<TRequest>(TRequest? input) where TRequest : class
                {
                    if (input == null)
                    {
                        return null;
                    }

                    return serializer.Serialize(input);
                }

                /// <summary>Tries to serialize body returned by HTTP request.</summary>
                protected virtual TResponse? DeserializeResponse<TResponse>(string? input) where TResponse : class
                {
                    if (input == null)
                    {
                        return default;
                    }

                    return serializer.Deserialize<TResponse>(input);
                }

                /// <summary>Computes path string for HTTP request.</summary>
                protected virtual string? ComputePath(string? path, params string[] parameters)
                {
                    if (parameters.Length == 0)
                    {
                        return path;
                    }

                    return string.Format(path, parameters);
                }

                /// <summary>Computes query string for HTTP request.</summary>
                protected virtual string? ComputeQueryString<TQuery>(TQuery? input) where TQuery : class
                {
                    if (input == null)
                    {
                        return null;
                    }

                    global::System.Reflection.PropertyInfo[] properties = input.GetType().GetProperties(global::System.Reflection.BindingFlags.Public | global::System.Reflection.BindingFlags.Instance);
                    global::System.Text.StringBuilder queryString = new global::System.Text.StringBuilder();

                    foreach (global::System.Reflection.PropertyInfo? property in properties)
                    {
                        string? value = property.GetValue(input)?.ToString().Trim();

                        if (value.Length > 0)
                        {
                            if (queryString.Length > 0)
                            {
                                queryString.Append("&");
                            }

                            OpenApiPropertyAttribute? attribute = global::System.Reflection.CustomAttributeExtensions.GetCustomAttribute<OpenApiPropertyAttribute>(property);
                            string name = attribute?.Name ?? property.Name;

                            queryString.Append(global::System.Uri.EscapeDataString(name));
                            queryString.Append('=');
                            queryString.Append(global::System.Uri.EscapeDataString(value));
                        }
                    }

                    if (queryString.Length > 0)
                    {
                        queryString.Insert(0, '?');
                    }

                    return queryString.ToString();
                }
            }

        """;

    private static void AppendClass(
        StringBuilder builder,
        OpenApiContract contract,
        ClientGeneratorSerializer serializer
    )
    {
        builder.AppendLine(
            serializer == ClientGeneratorSerializer.SystemTextJson
                ? SystemTextJsonClassHeader
                : NewtonsoftJsonClassHeader
        );
        int methodsCount = 0;

        foreach (OpenApiPath path in contract.Paths)
        {
            if (methodsCount > 0)
            {
                builder.AppendLine();
            }

            if (path.Summary?.Length > 0)
            {
                builder.AppendLine("        /// <inheritdoc/>");
            }

            builder.Append(
                "        public virtual async global::System.Threading.Tasks.Task<%CLASS%Result"
            );

            if (path.ResponseType?.Length > 0)
            {
                builder.Append("<");
                builder.Append(path.ResponseType);
                builder.Append(">");
            }

            builder.Append("> ");
            builder.Append(path.Name);
            builder.Append("(");

            if (path.RequestBodyType?.Length > 0)
            {
                builder.Append(path.RequestBodyType);
                builder.Append(" request, ");
            }

            if (path.RequestQueryType?.Length > 0)
            {
                builder.Append(path.RequestQueryType);
                builder.Append(" query, ");
            }

            builder.AppendLine("global::System.Threading.CancellationToken cancellationToken)");
            builder.AppendLine(
                $$$"""
                        {
                            string? responseBody = null;
                            string[] pathParameters = new string[0];

                            try
                            {
                                responseBody = await ExecuteRequestAsync(
                                    global::System.Net.Http.HttpMethod.Get,
                                    ComputePath({{{(
                    "\""
                    + path.Path
                    + "\""
                    + (path.RequestQueryType?.Length > 0 ? " + ComputeQueryString(query)" : null)
                )}}}, pathParameters),
                                    {{{(
                    path.RequestBodyType?.Length > 0 ? "SerializeRequest(request)" : "null"
                )}}},
                                    cancellationToken
                                );
                            }
                            catch (global::System.Exception e)
                            {
                                return new %CLASS%Result(new %CLASS%ResultError[]{new %CLASS%ResultError(e.Message)}, lastStatusCode);
                            }

                            return new %CLASS%Result(lastStatusCode);
                        }

                """
            );

            methodsCount++;
        }

        builder.AppendLine(ClassFooter);
    }
}
