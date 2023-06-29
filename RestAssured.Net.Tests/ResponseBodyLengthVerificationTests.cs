// <copyright file="ResponseBodyLengthVerificationTests.cs" company="On Test Automation">
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
    using NUnit.Framework;
    using RestAssured.Response.Exceptions;
    using RestAssured.Tests.Models;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class ResponseBodyLengthVerificationTests : TestBase
    {
        private Location location = new Location();
        private Place place = new Place();
        private string country;
        private string state;
        private string placeName;
        private int zipcode;
        private int placeInhabitants;
        private bool isCapital;

        /// <summary>
        /// Create the response body payload.
        /// </summary>
        [SetUp]
        public void SetLocation()
        {
            this.country = Faker.Country.Name();
            this.state = Faker.Address.UsState();
            this.zipcode = Faker.RandomNumber.Next(1000, 99999);

            this.location.Country = this.country;
            this.location.State = this.state;
            this.location.ZipCode = this.zipcode;

            this.placeName = Faker.Address.City();
            this.placeInhabitants = Faker.RandomNumber.Next(100010, 199990);
            this.isCapital = Faker.Boolean.Random();

            this.place.Name = this.placeName;
            this.place.Inhabitants = this.placeInhabitants;
            this.place.IsCapital = this.isCapital;

            this.location.Places.Add(this.place);
            this.location.Places.Add(new Place());
        }

        /// <summary>
        /// Clean up the response body payload.
        /// </summary>
        [TearDown]
        public void ClearPlaces()
        {
            this.location.Places = new List<Place>();
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// the response body length for a response.
        /// </summary>
        [Test]
        public void ResponseBodyLengthCanBeVerified()
        {
            this.CreateStubForJsonResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .Log(RestAssured.Response.Logging.ResponseLogLevel.All)
                .StatusCode(200)
                .ResponseBodyLength(NHamcrest.Is.GreaterThan(25));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// the response time for a response.
        /// </summary>
        [Test]
        public void FailingResponseBodyLengthVerificationThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .StatusCode(200)
                .ResponseBodyLength(NHamcrest.Is.LessThan(25));
            });

            Assert.That(rve?.Message, Does.Contain("Expected response body length to match 'less than 25' but was '"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting
        /// the response body and verifying the length for a response.
        /// </summary>
        [Test]
        public void ResponseBodyCanBeExtracted()
        {
            this.CreateStubForJsonResponseBody();

            string responseBodyAsString = Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .StatusCode(200)
                .Extract().Body();

            Assert.That(responseBodyAsString.Length, Is.GreaterThan(25));
        }

        /// <summary>
        /// Creates the stub response for the JSON response body extraction examples.
        /// </summary>
        private void CreateStubForJsonResponseBody()
        {
            this.Server?.Given(Request.Create().WithPath("/json-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(this.location)
                .WithStatusCode(200));
        }
    }
}