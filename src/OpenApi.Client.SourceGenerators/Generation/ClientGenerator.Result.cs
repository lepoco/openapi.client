// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Generation;

internal sealed partial class ClientGenerator
{
    public const string ResultTemplate = """
            /// <summary>Represents a result of the API call.</summary>
            [global::System.CodeDom.Compiler.GeneratedCode("OpenApiClient", "%VERSION%")]
            %ACCESS% struct %CLASS%ResultError
            {
                /// <summary>Initializes a new instance of the error struct.</summary>
                public %CLASS%ResultError(string message)
                {
                    Message = message;
                }

                /// <summary>Gets the error message.</summary>
                public string? Message { get; }
            }
            
            /// <summary>Represents a result of the API call.</summary>
            [global::System.CodeDom.Compiler.GeneratedCode("OpenApiClient", "%VERSION%")]
            %ACCESS% interface I%CLASS%Result
            {
                /// <summary>Gets a value indicating whether the API call has errors.</summary>
                bool HasErrors { get; }
                
                /// <summary>Gets a value indicating whether the API call succeeded.</summary>
                bool IsSucceeded{ get; }
            }

            /// <summary>Represents a result of the API call.</summary>
            [global::System.CodeDom.Compiler.GeneratedCode("OpenApiClient", "%VERSION%")]
            %ACCESS% class %CLASS%Result : I%CLASS%Result
            {
                /// <summary>Initializes a new instance of the class.</summary>
                /// <param name="errors">The errors of the API call.</param>
                /// <param name="statusCode">The status code of the API call.</param>
                public %CLASS%Result(%CLASS%ResultError[] errors, global::System.Net.HttpStatusCode? statusCode)
                {
                    Errors = errors;
                    StatusCode = statusCode;
                }

                /// <summary>Initializes a new instance of the result class.</summary>
                /// <param name="statusCode">The status code of the API call.</param>
                public %CLASS%Result(global::System.Net.HttpStatusCode? statusCode) : this(new %CLASS%ResultError[0], statusCode)
                {
                }

                /// <summary>Gets the status code of the API call.</summary>
                public global::System.Net.HttpStatusCode? StatusCode { get; }

                /// <summary>Gets the errors of the API call.</summary>
                public %CLASS%ResultError[] Errors { get; }

                /// <inheritdoc/>
                public bool HasErrors
                {
                    get
                    {
                        return Errors.Length > 0;
                    }
                }
                
                /// <inheritdoc/>
                public bool IsSucceeded
                {
                    get
                    {
                        return !HasErrors;
                    }
                }
            }

            /// <summary>Represents a result from the API.</summary>
            [global::System.CodeDom.Compiler.GeneratedCode("OpenApiClient", "%VERSION%")]
            %ACCESS% sealed class %CLASS%Result<TResult> : %CLASS%Result where TResult : class
            {
                private readonly TResult? _result;

                /// <summary>Initializes a new instance of the result class.</summary>
                /// <param name="result">Result of the API call.</param>
                /// <param name="statusCode">The status code of the API call.</param>
                public %CLASS%Result(TResult result, global::System.Net.HttpStatusCode? statusCode) : base(statusCode)
                {
                    if (result == null)
                    {
                        throw new global::System.ArgumentNullException(nameof(result));
                    }

                    _result = result;
                }

                /// <summary>Initializes a new instance of the result class.</summary>
                /// <param name="errors">The errors of the API call.</param>
                /// <param name="statusCode">The errors of the API call.</param>
                public %CLASS%Result(%CLASS%ResultError[] errors, global::System.Net.HttpStatusCode? statusCode) : base(errors, statusCode)
                {
                }

                /// <summary>Gets the result of the API call.</summary>
                public TResult Result
                {
                    get
                    {
                        return _result ?? throw new global::System.InvalidOperationException("Cannot retrieve API result if errors exist.");
                    }
                }
            }
        """;
}
