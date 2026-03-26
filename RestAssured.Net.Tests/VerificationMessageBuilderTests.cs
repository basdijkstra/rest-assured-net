// <copyright file="VerificationMessageBuilderTests.cs" company="On Test Automation">
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
namespace RestAssured.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using RestAssured.Logging;
    using RestAssured.Response;

    /// <summary>
    /// Tests for the <see cref="VerificationMessageBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class VerificationMessageBuilderTests
    {
        /// <summary>
        /// Test cases for the <see cref="VerificationMessageBuilder.BuildMessage"/> method.
        /// </summary>
        /// <param name="originalMessage">The original error message.</param>
        /// <param name="expectedValue">The expected value to insert in the error message.</param>
        /// <param name="actualValue">The actual value to insert in the error message.</param>
        /// <param name="expectedResultMessage">The expected result after error message processing.</param>
        [TestCaseSource(nameof(BuildMessageTestCases))]
        public void BuildMessageShouldYieldExpectedResult(string originalMessage, object expectedValue, object actualValue, string expectedResultMessage)
        {
            string resultMessage = VerificationMessageBuilder.BuildMessage(originalMessage, expectedValue, actualValue);

            Assert.That(resultMessage, Is.EqualTo(expectedResultMessage));
        }

        /// <summary>
        /// Verifies that <see cref="VerificationMessageBuilder.Resolve(ErrorMessage, string)"/>
        /// returns the custom message when one is provided.
        /// </summary>
        [Test]
        public void ResolveReturnsCustomMessageWhenProvided()
        {
            ErrorMessage errorMessage = "custom message";

            string result = VerificationMessageBuilder.Resolve(errorMessage, "default message");

            Assert.That(result, Is.EqualTo("custom message"));
        }

        /// <summary>
        /// Verifies that <see cref="VerificationMessageBuilder.Resolve(ErrorMessage, string)"/>
        /// returns the default message when no custom message is provided.
        /// </summary>
        [Test]
        public void ResolveReturnsDefaultMessageWhenNoneProvided()
        {
            ErrorMessage errorMessage = default;

            string result = VerificationMessageBuilder.Resolve(errorMessage, "default message");

            Assert.That(result, Is.EqualTo("default message"));
        }

        /// <summary>
        /// Verifies that <see cref="VerificationMessageBuilder.Resolve(ErrorMessage, string, object, object)"/>
        /// returns the custom message with template tokens substituted when one is provided.
        /// </summary>
        [Test]
        public void ResolveWithTemplateReturnsSubstitutedCustomMessageWhenProvided()
        {
            ErrorMessage errorMessage = "Expected [expected] but got [actual]";

            string result = VerificationMessageBuilder.Resolve(errorMessage, "default message", 200, 404);

            Assert.That(result, Is.EqualTo("Expected 200 but got 404"));
        }

        /// <summary>
        /// Verifies that <see cref="VerificationMessageBuilder.Resolve(ErrorMessage, string, object, object)"/>
        /// returns the default message when no custom message is provided.
        /// </summary>
        [Test]
        public void ResolveWithTemplateReturnsDefaultMessageWhenNoneProvided()
        {
            ErrorMessage errorMessage = default;

            string result = VerificationMessageBuilder.Resolve(errorMessage, "default message", 200, 404);

            Assert.That(result, Is.EqualTo("default message"));
        }

        private static IEnumerable<TestCaseData> BuildMessageTestCases()
        {
            yield return new TestCaseData("Error message", null, null, "Error message")
                .SetName("Error message without template values remains unchanged");

            yield return new TestCaseData("Expected [expected] but was [actual]", 200, 404, "Expected 200 but was 404")
                .SetName("Integer literals can be inserted into error message");

            yield return new TestCaseData("Expected [expected] but was [actual]", "banana", "apple", "Expected banana but was apple")
                .SetName("String literals can be inserted into error message");

            yield return new TestCaseData("Expected [expected] but was [actual]", false, true, "Expected False but was True")
                .SetName("Boolean literals can be inserted into error message");

            yield return new TestCaseData("Expected [expected] but was [actual]", NHamcrest.Is.LessThan(300), 401, "Expected less than 300 but was 401")
                .SetName("NHamcrest matcher can be inserted into error message");
        }
    }
}
