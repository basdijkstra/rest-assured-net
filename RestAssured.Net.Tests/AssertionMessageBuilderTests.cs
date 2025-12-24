// <copyright file="AssertionMessageBuilderTests.cs" company="On Test Automation">
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
    using RestAssured.Response.Logging;

    /// <summary>
    /// Tests for the <see cref="AssertionMessageBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class AssertionMessageBuilderTests
    {
        /// <summary>
        /// Test cases for the BuildMessage method in the <see cref="AssertionMessageBuilder>"/> class.
        /// </summary>
        /// <param name="originalMessage">The original error message.</param>
        /// <param name="expectedValue">The expected value to insert in the error message.</param>
        /// <param name="actualValue">The actual value to insert in the error message.</param>
        /// <param name="expectedResultMessage">The expected result after error message processing.</param>
        [TestCaseSource(nameof(ErrorMessageTestCases))]
        public void ErrorMessageProcessingShouldYieldExpectedResult(string originalMessage, object expectedValue, object actualValue, string expectedResultMessage)
        {
            string resultMessage = AssertionMessageBuilder.BuildMessage(originalMessage, expectedValue, actualValue);

            Assert.That(resultMessage, Is.EqualTo(expectedResultMessage));
        }

        private static IEnumerable<TestCaseData> ErrorMessageTestCases()
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