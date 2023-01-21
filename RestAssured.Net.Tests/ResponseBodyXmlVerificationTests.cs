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
    using NUnit.Framework;
    using RestAssured.Response.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class ResponseBodyXmlVerificationTests : TestBase
    {
        private readonly string xmlBody = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Location xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Country>United States</Country><State>California</State><ZipCode>90210</ZipCode><Places><Place><Name>Sun City</Name><Inhabitants>100000</Inhabitants><IsCapital>true</IsCapital></Place><Place><Name>Pleasure Meadow</Name><Inhabitants>50000</Inhabitants><IsCapital>false</IsCapital></Place></Places></Location>";

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
                .Get("http://localhost:9876/xml-response-body")
                .Then()
                .StatusCode(200)
                .Body("//Place[1]/Name", NHamcrest.Is.EqualTo("Sun City"));
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
                .Get("http://localhost:9876/xml-response-body")
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
                .Get("http://localhost:9876/xml-response-body")
                .Then()
                .StatusCode(200)
                .Body("//Place[1]/IsCapital", NHamcrest.Is.True());
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
                    .Get("http://localhost:9876/xml-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("//Places[0]/DoesNotExist", NHamcrest.Is.EqualTo("Sun City"));
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
                    .Get("http://localhost:9876/xml-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("//Place[1]/Inhabitants", NHamcrest.Is.True());
            });

            Assert.That(rve?.Message, Is.EqualTo("Response element value '100000' cannot be converted to value of type 'System.Boolean'"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void XmlResponseBodyElementCollectionCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForXmlResponseBody();

            Given()
                .When()
                .Get("http://localhost:9876/xml-response-body")
                .Then()
                .StatusCode(200)
                .Body("//Place/Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo("Sun City")));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// an XML response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void XmlResponseBodyElementCollectionNHamcrestMatcherMisMatchThrowsTheExpectedException()
        {
            this.CreateStubForXmlResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get("http://localhost:9876/xml-response-body")
                    .Then()
                    .StatusCode(200)
                    .Body("//Place/Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo("Atlantis")));
            });

            Assert.That(rve?.Message, Is.EqualTo($"Expected elements selected by '//Place/Name' to match 'a collection containing \"Atlantis\"', but was [Sun City, Pleasure Meadow]"));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example.
        /// </summary>
        private void CreateStubForXmlResponseBody()
        {
            this.Server?.Given(Request.Create().WithPath("/xml-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody(this.xmlBody)
                .WithStatusCode(200));
        }
    }
}