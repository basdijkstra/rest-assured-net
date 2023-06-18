// <copyright file="ResponseBodyJsonVerificationTests.cs" company="On Test Automation">
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
    using RestAssured.Response;
    using RestAssured.Response.Exceptions;
    using RestAssured.Tests.Models;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class ResponseBodyJsonVerificationTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body string element using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementStringValueCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForJsonResponseBody();

            Given()
                .When()
                .Get(MOCK_SERVER_BASE_URL + "/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].Name", NHamcrest.Contains.String("City"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that an NHamcrest string matcher mismatch throws the expected exception.
        /// </summary>
        [Test]
        public void StringElementNHamcrestMatcherMismatchThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get(MOCK_SERVER_BASE_URL + "/json-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("$.Places[0].Name", NHamcrest.Contains.String("Sin"));
            });

            Assert.That(rve?.Message, Is.EqualTo("Expected element selected by '$.Places[0].Name' to match 'a string containing \"Sin\"' but was 'Sun City'"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body integer element using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementIntegerValueCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForJsonResponseBody();

            Given()
                .When()
                .Get(MOCK_SERVER_BASE_URL + "/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].Inhabitants", NHamcrest.Is.GreaterThan(75000));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that an NHamcrest integer matcher mismatch throws the expected exception.
        /// </summary>
        [Test]
        public void IntegerElementNHamcrestMatcherMismatchThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get(MOCK_SERVER_BASE_URL + "/json-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("$.Places[0].Inhabitants", NHamcrest.Is.GreaterThan(200000));
            });

            Assert.That(rve?.Message, Is.EqualTo("Expected element selected by '$.Places[0].Inhabitants' to match 'greater than 200000' but was '100000'"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body boolean element using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementBooleanValueCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForJsonResponseBody();

            Given()
                .When()
                .Get(MOCK_SERVER_BASE_URL + "/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].IsCapital", NHamcrest.Is.True());
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body string element using an NHamcrest matcher
        /// overriding the response Content-Type header value.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementStringValueCanBeVerifiedOverridingContentTypeHeaderValue()
        {
            this.CreateStubForJsonResponseBodyWithNonMatchingContentTypeHeader();

            Given()
                .When()
                .Get(MOCK_SERVER_BASE_URL + "/json-response-body-header-mismatch")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].Name", NHamcrest.Contains.String("City"), VerifyAs.Json);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that an NHamcrest boolean matcher mismatch throws the expected exception.
        /// </summary>
        [Test]
        public void BooleanElementNHamcrestMatcherMismatchThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get(MOCK_SERVER_BASE_URL + "/json-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("$.Places[0].IsCapital", NHamcrest.Is.False());
            });

            Assert.That(rve?.Message, Is.EqualTo("Expected element selected by '$.Places[0].IsCapital' to match 'False' but was 'True'"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the expected exception is thrown when the JsonPath does not return results.
        /// </summary>
        [Test]
        public void NoJsonPathResultsThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get(MOCK_SERVER_BASE_URL + "/json-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("$.Places[0].DoesNotExist", NHamcrest.Is.False());
            });

            Assert.That(rve?.Message, Is.EqualTo("JsonPath expression '$.Places[0].DoesNotExist' did not yield any results."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementCollectionCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForJsonResponseBody();

            Given()
                .When()
                .Get(MOCK_SERVER_BASE_URL + "/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0:].Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo("Sun City")));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementCollectionCanBeVerifiedUsingNHamcrestMatcherOverridingResponseContentTypeHeader()
        {
            this.CreateStubForJsonResponseBodyWithNonMatchingContentTypeHeader();

            Given()
                .When()
                .Get(MOCK_SERVER_BASE_URL + "/json-response-body-header-mismatch")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0:].Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo("Sun City")), VerifyAs.Json);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementCollectionNHamcrestMatcherMisMatchThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get(MOCK_SERVER_BASE_URL + "/json-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("$.Places[0:].Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo("Atlantis")));
            });

            Assert.That(rve?.Message, Is.EqualTo($"Expected elements selected by '$.Places[0:].Name' to match 'a collection containing \"Atlantis\"', but was [Sun City, Pleasure Meadow]"));
        }

        /// <summary>
        /// Creates the stub response for the JSON response body example.
        /// </summary>
        private void CreateStubForJsonResponseBody()
        {
            this.Server?.Given(Request.Create().WithPath("/json-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(this.GetLocation())
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON response body example with a non-matching
        /// response Content-Type header value.
        /// </summary>
        private void CreateStubForJsonResponseBodyWithNonMatchingContentTypeHeader()
        {
            this.Server?.Given(Request.Create().WithPath("/json-response-body-header-mismatch").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "text/plain")
                .WithBodyAsJson(this.GetLocation())
                .WithStatusCode(200));
        }
    }
}