// <copyright file="ResponseXmlValueExtractionTests.cs" company="On Test Automation">
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
    public class ResponseXmlValueExtractionTests : TestBase
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
        /// A test demonstrating RestAssuredNet syntax for extracting an
        /// element value from an XML response body.
        /// </summary>
        [Test]
        public void XmlResponseBodyElementValueCanBeExtracted()
        {
            this.CreateStubForXmlResponseWithBodyAndHeaders();

            string placeName = (string)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body")
                .Then()
                .StatusCode(200)
                .Extract().Body("//Place[1]/Name");

            Assert.That(placeName, Is.EqualTo(this.placeName));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// string element value from an XML response body, overriding
        /// the default extraction method (determined using the response
        /// Content-Type header).
        /// </summary>
        [Test]
        public void XmlResponseBodyElementValueCanBeExtractedOverridingResponseContentType()
        {
            this.CreateStubForXmlResponseWithResponseContentTypeHeaderMismatch();

            string placeName = (string)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body-header-mismatch")
                .Then()
                .StatusCode(200)
                .Extract().Body("//Place[1]/Name", ExtractAs.Xml);

            Assert.That(placeName, Is.EqualTo(this.placeName));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// list of element values from an XML response body.
        /// </summary>
        [Test]
        public void XmlResponseBodyMultipleElementsCanBeExtracted()
        {
            this.CreateStubForXmlResponseWithBodyAndHeaders();

            // At least for now, if you want to retrieve multiple
            // XML response body element values, they will have to be
            // stored in an object of type List<string>.
            List<string> placeNames = (List<string>)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body")
                .Then()
                .StatusCode(200)
                .Extract().Body("//Place/Name");

            Assert.That(placeNames.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the expected exception is thrown when the XPath does not return results.
        /// </summary>
        [Test]
        public void NoXPathResultsThrowsTheExpectedException()
        {
            this.CreateStubForXmlResponseWithBodyAndHeaders();

            var ee = Assert.Throws<ExtractionException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body")
                    .Then()
                    .StatusCode(200)
                    .Extract().Body("//Place/DoesNotExist");
            });

            Assert.That(ee?.Message, Is.EqualTo("XPath expression '//Place/DoesNotExist' did not yield any results."));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example.
        /// </summary>
        private void CreateStubForXmlResponseWithBodyAndHeaders()
        {
            this.Server?.Given(Request.Create().WithPath("/xml-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody(this.GetLocationAsXmlString())
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example.
        /// </summary>
        private void CreateStubForXmlResponseWithResponseContentTypeHeaderMismatch()
        {
            this.Server?.Given(Request.Create().WithPath("/xml-response-body-header-mismatch").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "text/plain")
                .WithBody(this.GetLocationAsXmlString())
                .WithStatusCode(200));
        }

        /// <summary>
        /// Returns an XML string representing a <see cref="Location"/>.
        /// </summary>
        /// <returns>An XML string representing a <see cref="Location"/>.</returns>
        private new string GetLocationAsXmlString()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-16\"?><Location xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Country>" + this.country + "</Country><State>" + this.state + "</State><ZipCode>" + this.zipcode + "</ZipCode><Places><Place><Name>" + this.placeName + "</Name><Inhabitants>" + this.placeInhabitants + "</Inhabitants><IsCapital>" + this.isCapital + "</IsCapital></Place><Place><Name>Pleasure Meadow</Name><Inhabitants>50000</Inhabitants><IsCapital>false</IsCapital></Place></Places></Location>";
        }
    }
}