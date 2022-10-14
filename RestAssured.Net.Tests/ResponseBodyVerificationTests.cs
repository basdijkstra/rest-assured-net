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
using NHamcrest;
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

            Assert.That(ae.Message, NUnit.Framework.Is.EqualTo("Actual response body did not match expected response body.\nExpected: This is a different plaintext response body.\nActual: Here's a plaintext response body."));
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

            Assert.That(ae.Message, NUnit.Framework.Is.EqualTo($"Actual response body expected to match 'a string containing \"Jane Doe\"' but didn't.\nActual: {this.jsonStringResponseBody}"));
        }

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
            .Get("http://localhost:9876/json-response-body")
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

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            {
                Given()
                .When()
                .Get("http://localhost:9876/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].Name", NHamcrest.Contains.String("Sin"));
            });

            Assert.That(ae.Message, NUnit.Framework.Is.EqualTo("Expected element selected by '$.Places[0].Name' to match 'a string containing \"Sin\"' but was Sun City"));
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
            .Get("http://localhost:9876/json-response-body")
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

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            {
                Given()
                .When()
                .Get("http://localhost:9876/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].Inhabitants", NHamcrest.Is.GreaterThan(200000));
            });

            Assert.That(ae.Message, NUnit.Framework.Is.EqualTo("Expected element selected by '$.Places[0].Inhabitants' to match 'greater than 200000' but was 100000"));
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
            .Get("http://localhost:9876/json-response-body")
            .Then()
            .StatusCode(200)
            .Body("$.Places[0].IsCapital", NHamcrest.Is.True());
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that an NHamcrest boolean matcher mismatch throws the expected exception.
        /// </summary>
        [Test]
        public void BooleanElementNHamcrestMatcherMismatchThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            {
                Given()
                .When()
                .Get("http://localhost:9876/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].IsCapital", NHamcrest.Is.False());
            });

            Assert.That(ae.Message, NUnit.Framework.Is.EqualTo("Expected element selected by '$.Places[0].IsCapital' to match 'False' but was True"));
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
            .Get("http://localhost:9876/json-response-body")
            .Then()
            .StatusCode(200)
            .Body("$.Places[0:].Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo("Sun City")));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementCollectionNHamcrestMatcherMisMatchThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            { 
                Given()
                .When()
                .Get("http://localhost:9876/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0:].Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo("Atlantis")));
            });

            Assert.That(ae.Message, NUnit.Framework.Is.EqualTo($"Expected elements selected by '$.Places[0:].Name' to match 'a collection containing \"Atlantis\"', but was [Sun City, Pleasure Meadow]"));
        }

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
                IsCapital = true,
            };

            Place secondPlace = new Place
            {
                Name = "Pleasure Meadow",
                Inhabitants = 50000,
                IsCapital = false,
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