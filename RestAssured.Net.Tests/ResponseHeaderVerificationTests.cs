// <copyright file="ResponseHeaderVerificationTests.cs" company="On Test Automation">
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
    public class ResponseHeaderVerificationTests : TestBase
    {
        private readonly string headerName = "a_header";
        private readonly string headerValue = "a_value";

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a single response header and its value.
        /// </summary>
        [Test]
        public void SingleResponseHeaderCanBeVerified()
        {
            this.CreateStubForCustomSingleResponseHeader();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-response-header")
                .Then()
                .StatusCode(200)
                .Header(this.headerName, this.headerValue);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a single response header and whether its value matches an NHamcrest matcher.
        /// </summary>
        [Test]
        public void SingleResponseHeaderCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForCustomSingleResponseHeader();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-response-header")
                .Then()
                .StatusCode(200)
                .Header(this.headerName, NHamcrest.Is.EqualTo(this.headerValue));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the correct exception is thrown when an expected header is not found.
        /// </summary>
        [Test]
        public void NotFoundHeaderThrowsTheExpectedException()
        {
            this.CreateStubForCustomSingleResponseHeader();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/custom-response-header")
                    .Then()
                    .StatusCode(200)
                    .Header("header_does_not_exist", this.headerValue);
            });

            Assert.That(rve?.Message, Is.EqualTo("Expected header with name 'header_does_not_exist' to be in the response, but it could not be found."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the correct exception is thrown when a header does not have the expected value.
        /// </summary>
        [Test]
        public void HeaderWithIncorrectValueThrowsTheExpectedException()
        {
            this.CreateStubForCustomSingleResponseHeader();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/custom-response-header")
                    .Then()
                    .StatusCode(200)
                    .Header(this.headerName, "value_does_not_match");
            });

            Assert.That(rve?.Message, Is.EqualTo($"Expected value for response header with name '{this.headerName}' to match '\"value_does_not_match\"', but was '{this.headerValue}'."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// multiple response headers and their values.
        /// </summary>
        [Test]
        public void MultipleResponseHeadersCanBeVerified()
        {
            this.CreateStubForCustomMultipleResponseHeaders();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-multiple-response-headers")
                .Then()
                .StatusCode(200)
                .Header(this.headerName, this.headerValue)
                .Header("another_header", "another_value");
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// the response Content-Type header.
        /// </summary>
        [Test]
        public void ResponseContentTypeHeaderCanBeVerified()
        {
            this.CreateStubForCustomResponseContentTypeHeader();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-response-content-type-header")
                .Then()
                .StatusCode(200)
                .ContentType("application/something");
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// the response Content-Type header using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void ResponseContentTypeHeaderCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForCustomResponseContentTypeHeader();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-response-content-type-header")
                .Then()
                .StatusCode(200)
                .ContentType(NHamcrest.Contains.String("something"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the correct exception is thrown when the response does not have a Content-Type header.
        /// </summary>
        [Test]
        public void NoContentTypeHeaderThrowsTheExpectedException()
        {
            this.CreateStubForNoResponseContentTypeHeader();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/no-response-content-type-header")
                    .Then()
                    .StatusCode(200)
                    .ContentType("application/something");
            });

            Assert.That(rve?.Message, Is.EqualTo("Response Content-Type header could not be found."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the correct exception is thrown when the response content type does not equal the specified value.
        /// </summary>
        [Test]
        public void IncorrectContentTypeHeaderValueThrowsTheExpectedException()
        {
            this.CreateStubForCustomResponseContentTypeHeader();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/custom-response-content-type-header")
                    .Then()
                    .StatusCode(200)
                    .ContentType("application/something_else");
            });

            Assert.That(rve?.Message, Is.EqualTo("Expected value for response Content-Type header to match '\"application/something_else\"', but was 'application/something'."));
        }

        /// <summary>
        /// Creates the stub response for the single response header example.
        /// </summary>
        private void CreateStubForCustomSingleResponseHeader()
        {
            this.Server?.Given(Request.Create().WithPath("/custom-response-header").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader(this.headerName, this.headerValue)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the custom response content type example.
        /// </summary>
        private void CreateStubForCustomResponseContentTypeHeader()
        {
            this.Server?.Given(Request.Create().WithPath("/custom-response-content-type-header").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/something")
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the no response content type example.
        /// </summary>
        private void CreateStubForNoResponseContentTypeHeader()
        {
            this.Server?.Given(Request.Create().WithPath("/no-response-content-type-header").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the multiple response headers example.
        /// </summary>
        private void CreateStubForCustomMultipleResponseHeaders()
        {
            this.Server?.Given(Request.Create().WithPath("/custom-multiple-response-headers").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader(this.headerName, this.headerValue)
                .WithHeader("another_header", "another_value")
                .WithStatusCode(200));
        }
    }
}