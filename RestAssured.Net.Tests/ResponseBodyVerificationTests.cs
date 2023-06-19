// <copyright file="ResponseBodyVerificationTests.cs" company="On Test Automation">
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
    using NUnit.Framework;
    using RestAssured.Response.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class ResponseBodyVerificationTests : TestBase
    {
        private readonly string plaintextResponseBody = "Here's a plaintext response body.";

        private readonly string jsonStringResponseBody = "{\"id\": 1, \"user\": \"John Doe\"}";

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a plaintext response body.
        /// </summary>
        [Test]
        public void PlaintextResponseBodyCanBeVerified()
        {
            this.CreateStubForPlaintextResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/plaintext-response-body")
                .Then()
                .StatusCode(200)
                .Body(this.plaintextResponseBody);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON string response body.
        /// </summary>
        [Test]
        public void JsonStringResponseBodyCanBeVerified()
        {
            this.CreateStubForJsonStringResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-string-response-body")
                .Then()
                .StatusCode(200)
                .Body(this.jsonStringResponseBody);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON string response body using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonStringResponseBodyCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForJsonStringResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-string-response-body")
                .Then()
                .StatusCode(200)
                .Body(NHamcrest.Contains.String("John Doe"));
        }

        /// <summary>
        /// Verifies that the correct exception is thrown when the response body
        /// does not equal the expected value.
        /// </summary>
        [Test]
        public void ActualBodyNotEqualToExpectedThrowsTheExpectedException()
        {
            this.CreateStubForPlaintextResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/plaintext-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("This is a different plaintext response body.");
            });

            Assert.That(rve?.Message, Is.EqualTo("Actual response body did not match expected response body.\nExpected: 'This is a different plaintext response body.'\nActual: 'Here's a plaintext response body.'"));
        }

        /// <summary>
        /// Verifies that the correct exception is thrown when the response body
        /// does not match the given NHamcrest matcher.
        /// </summary>
        [Test]
        public void ActualBodyNotMatchingNHamcrestMatcherThrowsTheExpectedException()
        {
            this.CreateStubForJsonStringResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/json-string-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body(NHamcrest.Contains.String("Jane Doe"));
            });

            Assert.That(rve?.Message, Is.EqualTo($"Actual response body expected to match 'a string containing \"Jane Doe\"' but didn't.\nActual: '{this.jsonStringResponseBody}'"));
        }

        /// <summary>
        /// Verifies that the correct exception is thrown when the response body
        /// does not match the given NHamcrest matcher.
        /// </summary>
        [Test]
        public void CannotHandleResponseContentTypeThrowsTheExpectedException()
        {
            this.CreateStubForUnknownContentType();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/content-type-cannot-be-processed")
                    .Then()
                    .StatusCode(200)
                    .Body("$.Places[0].Name", NHamcrest.Contains.String("City"));
            });

            Assert.That(rve?.Message, Is.EqualTo($"Unable to extract elements from response with Content-Type 'application/unknown'"));
        }

        private void CreateStubForPlaintextResponseBody()
        {
            this.Server?.Given(Request.Create().WithPath("/plaintext-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithBody(this.plaintextResponseBody)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON string response body example.
        /// </summary>
        private void CreateStubForJsonStringResponseBody()
        {
            this.Server?.Given(Request.Create().WithPath("/json-string-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithBody(this.jsonStringResponseBody)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the example with a Content-Type that cannot be processed.
        /// </summary>
        private void CreateStubForUnknownContentType()
        {
            this.Server?.Given(Request.Create().WithPath("/content-type-cannot-be-processed").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/unknown")
                .WithBody(this.jsonStringResponseBody)
                .WithStatusCode(200));
        }
    }
}