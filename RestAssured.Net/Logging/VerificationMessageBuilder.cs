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
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using RestAssured.Response;

    /// <summary>
    /// Contains helper methods for building and resolving verification failure messages.
    /// </summary>
    internal class VerificationMessageBuilder
    {
        /// <summary>
        /// The template value to insert in a templated error message for the expected value.
        /// </summary>
        internal const string TEMPLATE_EXPECTED_VALUE = "expected";

        /// <summary>
        /// The template value to insert in a templated error message for the actual value.
        /// </summary>
        internal const string TEMPLATE_ACTUAL_VALUE = "actual";

        /// <summary>
        /// Matches the template values [expected] and [actual], optionally surrounded by whitespace within the brackets.
        /// </summary>
        private static readonly Regex TemplateValue = new Regex(
            $@"\[\s*({TEMPLATE_EXPECTED_VALUE}|{TEMPLATE_ACTUAL_VALUE})\s*\]",
            RegexOptions.Compiled);

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
            // Only the template values [expected] and [actual] are replaced,
            // all other text (including any other text between square brackets) is left unchanged.
            return TemplateValue.Replace(
                originalMessage,
                match => FormatValue(match.Groups[1].Value == TEMPLATE_EXPECTED_VALUE ? expectedValue : actualValue));
        }

        /// <summary>
        /// Formats a value for insertion into an error message, independent of the current culture.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The formatted value, or an empty string if the value is <c>null</c>.</returns>
        private static string FormatValue(object? value)
        {
            // Stryker disable once String: Equivalent mutant - Convert.ToString only returns null when the ToString() of the value returns null
            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}
