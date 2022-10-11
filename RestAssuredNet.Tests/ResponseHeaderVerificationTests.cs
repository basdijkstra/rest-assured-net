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
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class ResponseHeaderVerificationTests : TestBase
    {
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
            .Get("http://localhost:9876/custom-response-header")
            .Then()
            .StatusCode(200)
            .And() // Example of using the And() syntactic sugar method in response verification.
            .Header("custom_header_name", "custom_header_value");
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for sending
        /// a plaintext request body when performing an HTTP POST.
        /// </summary>
        [Test]
        public void NotFoundHeaderThrowsTheExpectedException()
        {
            this.CreateStubForCustomSingleResponseHeader();

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>

            {
                Given()
                .When()
                .Get("http://localhost:9876/custom-response-header")
                .Then()
                .StatusCode(200)
                .Header("header_does_not_exist", "custom_header_value");
            });

            Assert.That(ae.Message, Is.EqualTo("Expected header with name 'header_does_not_exist' to be in the response, but it could not be found."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for sending
        /// a plaintext request body when performing an HTTP POST.
        /// </summary>
        [Test]
        public void HeaderWithIncorrectValueThrowsTheExpectedException()
        {
            this.CreateStubForCustomSingleResponseHeader();

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>

            {
                Given()
                .When()
                .Get("http://localhost:9876/custom-response-header")
                .Then()
                .StatusCode(200)
                .Header("custom_header_name", "value_does_not_match");
            });

            Assert.That(ae.Message, Is.EqualTo("Expected value for header with name 'custom_header_name' to be 'value_does_not_match', but was 'custom_header_value'."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a single response header and its value.
        /// </summary>
        [Test]
        public void MultipleResponseHeadersCanBeVerified()
        {
            this.CreateStubForCustomMultipleResponseHeaders();

            Given()
            .When()
            .Get("http://localhost:9876/custom-multiple-response-headers")
            .Then()
            .StatusCode(200)
            .Header("custom_header_name", "custom_header_value")
            .Header("another_header", "another_value");
        }

        /// <summary>
        /// Creates the stub response for the JSON string request body example.
        /// </summary>
        private void CreateStubForCustomSingleResponseHeader()
        {
            this.Server.Given(Request.Create().WithPath("/custom-response-header").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("custom_header_name", "custom_header_value")
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON string request body example.
        /// </summary>
        private void CreateStubForCustomMultipleResponseHeaders()
        {
            this.Server.Given(Request.Create().WithPath("/custom-multiple-response-headers").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("custom_header_name", "custom_header_value")
                .WithHeader("another_header", "another_value")
                .WithStatusCode(200));
        }
    }
}