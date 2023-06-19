// <copyright file="XmlSchemaValidationTests.cs" company="On Test Automation">
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
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    using NUnit.Framework;
    using RestAssured.Response.Exceptions;
    using RestAssured.Tests.Schemas;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class XmlSchemaValidationTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for validating a response
        /// against an XML schema supplied as a string.
        /// </summary>
        [Test]
        public void XmlSchemaCanBeSuppliedAndVerifiedAsString()
        {
            this.CreateStubForXmlSchemaValidation();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-schema-validation")
                .Then()
                .StatusCode(200)
                .And()
                .MatchesXsd(XmlSchemaDefinitions.MatchingXmlSchemaAsString);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for validating a response
        /// against an XML schema supplied as an <see cref="XmlSchemaSet"/>.
        /// </summary>
        [Test]
        public void XmlSchemaCanBeSuppliedAndVerifiedAsXmlSchemaSet()
        {
            this.CreateStubForXmlSchemaValidation();

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(string.Empty, XmlReader.Create(new StringReader(XmlSchemaDefinitions.MatchingXmlSchemaAsString)));

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/xml-schema-validation")
                .Then()
                .StatusCode(200)
                .And()
                .MatchesXsd(schemaSet);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for validating a response
        /// against an inline DTD definition.
        /// </summary>
        [Test]
        public void XmlResponseCanBeVerifiedAgainstInlineDtd()
        {
            this.CreateStubForMatchingDtdValidation();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/matching-dtd-validation")
                .Then()
                .StatusCode(200)
                .And()
                .MatchesInlineDtd();
        }

        /// <summary>
        /// A test checking that an XML schema mismatch throws the expected exception.
        /// </summary>
        [Test]
        public void MismatchWithXmlSchemaThrowsTheExpectedException()
        {
            this.CreateStubForXmlSchemaValidation();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/xml-schema-validation")
                    .Then()
                    .StatusCode(200)
                    .And()
                    .MatchesXsd(XmlSchemaDefinitions.NonMatchingXmlSchemaAsString);
            });

            Assert.That(rve?.Message, Does.Contain("Response body did not match XML schema supplied."));
        }

        /// <summary>
        /// A test checking that an inline DTD mismatch throws the expected exception.
        /// </summary>
        [Test]
        public void MismatchWithInlineDtdThrowsTheExpectedException()
        {
            this.CreateStubForNonMatchingDtdValidation();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/non-matching-dtd-validation")
                    .Then()
                    .StatusCode(200)
                    .And()
                    .MatchesInlineDtd();
            });

            Assert.That(rve?.Message, Does.Contain("Response body did not match inline DTD."));
        }

        /// <summary>
        /// A test checking that an incorrectly formatted XML schema throws the expected exception.
        /// </summary>
        [Test]
        public void IncorrectlyFormattedXmlSchemaThrowsTheExpectedException()
        {
            this.CreateStubForXmlSchemaValidation();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/xml-schema-validation")
                    .Then()
                    .StatusCode(200)
                    .And()
                    .MatchesXsd(XmlSchemaDefinitions.InvalidXmlSchemaAsString);
            });

            Assert.That(rve?.Message, Does.Contain("Could not parse supplied XML schema. Error:"));
        }

        /// <summary>
        /// A test checking that a response with an unexpected Content-Type throws the expected exception.
        /// </summary>
        [Test]
        public void UnexpectedResponseContentTypeThrowsTheExpectedExceptionWhenValidatingXsd()
        {
            this.CreateStubForXmlSchemaUnexpectedResponseContentType();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/xml-schema-unexpected-content-type")
                    .Then()
                    .StatusCode(200)
                    .And()
                    .MatchesXsd(XmlSchemaDefinitions.MatchingXmlSchemaAsString);
            });

            Assert.That(rve?.Message, Is.EqualTo("Expected response Content-Type header to contain 'xml', but was 'application/something'"));
        }

        /// <summary>
        /// A test checking that a response with an unexpected Content-Type throws the expected exception.
        /// </summary>
        [Test]
        public void UnexpectedResponseContentTypeThrowsTheExpectedExceptionWhenValidatingDtd()
        {
            this.CreateStubForXmlSchemaUnexpectedResponseContentType();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/xml-schema-unexpected-content-type")
                    .Then()
                    .StatusCode(200)
                    .And()
                    .MatchesInlineDtd();
            });

            Assert.That(rve?.Message, Is.EqualTo("Expected response Content-Type header to contain 'xml', but was 'application/something'"));
        }

        /// <summary>
        /// Creates the stub response for the XML schema validation example.
        /// </summary>
        private void CreateStubForXmlSchemaValidation()
        {
            this.Server?.Given(Request.Create().WithPath("/xml-schema-validation").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody(this.GetLocationAsXmlString())
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the matching inline DTD validation example.
        /// </summary>
        private void CreateStubForMatchingDtdValidation()
        {
            this.Server?.Given(Request.Create().WithPath("/matching-dtd-validation").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody(XmlSchemaDefinitions.XmlWithMatchingInlineDtd)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the non-matching inline DTD validation example.
        /// </summary>
        private void CreateStubForNonMatchingDtdValidation()
        {
            this.Server?.Given(Request.Create().WithPath("/non-matching-dtd-validation").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody(XmlSchemaDefinitions.XmlWithNonMatchingInlineDtd)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON schema validation example
        /// with an unexpected response Content-Type header value.
        /// </summary>
        private void CreateStubForXmlSchemaUnexpectedResponseContentType()
        {
            this.Server?.Given(Request.Create().WithPath("/xml-schema-unexpected-content-type").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/something")
                .WithBody("Something")
                .WithStatusCode(200));
        }
    }
}