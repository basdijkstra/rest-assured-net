// <copyright file="ResponseBodyDeserializationTests.cs" company="On Test Automation">
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
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using RestAssured.Response.Deserialization;
    using RestAssured.Response.Exceptions;
    using RestAssured.Tests.Models;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class ResponseBodyDeserializationTests : TestBase
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
        /// Initializes the <see cref="Location"/> object used in these tests.
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
        /// Clears the <see cref="Place"/> objects.
        /// </summary>
        [TearDown]
        public void ClearPlaces()
        {
            this.location.Places = new List<Place>();
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
                .Get($"{MOCK_SERVER_BASE_URL}/json-deserialization")
                .DeserializeTo(typeof(Location));

            Assert.That(responseLocation.Country, Is.EqualTo(this.country));
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo(this.placeName));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(this.placeInhabitants));
            Assert.That(firstPlace.IsCapital, Is.EqualTo(this.isCapital));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for deserializing
        /// a JSON response into an object when performing an HTTP GET
        /// after performing some initial verifications.
        /// </summary>
        [Test]
        public void ObjectCanBeDeserializedFromJsonAfterVerifications()
        {
            this.CreateStubForJsonResponseBody();

            Location responseLocation = (Location)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-deserialization")
                .Then()
                .StatusCode(200)
                .And()
                .DeserializeTo(typeof(Location));

            Assert.That(responseLocation.Country, Is.EqualTo(this.country));
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo(this.placeName));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(this.placeInhabitants));
            Assert.That(firstPlace.IsCapital, Is.EqualTo(this.isCapital));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for deserializing
        /// a JSON response into an object when performing an HTTP GET
        /// using the DeserializeTo() alias method.
        /// </summary>
        [Test]
        public void ObjectCanBeDeserializedFromJsonUsingDeserializeToOverridingContentTypeHeader()
        {
            this.CreateStubForJsonResponseBodyWithNonMatchingContentTypeHeader();

            Location responseLocation = (Location)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-deserialization-header-mismatch")
                .Then()
                .DeserializeTo(typeof(Location), DeserializeAs.Json);

            Assert.That(responseLocation.Country, Is.EqualTo(this.country));
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo(this.placeName));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(this.placeInhabitants));
            Assert.That(firstPlace.IsCapital, Is.EqualTo(this.isCapital));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for deserializing
        /// a XML response into an object when performing an HTTP GET.
        /// </summary>
        [Test]
        public void ObjectCanBeDeserializedFromXml()
        {
            this.CreateStubForXmlResponseBody();

            Location responseLocation = (Location)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-deserialization")
                .Then()
                .DeserializeTo(typeof(Location));

            Assert.That(responseLocation.Country, Is.EqualTo(this.country));
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo(this.placeName));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(this.placeInhabitants));
            Assert.That(firstPlace.IsCapital, Is.EqualTo(this.isCapital));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for deserializing
        /// a XML response into an object when performing an HTTP GET.
        /// </summary>
        [Test]
        public void ObjectCanBeDeserializedFromXmlUsingDeserializeToOverridingContentTypeHeader()
        {
            this.CreateStubForXmlResponseBodyWithNonMatchingContentTypeHeader();

            Location responseLocation = (Location)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-deserialization-header-mismatch")
                .Then()
                .DeserializeTo(typeof(Location), DeserializeAs.Xml);

            Assert.That(responseLocation.Country, Is.EqualTo(this.country));
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo(this.placeName));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(this.placeInhabitants));
            Assert.That(firstPlace.IsCapital, Is.EqualTo(this.isCapital));
        }

        /// <summary>
        /// Verifies that the correct exception is thrown when the request body
        /// cannot be deserialized based on the Content-Type header value.
        /// </summary>
        [Test]
        public void UnableToDeserializeThrowsTheExpectedException()
        {
            this.CreateStubForXmlResponseBodyWithUnrecognizedContentType();

            var de = Assert.Throws<DeserializationException>(() =>
            {
                Location responseLocation = (Location)Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/xml-deserialization-unrecognized-content-type")
                    .DeserializeTo(typeof(Location));
            });

            Assert.That(de?.Message, Is.EqualTo("Unable to deserialize response with Content-Type 'application/something'"));
        }

        /// <summary>
        /// Creates the stub response for the JSON response body example.
        /// </summary>
        private void CreateStubForJsonResponseBody()
        {
            this.Server?.Given(Request.Create().WithPath("/json-deserialization").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(this.location)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON response body example with
        /// a Content-Type header that doesn't match the content.
        /// </summary>
        private void CreateStubForJsonResponseBodyWithNonMatchingContentTypeHeader()
        {
            this.Server?.Given(Request.Create().WithPath("/json-deserialization-header-mismatch").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "text/plain")
                .WithBodyAsJson(this.location)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example.
        /// </summary>
        private void CreateStubForXmlResponseBody()
        {
            this.Server?.Given(Request.Create().WithPath("/xml-deserialization").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody(this.GetLocationAsXmlString())
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example.
        /// </summary>
        private void CreateStubForXmlResponseBodyWithNonMatchingContentTypeHeader()
        {
            this.Server?.Given(Request.Create().WithPath("/xml-deserialization-header-mismatch").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "text/plain")
                .WithBody(this.GetLocationAsXmlString())
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example with an unrecognized Content-Type.
        /// </summary>
        private void CreateStubForXmlResponseBodyWithUnrecognizedContentType()
        {
            this.Server?.Given(Request.Create().WithPath("/xml-deserialization-unrecognized-content-type").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/something")
                .WithBody(this.GetLocationAsXmlString())
                .WithStatusCode(200));
        }

        /// <summary>
        /// Returns an XML string representing a <see cref="Location"/>.
        /// </summary>
        /// <returns>An XML string representing a <see cref="Location"/>.</returns>
        private new string GetLocationAsXmlString()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-16\"?><Location xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Country>" + this.country + "</Country><State>" + this.state + "</State><ZipCode>" + this.zipcode + "</ZipCode><Places><Place><Name>" + this.placeName + "</Name><Inhabitants>" + this.placeInhabitants + "</Inhabitants><IsCapital>" + this.isCapital.ToString().ToLower() + "</IsCapital></Place><Place><Name>Pleasure Meadow</Name><Inhabitants>50000</Inhabitants><IsCapital>false</IsCapital></Place></Places></Location>";
        }
    }
}