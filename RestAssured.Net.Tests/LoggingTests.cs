// <copyright file="LoggingTests.cs" company="On Test Automation">
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
    using RestAssured.Tests.Models;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class LoggingTests : TestBase
    {
        private readonly string xmlBody = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Location xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Country>United States</Country><State>California</State><ZipCode>90210</ZipCode><Places><Place><Name>Sun City</Name><Inhabitants>100000</Inhabitants><IsCapital>true</IsCapital></Place><Place><Name>Pleasure Meadow</Name><Inhabitants>50000</Inhabitants><IsCapital>false</IsCapital></Place></Places></Location>";
        private readonly string jsonBody = "{\"id\": 1, \"user\": \"John Doe\"}";

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// JSON request details to the standard output.
        /// </summary>
        [Test]
        public void RequestDetailsCanBeWrittenToStandardOutputForJson()
        {
            this.CreateStubForLoggingJsonResponse();

            Given()
            .Log().All()
            .And()
            .Accept("application/json")
            .Header("CustomHeader", "custom header value")
            .ContentType("application/json")
            .Body(this.jsonBody)
            .When()
            .Get("http://localhost:9876/log-json-response")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// XML request details to the standard output.
        /// </summary>
        [Test]
        public void RequestDetailsCanBeWrittenToStandardOutputForXml()
        {
            this.CreateStubForLoggingXmlResponse();

            Given()
            .Log().All()
            .ContentType("application/xml")
            .Body(this.xmlBody)
            .When()
            .Get("http://localhost:9876/log-xml-response")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output.
        /// </summary>
        [Test]
        public void ResponseDetailsCanBeWrittenToStandardOutputForJson()
        {
            this.CreateStubForLoggingJsonResponse();

            Given()
            .When()
            .Get("http://localhost:9876/log-json-response")
            .Then()
            .Log().All()
            .And()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output.
        /// </summary>
        [Test]
        public void ResponseDetailsCanBeWrittenToStandardOutputForXml()
        {
            this.CreateStubForLoggingXmlResponse();

            Given()
            .When()
            .Get("http://localhost:9876/log-xml-response")
            .Then()
            .Log().All()
            .And()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output.
        /// </summary>
        [Test]
        public void NoResponseBodyDoesntThrowNullReferenceException()
        {
            this.CreateStubForLoggingResponseWithoutBody();

            Given()
            .When()
            .Get("http://localhost:9876/log-no-response-body")
            .Then()
            .Log().All()
            .And()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output.
        /// </summary>
        [Test]
        public void NoRequestBodyDoesntThrowNullReferenceException()
        {
            this.CreateStubForLoggingResponseWithoutBody();

            Given()
            .Log().All()
            .When()
            .Get("http://localhost:9876/log-no-response-body")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for the JSON response body example.
        /// </summary>
        private void CreateStubForLoggingJsonResponse()
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

            this.Server?.Given(Request.Create().WithPath("/log-json-response").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(location));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example.
        /// </summary>
        private void CreateStubForLoggingXmlResponse()
        {
            this.Server?.Given(Request.Create().WithPath("/log-xml-response").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody(this.xmlBody)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the no response body example.
        /// </summary>
        private void CreateStubForLoggingResponseWithoutBody()
        {
            this.Server?.Given(Request.Create().WithPath("/log-no-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}