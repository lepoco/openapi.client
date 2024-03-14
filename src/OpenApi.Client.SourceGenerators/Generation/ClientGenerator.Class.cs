// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

using OpenApi.Client.SourceGenerators.Contracts;

namespace OpenApi.Client.SourceGenerators.Genertion;

internal sealed partial class ClientGenerator
{
    public const string ClassHeader = """
            %A partial class %C : I%C
            {
                private global::System.Net.HttpStatusCode? lastStatusCode;

                /// <summary>The HTTP Client used for the Open API Client.</summary>
                protected readonly global::System.Net.Http.HttpClient HttpClient;

                /// <summary>Options for the serializer used to create API objects.</summary>
                protected readonly global::System.Text.Json.JsonSerializerOptions JsonSettings;

                public %C(global::System.Net.Http.HttpClient httpClient)
                    : this(httpClient, new global::System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters =
                        {
                            new global::System.Text.Json.Serialization.JsonStringEnumConverter()
                        }
                    })
                {
                }

                protected %C(global::System.Net.Http.HttpClient httpClient, global::System.Text.Json.JsonSerializerOptions jsonSettings)
                {
                    HttpClient = httpClient;
                    JsonSettings = jsonSettings;
                }

        """;

    public const string ClassFooter = """
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

                    return global::System.Text.Json.JsonSerializer.Serialize(input, JsonSettings);
                }

                /// <summary>Tries to serialize body returned by HTTP request.</summary>
                protected virtual TResponse? DeserializeResponse<TResponse>(string? input) where TResponse : class
                {
                    if (input == null)
                    {
                        return default;
                    }

                    return global::System.Text.Json.JsonSerializer.Deserialize<TResponse>(input, JsonSettings);
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

                            queryString.Append(global::System.Uri.EscapeDataString(property.Name));
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

    public static void AppendClass(StringBuilder builder, OpenApiContract contract)
    {
        builder.AppendLine(ClassHeader);
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
                "        public virtual async global::System.Threading.Tasks.Task<%CResult"
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

                            try
                            {
                                responseBody = await ExecuteRequestAsync(global::System.Net.Http.HttpMethod.Get, {{{(
                    "\""
                    + path.Path
                    + "\""
                    + (path.RequestQueryType?.Length > 0 ? " + ComputeQueryString(query)" : null)
                )}}}, {{{(
                    path.RequestBodyType?.Length > 0 ? "SerializeRequest(request)" : "null"
                )}}}, cancellationToken);
                            }
                            catch (global::System.Exception e)
                            {
                                return new %CResult(new %CResultError[]{new %CResultError(e.Message)}, lastStatusCode);
                            }

                            return new %CResult(lastStatusCode);
                        }
                """
            );

            methodsCount++;
        }

        builder.AppendLine(ClassFooter);
    }
}
