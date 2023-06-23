// <copyright file="ResponseJsonValueExtractionTests.cs" company="On Test Automation">
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
    using System.Linq;
    using System.Net;
    using System.Net.Http;
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
    public class ResponseJsonValueExtractionTests : TestBase
    {
        private Location location = new Location();
        private Place place = new Place();
        private string country;
        private string state;
        private string placeName;
        private int zipcode;
        private int placeInhabitants;
        private bool isCapital;

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

        [TearDown]
        public void ClearPlaces()
        {
            this.location.Places = new List<Place>();
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// string element value from a JSON response body.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementStringValueCanBeExtracted()
        {
            this.CreateStubForJsonResponseWithBodyAndHeaders();

            string placeName = (string)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .StatusCode(200)
                .Extract().Body("$.Places[0].Name");

            Assert.That(placeName, Is.EqualTo(this.placeName));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// integer element value from a JSON response body.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementIntegerValueCanBeExtracted()
        {
            this.CreateStubForJsonResponseWithBodyAndHeaders();

            // For now, you'll need to store number values in an object of
            // type 'long', because Json.NET by default deserializes numbers
            // into Int64 (=long), not Int32 (= int).
            long numberOfInhabitants = (long)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .StatusCode(200)
                .Extract().Body("$.Places[0].Inhabitants");

            int numberOfInhabitantsInt = Convert.ToInt32(numberOfInhabitants);

            Assert.That(numberOfInhabitantsInt, Is.EqualTo(this.placeInhabitants));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// boolean element value from a JSON response body.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementBooleanValueCanBeExtracted()
        {
            this.CreateStubForJsonResponseWithBodyAndHeaders();

            bool isCapitalResponse = (bool)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .StatusCode(200)
                .Extract().Body("$.Places[0].IsCapital");

            Assert.That(isCapitalResponse, Is.EqualTo(this.isCapital));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// list of element values from a JSON response body.
        /// </summary>
        [Test]
        public void JsonResponseBodyMultipleElementsCanBeExtracted()
        {
            this.CreateStubForJsonResponseWithBodyAndHeaders();

            // At least for now, if you want to retrieve multiple
            // JSON response body objects, they will have to be
            // stored in an object of type List<object>.
            List<object> placeNames = (List<object>)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .StatusCode(200)
                .Extract().Body("$.Places[0:].IsCapital");

            Assert.That(placeNames.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// string element value from a JSON response body, overriding
        /// the default extraction method (determined using the response
        /// Content-Type header).
        /// </summary>
        [Test]
        public void JsonResponseBodyElementStringValueCanBeExtractedOverridingResponseContentType()
        {
            this.CreateStubForJsonResponseWithResponseContentTypeHeaderMismatch();

            string placeName = (string)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body-header-mismatch")
                .Then()
                .StatusCode(200)
                .Extract().Body("$.Places[0].Name", ExtractAs.Json);

            Assert.That(placeName, Is.EqualTo(this.placeName));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the expected exception is thrown when the JsonPath does not return results.
        /// </summary>
        [Test]
        public void NoJsonPathResultsThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseWithBodyAndHeaders();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                    .Then()
                    .StatusCode(200)
                    .Extract().Body("$.Places[0].DoesNotExist");
            });

            Assert.That(rve?.Message, Is.EqualTo("JsonPath expression '$.Places[0].DoesNotExist' did not yield any results."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// response header value.
        /// </summary>
        [Test]
        public void ResponseHeaderCanBeExtracted()
        {
            this.CreateStubForJsonResponseWithBodyAndHeaders();

            string responseHeaderValue = Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .StatusCode(200)
                .Extract().Header("custom_header");

            Assert.That(responseHeaderValue, Is.EqualTo("custom_header_value"));
        }

        /// <summary>
        /// A test for verifying that the expected exception is thrown
        /// when the header value to be extracted does not exist in the response.
        /// </summary>
        [Test]
        public void HeaderNotFoundInResponseThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseWithBodyAndHeaders();

            var ee = Assert.Throws<ExtractionException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                    .Then()
                    .StatusCode(200)
                    .Extract().Header("does_not_exist");
            });

            Assert.That(ee?.Message, Is.EqualTo("Header with name 'does_not_exist' could not be found in the response."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting
        /// the entire response as an HttpResponseMessage object.
        /// </summary>
        [Test]
        public void EntireResponseCanBeExtracted()
        {
            this.CreateStubForJsonResponseWithBodyAndHeaders();

            HttpResponseMessage response = Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .StatusCode(200)
                .Extract().Response();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Headers.GetValues("custom_header").First(), Is.EqualTo("custom_header_value"));
        }

        /// <summary>
        /// Creates the stub response for the JSON response body extraction examples.
        /// </summary>
        private void CreateStubForJsonResponseWithBodyAndHeaders()
        {
            this.Server?.Given(Request.Create().WithPath("/json-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("custom_header", "custom_header_value")
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(this.location)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON response body extraction examples.
        /// </summary>
        private void CreateStubForJsonResponseWithResponseContentTypeHeaderMismatch()
        {
            this.Server?.Given(Request.Create().WithPath("/json-response-body-header-mismatch").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("custom_header", "custom_header_value")
                .WithHeader("Content-Type", "text/plain")
                .WithBodyAsJson(this.location)
                .WithStatusCode(200));
        }
    }
}