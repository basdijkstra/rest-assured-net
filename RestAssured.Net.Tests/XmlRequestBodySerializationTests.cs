// <copyright file="XmlRequestBodySerializationTests.cs" company="On Test Automation">
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
    using NUnit.Framework;
    using RestAssured.Request.Exceptions;
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class XmlRequestBodySerializationTests : TestBase
    {
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
                .Body(this.GetLocation())
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/xml-serialization")
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
                    .Body(this.GetLocation())
                    .When()
                    .Post($"{MOCK_SERVER_BASE_URL}/xml-serialization")
                    .Then()
                    .StatusCode(201);
            });

            Assert.That(rce?.Message, Is.EqualTo("Could not determine how to serialize request based on specified content type 'application/something'"));
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