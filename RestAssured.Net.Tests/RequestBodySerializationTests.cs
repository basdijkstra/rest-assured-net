// <copyright file="RequestBodySerializationTests.cs" company="On Test Automation">
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
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class RequestBodySerializationTests : TestBase
    {
        private readonly string expectedSerializedRequestBody = "{\"Country\":\"United States\",\"State\":\"California\",\"ZipCode\":90210,\"Places\":[{\"Name\":\"Sun City\",\"Inhabitants\":100000,\"IsCapital\":true},{\"Name\":\"Pleasure Meadow\",\"Inhabitants\":50000,\"IsCapital\":false}]}";

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for serializing
        /// and sending a JSON request body when performing an HTTP POST.
        /// </summary>
        [Test]
        public void ObjectCanBeSerializedToJson()
        {
            this.CreateStubForJsonRequestBody();

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

            Given()
            .Body(location)
            .When()
            .Post("http://localhost:9876/json-serialization")
            .Then()
            .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for deserializing
        /// a JSON response into an object when performing an HTTP GET.
        /// </summary>
        [Test]
        public void ObjectCanBeDeserializedFromJson()
        {
            this.CreateStubForJsonResponseBody();

            Location responseLocation = (Location)Given()
            .When()
            .Get("http://localhost:9876/json-deserialization")
            .As(typeof(Location));

            Assert.That(responseLocation.Country, Is.EqualTo("United States"));
            Assert.That(responseLocation.Places.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Creates the stub response for the JSON string request body example.
        /// </summary>
        private void CreateStubForJsonRequestBody()
        {
            this.Server.Given(Request.Create().WithPath("/json-serialization").UsingPost()
                .WithBody(new JsonMatcher(this.expectedSerializedRequestBody)))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
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

            this.Server.Given(Request.Create().WithPath("/json-deserialization").UsingGet())
                .RespondWith(Response.Create()
                .WithBodyAsJson(location)
                .WithStatusCode(200));
        }
    }
}