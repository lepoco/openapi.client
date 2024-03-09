// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and OpenAPI Client Contributors.
// All Rights Reserved.

namespace OpenApi.Client.SourceGenerators.Genertion;

internal sealed partial class ClientGenerator
{
    private const string ResultTemplate = """
            /// <summary>Represents a result of the API call.</summary>
            {{ContractAccess}} struct {{ContractClassName}}ResultError
            {
                /// <summary>Initializes a new instance of the error struct.</summary>
                public {{ContractClassName}}ResultError(string message)
                {
                    Message = message;
                }

                /// <summary>Gets the error message.</summary>
                public string? Message { get; }
            }

            /// <summary>Represents a result of the API call.</summary>
            {{ContractAccess}} class {{ContractClassName}}Result
            {
                /// <summary>Initializes a new instance of the class.</summary>
                /// <param name="errors">The errors of the API call.</param>
                /// <param name="statusCode">The status code of the API call.</param>
                public {{ContractClassName}}Result({{ContractClassName}}ResultError[] errors, global::System.Net.HttpStatusCode? statusCode)
                {
                    Errors = errors;
                    StatusCode = statusCode;
                }

                /// <summary>Initializes a new instance of the result class.</summary>
                /// <param name="statusCode">The errors of the API call.</param>
                public {{ContractClassName}}Result(global::System.Net.HttpStatusCode? statusCode) : this(new {{ContractClassName}}ResultError[0], statusCode)
                {
                }

                /// <summary>Initializes a new instance of the result class.</summary>
                /// <param name="errors">The errors of the API call.</param>
                public {{ContractClassName}}Result({{ContractClassName}}ResultError[] errors) : this(errors, null)
                {
                }

                /// <summary>Gets the status code of the API call.</summary>
                public global::System.Net.HttpStatusCode? StatusCode { get; }

                /// <summary>Gets the errors of the API call.</summary>
                public {{ContractClassName}}ResultError[] Errors { get; }

                /// <summary>Gets a value indicating whether the API call has errors.</summary>
                public bool HasErrors => Errors.Length > 0;

                /// <summary>Gets a value indicating whether the API call is successful.</summary>
                public bool IsSucceed => !HasErrors;
            }

            /// <summary>Represents a result from the API.</summary>
            {{ContractAccess}} sealed class {{ContractClassName}}Result<TResult> : {{ContractClassName}}Result
            {
                private readonly TResult? _result;

                /// <summary>Initializes a new instance of the result class.</summary>
                /// <param name="result">Result of the API call.</param>
                /// <param name="statusCode">The status code of the API call.</param>
                public {{ContractClassName}}Result(TResult result, global::System.Net.HttpStatusCode? statusCode) : base(statusCode)
                {
                    _result = result;
                }

                /// <summary>Initializes a new instance of the result class.</summary>
                /// <param name="errors">The errors of the API call.</param>
                /// <param name="statusCode">The errors of the API call.</param>
                public {{ContractClassName}}Result({{ContractClassName}}ResultError[] errors, global::System.Net.HttpStatusCode? statusCode) : base(errors, statusCode)
                {
                }

                /// <summary>Gets the result of the API call.</summary>
                public TResult Result => _result ?? throw new global::System.InvalidOperationException("Cannot retrieve API result if errors exist.");
            }
        """;
}
