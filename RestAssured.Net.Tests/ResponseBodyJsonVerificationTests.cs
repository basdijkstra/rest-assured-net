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
    using System;
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
        private Location location = new Location();
        private Place place = new Place();
        private string country;
        private string state;
        private string placeName;
        private int zipcode;
        private int placeInhabitants;
        private bool isCapital;

        /// <summary>
        /// Initializes the Location test data object used in these tests.
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
        /// Clean up our test data afterwards.
        /// </summary>
        [TearDown]
        public void ClearPlaces()
        {
            this.location.Places = new List<Place>();
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
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].Name", NHamcrest.Core.IsEqualMatcher<string>.EqualTo(this.placeName));
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
                    .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("$.Places[0].Name", NHamcrest.Contains.String("Sin"));
            });

            Assert.That(rve?.Message, Is.EqualTo("Expected element selected by '$.Places[0].Name' to match 'a string containing \"Sin\"' but was '" + this.placeName + "'"));
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
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
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
                    .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("$.Places[0].Inhabitants", NHamcrest.Is.GreaterThan(200000));
            });

            Assert.That(rve?.Message, Is.EqualTo("Expected element selected by '$.Places[0].Inhabitants' to match 'greater than 200000' but was '" + this.placeInhabitants + "'"));
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
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].IsCapital", NHamcrest.Is.EqualTo(this.isCapital));
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
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body-header-mismatch")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0].Name", NHamcrest.Is.EqualTo(this.placeName), VerifyAs.Json);
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
                    .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("$.Places[0].IsCapital", NHamcrest.Is.EqualTo(!this.isCapital));
            });

            Assert.That(rve?.Message, Is.EqualTo("Expected element selected by '$.Places[0].IsCapital' to match '" + !this.isCapital + "' but was '" + this.isCapital + "'"));
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
                    .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
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
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0:].Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo(this.placeName)));
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
                .Get($"{MOCK_SERVER_BASE_URL}/json-response-body-header-mismatch")
                .Then()
                .StatusCode(200)
                .Body("$.Places[0:].Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo(this.placeName)), VerifyAs.Json);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementCollectionNHamcrestMatcherMisMatchThrowsTheExpectedException()
        {
            this.CreateStubForJsonResponseBody();

            string placeNamesInLocation = $"{this.placeName}, {this.location.Places[1].Name}";
            string mismatchCityName = Faker.Address.UkCounty();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/json-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("$.Places[0:].Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo(mismatchCityName)));
            });

            Assert.That(rve?.Message, Is.EqualTo($"Expected elements selected by '$.Places[0:].Name' to match 'a collection containing \"{mismatchCityName}\"', but was [{placeNamesInLocation}]"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON array response body element using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonArrayResponseBodyElementCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForJsonArrayResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-array-response-body")
                .Then()
                .StatusCode(200)
                .Body("$[1].text", NHamcrest.Is.EqualTo("Clean out the trash"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON array response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonArrayResponseBodyElementCollectionCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForJsonArrayResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-array-response-body")
                .Then()
                .StatusCode(200)
                .Body("$[0:].text", NHamcrest.Has.Item(NHamcrest.Is.EqualTo("Read the newspaper")));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON array response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementEmptyArrayValueCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForJsonEmptyArrayResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-empty-array-response-body")
                .Then()
                .StatusCode(200)
                .Body("$.errors", NHamcrest.Is.OfLength(0));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON array response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void JsonResponseBodyElementEmptyArrayValueMismatchThrowsExpectedException()
        {
            this.CreateStubForJsonEmptyArrayResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/json-empty-array-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("$.errors", NHamcrest.Is.OfLength(1));
             });

            Assert.That(rve?.Message, Is.EqualTo($"Expected element selected by '$.errors' to match 'NHamcrest.Core.LengthMatcher`1[System.Collections.ICollection]' but was '[]'"));
        }

        /// <summary>
        /// Creates the stub response for the JSON response body example.
        /// </summary>
        private void CreateStubForJsonResponseBody()
        {
            this.Server?.Given(Request.Create().WithPath("/json-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(this.location)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON array response body example.
        /// </summary>
        private void CreateStubForJsonArrayResponseBody()
        {
            var todoItems = new[]
            {
                new
                {
                    id = 1,
                    text = "Do the dishes",
                },
                new
                {
                    id = 2,
                    text = "Clean out the trash",
                },
                new
                {
                    id = 3,
                    text = "Read the newspaper",
                },
            };

            this.Server?.Given(Request.Create().WithPath("/json-array-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(todoItems)
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
                .WithBodyAsJson(this.location)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON response body example with an element with
        /// an empty array value.
        /// </summary>
        private void CreateStubForJsonEmptyArrayResponseBody()
        {
            var responseBody = new
            {
                success = true,
                errors = Array.Empty<string>(),
            };

            this.Server?.Given(Request.Create().WithPath("/json-empty-array-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(responseBody)
                .WithStatusCode(200));
        }
    }
}