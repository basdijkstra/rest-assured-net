// <copyright file="RequestHeaderTests.cs" company="On Test Automation">
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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class RequestHeaderTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a header with a single value when sending an HTTP request.
        /// </summary>
        [Test]
        public void HeaderWithASingleValueCanBeSupplied()
        {
            this.CreateStubForSingleHeaderValue();

            Given()
                .Header("my_header", "my_header_value")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/single-header-value")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// multiple headers when sending an HTTP request.
        /// </summary>
        [Test]
        public void MultipleHeadersCanBeSuppliedSeparately()
        {
            this.CreateStubForMultipleHeaders();

            Given()
                .Header("header_one", "header_one_value")
                .Header("header_two", "header_two_value")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/multiple-headers")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// multiple headers as a Dictionary when sending an HTTP request.
        /// </summary>
        [Test]
        public void MultipleHeadersCanBeSuppliedAsADictionary()
        {
            this.CreateStubForMultipleHeaders();

            var headers = new Dictionary<string, object>
            {
                { "header_one", "header_one_value" },
                { "header_two", "header_two_value" },
            };

            Given()
                .Headers(headers)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/multiple-headers")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a header with multiple values when sending an HTTP request.
        /// </summary>
        [Test]
        public void HeaderWithMultipleValuesCanBeSupplied()
        {
            this.CreateStubForMultipleHeaderValues();

            Given()
                .Header("my_header", new List<string>() { "my_header_value_1", "my_header_value_2" })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/multiple-header-values")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a header with an unrecognized value when sending an HTTP request.
        /// </summary>
        [Test]
        public void AddingHeaderWithUnrecognizedValue_WhenValidationIsSkipped_ShouldBeAddedToRequest()
        {
            this.CreateStubForUnrecognizedHeaderValue();

            string datetime = "2025-06-01T09:46:14.698+02:00";

            Given()
                .Header("If-Modified-Since", datetime, validate: false)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/unrecognized-header-value")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a header with an unrecognized value when sending an HTTP request.
        /// </summary>
        [Test]
        public void AddingHeaderWithUnrecognizedValue_WhenValidated_ShouldThrowExpectedException()
        {
            this.CreateStubForUnrecognizedHeaderValue();

            string datetime = "2025-06-01T09:46:14.698+02:00";

            var fe = Assert.Throws<FormatException>(() =>
            {
                Given()
                .Header("If-Modified-Since", datetime, validate: true)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/unrecognized-header-value");
            });

            Assert.That(fe.Message, Is.EqualTo($"The format of value '{datetime}' is invalid."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for setting
        /// a Content-Type header with a string value.
        /// </summary>
        [Test]
        public void ContentTypeHeaderCanBeSuppliedAsString()
        {
            this.CreateStubForContentTypeHeaderAsString();

            Given()
                .ContentType("application/xml")
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/content-type-as-string")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for setting
        /// the content character encoding.
        /// </summary>
        [Test]
        public void EncodingCanBeSupplied()
        {
            this.CreateStubForEncoding();

            Given()
                .ContentType("application/xml")
                .And() // Example of using the And() syntactic sugar method in request building.
                .ContentEncoding(Encoding.ASCII)
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/content-type-with-encoding")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for setting
        /// the Accept header.
        /// </summary>
        [Test]
        public void AcceptHeaderCanBeSuppliedAsString()
        {
            this.CreateStubForAcceptHeaderAsString();

            Given()
                .Accept("application/xml")
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/accept-header-as-string")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// Creates the stub response for the single header value example.
        /// </summary>
        private void CreateStubForSingleHeaderValue()
        {
            this.Server?.Given(Request.Create().WithPath("/single-header-value").UsingGet()
                .WithHeader("my_header", "my_header_value"))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the multiple header values example.
        /// </summary>
        private void CreateStubForMultipleHeaderValues()
        {
            this.Server?.Given(Request.Create().WithPath("/multiple-header-values").UsingGet()
                .WithHeader("my_header", "my_header_value_1, my_header_value_2"))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the multiple header example.
        /// </summary>
        private void CreateStubForMultipleHeaders()
        {
            this.Server?.Given(Request.Create().WithPath("/multiple-headers").UsingGet()
                .WithHeader("header_one", "header_one_value")
                .WithHeader("header_two", "header_two_value"))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the unrecognized header value example.
        /// </summary>
        private void CreateStubForUnrecognizedHeaderValue()
        {
            this.Server?.Given(Request.Create().WithPath("/unrecognized-header-value").UsingGet()
                .WithHeader("If-Modified-Since", "2025-06-01T09:46:14.698+02:00"))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the example setting the content type as a string.
        /// </summary>
        private void CreateStubForContentTypeHeaderAsString()
        {
            this.Server?.Given(Request.Create().WithPath("/content-type-as-string").UsingPost()
                .WithHeader("Content-Type", new ExactMatcher("application/xml; charset=utf-8")))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }

        /// <summary>
        /// Creates the stub response for the example setting the content encoding.
        /// </summary>
        private void CreateStubForEncoding()
        {
            this.Server?.Given(Request.Create().WithPath("/content-type-with-encoding").UsingPost()
                .WithHeader("Content-Type", new ExactMatcher("application/xml; charset=us-ascii")))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }

        /// <summary>
        /// Creates the stub response for the example setting the accept header as a string.
        /// </summary>
        private void CreateStubForAcceptHeaderAsString()
        {
            this.Server?.Given(Request.Create().WithPath("/accept-header-as-string").UsingPost()
                .WithHeader("Accept", new ExactMatcher("application/xml")))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }
    }
}