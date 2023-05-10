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
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo("Sun City"));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(100000));
            Assert.That(firstPlace.IsCapital, Is.True);
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
                .Get("http://localhost:9876/json-deserialization")
                .Then()
                .StatusCode(200)
                .And()
                .Extract()
                .As(typeof(Location));

            Assert.That(responseLocation.Country, Is.EqualTo("United States"));
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo("Sun City"));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(100000));
            Assert.That(firstPlace.IsCapital, Is.True);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for deserializing
        /// a JSON response into an object when performing an HTTP GET
        /// using the DeserializeTo() alias method.
        /// </summary>
        [Test]
        public void ObjectCanBeDeserializedFromJsonUsingDeserializeTo()
        {
            this.CreateStubForJsonResponseBody();

            Location responseLocation = (Location)Given()
                .When()
                .Get("http://localhost:9876/json-deserialization")
                .Then()
                .DeserializeTo(typeof(Location));

            Assert.That(responseLocation.Country, Is.EqualTo("United States"));
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo("Sun City"));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(100000));
            Assert.That(firstPlace.IsCapital, Is.True);
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
                .Get("http://localhost:9876/json-deserialization-header-mismatch")
                .Then()
                .DeserializeTo(typeof(Location), DeserializeAs.Json);

            Assert.That(responseLocation.Country, Is.EqualTo("United States"));
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo("Sun City"));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(100000));
            Assert.That(firstPlace.IsCapital, Is.True);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for deserializing
        /// a JSON response into an object when performing an HTTP GET
        /// after performing some initial verifications, using the
        /// DeserializeTo() alias method.
        /// </summary>
        [Test]
        public void ObjectCanBeDeserializedFromJsonAfterVerificationsUsingDeserializeTo()
        {
            this.CreateStubForJsonResponseBody();

            Location responseLocation = (Location)Given()
                .When()
                .Get("http://localhost:9876/json-deserialization")
                .Then()
                .StatusCode(200)
                .And()
                .DeserializeTo(typeof(Location));

            Assert.That(responseLocation.Country, Is.EqualTo("United States"));
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo("Sun City"));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(100000));
            Assert.That(firstPlace.IsCapital, Is.True);
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
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo("Sun City"));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(100000));
            Assert.That(firstPlace.IsCapital, Is.True);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for deserializing
        /// a XML response into an object when performing an HTTP GET.
        /// </summary>
        [Test]
        public void ObjectCanBeDeserializedFromXmlUsingDeserializeTo()
        {
            this.CreateStubForXmlResponseBody();

            Location responseLocation = (Location)Given()
                .When()
                .Get("http://localhost:9876/xml-deserialization")
                .Then()
                .DeserializeTo(typeof(Location));

            Assert.That(responseLocation.Country, Is.EqualTo("United States"));
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo("Sun City"));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(100000));
            Assert.That(firstPlace.IsCapital, Is.True);
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
                .Get("http://localhost:9876/xml-deserialization-header-mismatch")
                .Then()
                .DeserializeTo(typeof(Location), DeserializeAs.Xml);

            Assert.That(responseLocation.Country, Is.EqualTo("United States"));
            Assert.That(responseLocation.Places?.Count, Is.EqualTo(2));

            Place firstPlace = responseLocation.Places!.First();

            Assert.That(firstPlace.Name, Is.EqualTo("Sun City"));
            Assert.That(firstPlace.Inhabitants, Is.EqualTo(100000));
            Assert.That(firstPlace.IsCapital, Is.True);
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
                .WithBodyAsJson(this.GetLocation())
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
                .WithBodyAsJson(this.GetLocation())
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
    }
}