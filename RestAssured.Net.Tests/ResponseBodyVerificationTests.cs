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
using System.Collections.Generic;
using NUnit.Framework;
using RestAssured.Net.Tests.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
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
            .Get("http://localhost:9876/plaintext-response-body")
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
            .Get("http://localhost:9876/json-string-response-body")
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
            .Get("http://localhost:9876/json-string-response-body")
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

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            {
                Given()
                .When()
                .Get("http://localhost:9876/plaintext-response-body")
                .Then()
                .StatusCode(200)
                .Body("This is a different plaintext response body.");
            });

            Assert.That(ae.Message, Is.EqualTo("Actual response body did not match expected response body.\nExpected: This is a different plaintext response body.\nActual: Here's a plaintext response body."));
        }

        /// <summary>
        /// Verifies that the correct exception is thrown when the response body
        /// does not match the given NHamcrest matcher.
        /// </summary>
        [Test]
        public void ActualBodyNotMatchingNHamcrestMatcherThrowsTheExpectedException()
        {
            this.CreateStubForJsonStringResponseBody();

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            {
                Given()
                .When()
                .Get("http://localhost:9876/json-string-response-body")
                .Then()
                .StatusCode(200)
                .Body(NHamcrest.Contains.String("Jane Doe"));
            });

            Assert.That(ae.Message, Is.EqualTo($"Actual response body expected to match 'a string containing \"Jane Doe\"' but didn't.\nActual: {this.jsonStringResponseBody}"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body element value 1-on-1.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementCanBeVerified()
        {
            this.CreateStubForJsonResponseBody();

            Given()
            .When()
            .Get("http://localhost:9876/json-response-body")
            .Then()
            .StatusCode(200)
            .Body("$.Places[0].Name", "Sun City");
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body element value 1-on-1.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementValueMismatchThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            {
                Given()
                .When()
                .Get("http://localhost:9876/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].Name", "Sin City");
            });

            Assert.That(ae.Message, Is.EqualTo($"Expected JsonPath expression '$.Places[0].Name' to yield an element with value 'Sin City', but was 'Sun City'"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body element value 1-on-1.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementNotFoundThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            {
                Given()
                .When()
                .Get("http://localhost:9876/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].NonExistingElement", "Sun City");
            });

            Assert.That(ae.Message, Is.EqualTo($"JsonPath expression '$.Places[0].NonExistingElement' did not yield any elements."));
        }

        /// <summary>
        /// Creates the stub response for the plaintext response body example.
        /// </summary>
        private void CreateStubForPlaintextResponseBody()
        {
            this.Server.Given(Request.Create().WithPath("/plaintext-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithBody(this.plaintextResponseBody)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON string response body example.
        /// </summary>
        private void CreateStubForJsonStringResponseBody()
        {
            this.Server.Given(Request.Create().WithPath("/json-string-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithBody(this.jsonStringResponseBody)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON response body example.
        /// </summary>
        private void CreateStubForJsonResponseBody()
        {
            Place firstPlace = new Place
            {
                Name = "Sun City",
                Inhabitants = 100000,
            };

            Place secondPlace = new Place
            {
                Name = "Pleasure Meadow",
                Inhabitants = 50000,
            };

            Location location = new Location
            {
                Country = "United States",
                State = "California",
                ZipCode = 90210,
                Places = new List<Place>() { firstPlace, secondPlace },
            };

            this.Server.Given(Request.Create().WithPath("/json-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithBodyAsJson(location)
                .WithStatusCode(200));
        }
    }
}