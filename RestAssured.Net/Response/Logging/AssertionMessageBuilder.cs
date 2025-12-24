// <copyright file="AssertionMessageBuilder.cs" company="On Test Automation">
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
namespace RestAssured.Response.Logging
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Stubble.Core;
    using Stubble.Core.Builders;
    using Stubble.Core.Classes;

    /// <summary>
    /// Contains helper methods for logging assertion results.
    /// </summary>
    internal class AssertionMessageBuilder
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
        /// The template value to insert in a templated assertion error message for the expected value.
        /// </summary>
        internal const string TEMPLATE_EXPECTED_VALUE = "expected";

        /// <summary>
        /// The template value to insert in a templated assertion error message for the actual value.
        /// </summary>
        internal const string TEMPLATE_ACTUAL_VALUE = "actual";

        /// <summary>
        /// Builds an error message based on the original assertion error message and values to insert.
        /// </summary>
        /// <param name="originalMessage">The original error message.</param>
        /// <param name="expectedValue">The expected assertion value to insert in the message.</param>
        /// <param name="actualValue">The actual assertion value to insert in the message.</param>
        /// <returns>The updated error message.</returns>
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

            return renderer.Render(originalMessage, values);
        }
    }
}
