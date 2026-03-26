// <copyright file="VerificationMessageBuilder.cs" company="On Test Automation">
// Copyright 2019 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
namespace RestAssured.Logging
{
    using System.Collections.Generic;
    using RestAssured.Response;
    using Stubble.Core;
    using Stubble.Core.Builders;
    using Stubble.Core.Classes;
    using Stubble.Core.Settings;

    /// <summary>
    /// Contains helper methods for building and resolving verification failure messages.
    /// </summary>
    internal class VerificationMessageBuilder
    {
        /// <summary>
        /// The start tag used to identify a template.
        /// </summary>
        internal const string START_TAG = "[";

        /// <summary>
        /// The end tag used to identify a template.
        /// </summary>
        internal const string END_TAG = "]";

        /// <summary>
        /// The template value to insert in a templated error message for the expected value.
        /// </summary>
        internal const string TEMPLATE_EXPECTED_VALUE = "expected";

        /// <summary>
        /// The template value to insert in a templated error message for the actual value.
        /// </summary>
        internal const string TEMPLATE_ACTUAL_VALUE = "actual";

        /// <summary>
        /// Returns the custom error message if one was supplied, otherwise returns the default message.
        /// Use this overload for "not found" failures where there are no expected/actual values to substitute.
        /// </summary>
        /// <param name="errorMessage">The optional custom error message.</param>
        /// <param name="defaultMessage">The default message to use when no custom message was supplied.</param>
        /// <returns>The resolved error message.</returns>
        internal static string Resolve(ErrorMessage errorMessage, string defaultMessage)
            => errorMessage.HasValue ? errorMessage.Value! : defaultMessage;

        /// <summary>
        /// Returns the custom error message prefixed to the default message if one was supplied, otherwise returns the default message.
        /// Use this overload for "not found" or "format error" failures where the custom message should provide additional context
        /// alongside the default message, rather than replacing it entirely.
        /// </summary>
        /// <param name="errorMessage">The optional custom error message.</param>
        /// <param name="defaultMessage">The default message to append after the custom message.</param>
        /// <returns>The resolved error message.</returns>
        internal static string ResolveWithPrefix(ErrorMessage errorMessage, string defaultMessage)
            => errorMessage.HasValue ? $"{errorMessage.Value!}: {defaultMessage}" : defaultMessage;

        /// <summary>
        /// Returns the custom error message with template substitution if one was supplied, otherwise returns the default message.
        /// Use this overload for "value mismatch" failures where <c>[expected]</c> and <c>[actual]</c> tokens can be substituted.
        /// </summary>
        /// <param name="errorMessage">The optional custom error message.</param>
        /// <param name="defaultMessage">The default message to use when no custom message was supplied.</param>
        /// <param name="expectedValue">The expected value to substitute for <c>[expected]</c> in the custom message.</param>
        /// <param name="actualValue">The actual value to substitute for <c>[actual]</c> in the custom message.</param>
        /// <returns>The resolved error message.</returns>
        internal static string Resolve(ErrorMessage errorMessage, string defaultMessage, object expectedValue, object actualValue)
            => errorMessage.HasValue ? BuildMessage(errorMessage.Value!, expectedValue, actualValue) : defaultMessage;

        /// <summary>
        /// Builds an error message by substituting <c>[expected]</c> and <c>[actual]</c> template tokens.
        /// </summary>
        /// <param name="originalMessage">The original error message template.</param>
        /// <param name="expectedValue">The expected value to substitute for <c>[expected]</c>.</param>
        /// <param name="actualValue">The actual value to substitute for <c>[actual]</c>.</param>
        /// <returns>The rendered error message.</returns>
        internal static string BuildMessage(string originalMessage, object expectedValue, object actualValue)
        {
            var values = new Dictionary<string, object>()
                {
                    { TEMPLATE_EXPECTED_VALUE, expectedValue },
                    { TEMPLATE_ACTUAL_VALUE, actualValue },
                };

            StubbleVisitorRenderer renderer = new StubbleBuilder()
                .Configure(builder => builder.SetDefaultTags(new Tags(START_TAG, END_TAG)))
                .Build();

            return renderer.Render(originalMessage, values, new RenderSettings { SkipHtmlEncoding = true });
        }
    }
}
