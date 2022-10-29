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
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
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
        [Ignore("Reactivate when XML element verification is properly implemented")]
        public void XmlResponseBodyElementStringValueCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForXmlResponseBody();

            Given()
            .When()
            .Get("http://localhost:9876/xml-response-body")
            .Then()
            .StatusCode(200)
            .Body("//Place[0]/Name", NHamcrest.Is.EqualTo("Sun City"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the expected exception is thrown when the XPath does not return results.
        /// </summary>
        [Test]
        public void NoXPathResultsThrowsTheExpectedException()
        {
            this.CreateStubForXmlResponseBody();

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            {
                Given()
                .When()
                .Get("http://localhost:9876/xml-response-body")
                .Then()
                .StatusCode(200)
                .Body("//Places[0]/DoesNotExist", NHamcrest.Is.EqualTo("Sun City"));
            });

            Assert.That(ae.Message, Is.EqualTo("XPath expression '//Places[0]/DoesNotExist' did not yield any results."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a JSON response body element collection using an NHamcrest matcher.
        /// </summary>
        [Test]
        [Ignore("Reactivate when XML element verification is properly implemented")]
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
        [Ignore("Reactivate when XML element verification is properly implemented")]
        public void XmlResponseBodyElementCollectionNHamcrestMatcherMisMatchThrowsTheExpectedException()
        {
            this.CreateStubForXmlResponseBody();

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            { 
                Given()
                .When()
                .Get("http://localhost:9876/xml-response-body")
                .Then()
                .StatusCode(200)
                .Body("//Place/Name", NHamcrest.Has.Item(NHamcrest.Is.EqualTo("Atlantis")));
            });

            Assert.That(ae.Message, Is.EqualTo($"Expected elements selected by '//Place/Name' to match 'a collection containing \"Atlantis\"', but was [Sun City, Pleasure Meadow]"));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example.
        /// </summary>
        private void CreateStubForXmlResponseBody()
        {
            this.Server.Given(Request.Create().WithPath("/xml-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody(this.xmlBody)
                .WithStatusCode(200));
        }
    }
}