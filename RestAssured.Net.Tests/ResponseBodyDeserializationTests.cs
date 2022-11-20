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
    public class ResponseBodyDeserializationTests : TestBase
    {
        private readonly string xmlBody = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Location xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Country>United States</Country><State>California</State><ZipCode>90210</ZipCode><Places><Place><Name>Sun City</Name><Inhabitants>100000</Inhabitants><IsCapital>true</IsCapital></Place><Place><Name>Pleasure Meadow</Name><Inhabitants>50000</Inhabitants><IsCapital>false</IsCapital></Place></Places></Location>";

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
        /// A test demonstrating RestAssuredNet syntax for deserializing
        /// a XML response into an object when performing an HTTP GET.
        /// </summary>
        [Test]
        public void ObjectCanBeDeserializedFromXml()
        {
            this.CreateStubForXmlResponseBody();

            Location responseLocation = (Location)Given()
            .When()
            .Get("http://localhost:9876/xml-deserialization")
            .As(typeof(Location));

            Assert.That(responseLocation.Country, Is.EqualTo("United States"));
            Assert.That(responseLocation.Places.Count, Is.EqualTo(2));
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
                .Get("http://localhost:9876/xml-deserialization-unrecognized-content-type")
                .As(typeof(Location));
            });

            Assert.That(de.Message, Is.EqualTo("Unable to deserialize response with Content-Type 'application/something'"));
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
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(location)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example.
        /// </summary>
        private void CreateStubForXmlResponseBody()
        {
            this.Server.Given(Request.Create().WithPath("/xml-deserialization").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody(this.xmlBody)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example with an unrecognized Content-Type.
        /// </summary>
        private void CreateStubForXmlResponseBodyWithUnrecognizedContentType()
        {
            this.Server.Given(Request.Create().WithPath("/xml-deserialization-unrecognized-content-type").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/something")
                .WithBody(this.xmlBody)
                .WithStatusCode(200));
        }
    }
}