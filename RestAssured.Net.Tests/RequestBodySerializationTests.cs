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
namespace RestAssured.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using RestAssured.Request.Exceptions;
    using RestAssured.Tests.Models;
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class RequestBodySerializationTests : TestBase
    {
        private readonly string expectedSerializedJsonRequestBody = "{\"Country\":\"United States\",\"State\":\"California\",\"ZipCode\":90210,\"Places\":[{\"Name\":\"Sun City\",\"Inhabitants\":100000,\"IsCapital\":true},{\"Name\":\"Pleasure Meadow\",\"Inhabitants\":50000,\"IsCapital\":false}]}";
        private readonly string expectedSerializedObject = "{\"Id\":1,\"Title\":\"My post title\",\"Body\":\"My post body\"}";
        private readonly string xmlBody = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Location xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Country>United States</Country><State>California</State><ZipCode>90210</ZipCode><Places><Place><Name>Sun City</Name><Inhabitants>100000</Inhabitants><IsCapital>true</IsCapital></Place><Place><Name>Pleasure Meadow</Name><Inhabitants>50000</Inhabitants><IsCapital>false</IsCapital></Place></Places></Location>";

        private Location location = new Location();

        /// <summary>
        /// Creates the <see cref="Location"/> object to be serialized.
        /// </summary>
        [SetUp]
        public void SetUpLocation()
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

            this.location = new Location
            {
                Country = "United States",
                State = "California",
                ZipCode = 90210,
                Places = new List<Place>() { firstPlace, secondPlace },
            };
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for serializing
        /// and sending a JSON request body when performing an HTTP POST.
        /// </summary>
        [Test]
        public void ObjectCanBeSerializedToJson()
        {
            this.CreateStubForJsonRequestBody();

            Given()
            .Body(this.location)
            .When()
            .Post("http://localhost:9876/json-serialization")
            .Then()
            .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for serializing
        /// a Dictionary to JSON and sending it when performing an HTTP POST.
        /// </summary>
        [Test]
        public void DictionaryCanBeSerializedToJson()
        {
            this.CreateStubForObjectSerialization();

            Dictionary<string, object> post = new Dictionary<string, object>
            {
                { "Id", 1 },
                { "Title", "My post title" },
                { "Body", "My post body" },
            };

            Given()
            .Body(post)
            .When()
            .Post("http://localhost:9876/object-serialization")
            .Then()
            .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for serializing
        /// an anonymous type to JSON and sending it when performing an HTTP POST.
        /// </summary>
        [Test]
        public void AnonymousObjectCanBeSerializedToJson()
        {
            this.CreateStubForObjectSerialization();

            var post = new
            {
                Id = 1,
                Title = "My post title",
                Body = "My post body",
            };

            Given()
            .Body(post)
            .When()
            .Post("http://localhost:9876/object-serialization")
            .Then()
            .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for serializing
        /// and sending an XML request body when performing an HTTP POST.
        /// </summary>
        [Test]
        public void ObjectCanBeSerializedToXml()
        {
            this.CreateStubForXmlRequestBody();

            Given()
            .ContentType("application/xml")
            .Body(this.location)
            .When()
            .Post("http://localhost:9876/xml-serialization")
            .Then()
            .StatusCode(201);
        }

        /// <summary>
        /// Verifies that the correct exception is thrown when the request body
        /// cannot be serialized based on the Content-Type header value.
        /// </summary>
        [Test]
        public void UnableToSerializeThrowsTheExpectedException()
        {
            this.CreateStubForXmlRequestBody();

            var rce = Assert.Throws<RequestCreationException>(() =>
            {
                Given()
                .ContentType("application/something")
                .Body(this.location)
                .When()
                .Post("http://localhost:9876/xml-serialization")
                .Then()
                .StatusCode(201);
            });

            Assert.That(rce?.Message, Is.EqualTo("Could not determine how to serialize request based on specified content type 'application/something'"));
        }

        /// <summary>
        /// Creates the stub response for the JSON string request body example.
        /// </summary>
        private void CreateStubForJsonRequestBody()
        {
            this.Server?.Given(Request.Create().WithPath("/json-serialization").UsingPost()
                .WithBody(new JsonMatcher(this.expectedSerializedJsonRequestBody)))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }

        /// <summary>
        /// Creates the stub response for the object serialization example.
        /// </summary>
        private void CreateStubForObjectSerialization()
        {
            this.Server?.Given(Request.Create().WithPath("/object-serialization").UsingPost()
                .WithBody(new JsonMatcher(this.expectedSerializedObject)))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }

        /// <summary>
        /// Creates the stub response for the XML request body example.
        /// </summary>
        private void CreateStubForXmlRequestBody()
        {
            this.Server?.Given(Request.Create().WithPath("/xml-serialization").UsingPost()
                .WithBody(new XPathMatcher("//Places[count(Place) = 2]")))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }
    }
}