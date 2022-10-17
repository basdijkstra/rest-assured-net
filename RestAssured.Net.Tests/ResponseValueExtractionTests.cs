// <copyright file="ResponseValueExtractionTests.cs" company="On Test Automation">
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
using System;
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
    public class ResponseValueExtractionTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// string element value from a JSON response body.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementStringValueCanBeExtracted()
        {
            this.CreateStubForJsonResponseBody();

            string placeName = (string)Given()
            .When()
            .Get("http://localhost:9876/json-response-body")
            .Then()
            .StatusCode(200)
            .Extract("$.Places[0].Name");

            Assert.That(placeName, NUnit.Framework.Is.EqualTo("Sun City"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// string element value from a JSON response body.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementIntegerValueCanBeExtracted()
        {
            this.CreateStubForJsonResponseBody();

            // For now, you'll need to store number values in an object of
            // type 'long', because Json.NET by default deserializes numbers
            // into Int64 (=long), not Int32 (= int).
            long numberOfInhabitants = (long)Given()
            .When()
            .Get("http://localhost:9876/json-response-body")
            .Then()
            .StatusCode(200)
            .Extract("$.Places[0].Inhabitants");

            int numberOfInhabitantsInt = Convert.ToInt32(numberOfInhabitants);

            Assert.That(numberOfInhabitantsInt, NUnit.Framework.Is.EqualTo(100000));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// string element value from a JSON response body.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementBooleanValueCanBeExtracted()
        {
            this.CreateStubForJsonResponseBody();

            bool isCapital = (bool)Given()
            .When()
            .Get("http://localhost:9876/json-response-body")
            .Then()
            .StatusCode(200)
            .Extract("$.Places[0].IsCapital");

            Assert.That(isCapital, NUnit.Framework.Is.True);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// string element value from a JSON response body.
        /// </summary>
        [Test]
        public void JsonResponseBodyMultipleElementsCanBeExtracted()
        {
            this.CreateStubForJsonResponseBody();

            // At least for now, if you want to retrieve multiple
            // JSON response body objects, they will have to be
            // stored in an object of type List<object>.
            List<object> placeNames = (List<object>)Given()
            .When()
            .Get("http://localhost:9876/json-response-body")
            .Then()
            .StatusCode(200)
            .Extract("$.Places[0:].IsCapital");

            Assert.That(placeNames.Count, NUnit.Framework.Is.EqualTo(2));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the expected exception is thrown when the JsonPath does not return results.
        /// </summary>
        [Test]
        public void NoJsonPathResultsThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            {
                Given()
                .When()
                .Get("http://localhost:9876/json-response-body")
                .Then()
                .StatusCode(200)
                .Extract("$.Places[0].DoesNotExist");
            });

            Assert.That(ae.Message, NUnit.Framework.Is.EqualTo("JsonPath expression '$.Places[0].DoesNotExist' did not yield any results."));
        }

        /// <summary>
        /// Creates the stub response for the JSON response body extraction examples.
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