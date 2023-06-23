// <copyright file="ResponseBodyXmlVerificationTests.cs" company="On Test Automation">
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
    public class ResponseBodyXmlVerificationTests : TestBase
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
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// an XML response body element using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void XmlResponseBodyElementStringValueCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForXmlResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body")
                .Then()
                .StatusCode(200)
                .Body("//Place[1]/Name", NHamcrest.Is.EqualTo(this.placeName));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// an XML response body element using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void XmlResponseBodyElementIntegerValueCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForXmlResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body")
                .Then()
                .StatusCode(200)
                .Body("//Place[1]/Inhabitants", NHamcrest.Is.GreaterThan(75000));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// an XML response body element using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void XmlResponseBodyElementBooleanValueCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForXmlResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body")
                .Then()
                .StatusCode(200)
                .Body("//Place[1]/IsCapital", NHamcrest.Is.EqualTo(this.isCapital));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// an XML response body element using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void XmlResponseBodyElementStringValueCanBeVerifiedOverridingResponseContentTypeHeader()
        {
            this.CreateStubForXmlResponseBodyWithResponseContentTypeHeaderMismatch();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body-header-mismatch")
                .Then()
                .StatusCode(200)
                .Body("//Place[1]/Name", NHamcrest.Is.EqualTo(this.placeName), VerifyAs.Xml);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the expected exception is thrown when the XPath does not return results.
        /// </summary>
        [Test]
        public void NoXPathResultsThrowsTheExpectedException()
        {
            this.CreateStubForXmlResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("//Places[0]/DoesNotExist", NHamcrest.Is.EqualTo(this.placeName));
            });

            Assert.That(rve?.Message, Is.EqualTo("XPath expression '//Places[0]/DoesNotExist' did not yield any results."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the expected exception is thrown when the element value returned
        /// by the XPath does not match the matcher type.
        /// </summary>
        [Test]
        public void ElementValueNotMatchingMatcherTypeThrowsTheExpectedException()
        {
            this.CreateStubForXmlResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("//Place[1]/Inhabitants", NHamcrest.Is.True());
            });

            Assert.That(rve?.Message, Is.EqualTo("Response element value " + this.placeInhabitants + " cannot be converted to value of type 'System.Boolean'"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// an XML response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void XmlResponseBodyElementCollectionCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForXmlResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body")
                .Then()
                .StatusCode(200)
                .Body("//Place/Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo(this.placeName)));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// an XML response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void XmlResponseBodyElementCollectionCanBeVerifiedUsingNHamcrestMatcherOverridingResponseContentTypeHeader()
        {
            this.CreateStubForXmlResponseBodyWithResponseContentTypeHeaderMismatch();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body-header-mismatch")
                .Then()
                .StatusCode(200)
                .Body("//Place/Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo(this.placeName)), VerifyAs.Xml);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying that
        /// an XML response body element collection not matching the specified
        /// NHamcrest matcher throws the right exception.
        /// </summary>
        [Test]
        public void XmlResponseBodyElementCollectionNHamcrestMatcherMisMatchThrowsTheExpectedException()
        {
            this.CreateStubForXmlResponseBody();

            string placeNamesInLocation = this.placeName + ", " + this.location.Places[1].Name;
            string mismatchCityName = Faker.Address.UkCounty();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/xml-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("//Place/Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo(mismatchCityName)));
            });

            Assert.That(rve?.Message, Is.EqualTo($"Expected elements selected by '//Place/Name' to match 'a collection containing \"" + mismatchCityName + "\"', but was [" + placeNamesInLocation + "]"));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example.
        /// </summary>
        private void CreateStubForXmlResponseBody()
        {
            this.Server?.Given(Request.Create().WithPath("/xml-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody(this.GetLocationAsXmlString())
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example with a non-matching
        /// response Content-Type header value.
        /// </summary>
        private void CreateStubForXmlResponseBodyWithResponseContentTypeHeaderMismatch()
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
            return "<?xml version=\"1.0\" encoding=\"utf-16\"?><Location xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Country>" + this.country + "</Country><State>" + this.state + "</State><ZipCode>" + this.zipcode + "</ZipCode><Places><Place><Name>" + this.placeName + "</Name><Inhabitants>" + this.placeInhabitants + "</Inhabitants><IsCapital>" + this.isCapital + "</IsCapital></Place><Place><Name>" + this.location.Places[1].Name + "</Name><Inhabitants>50000</Inhabitants><IsCapital>false</IsCapital></Place></Places></Location>";
        }
    }
}