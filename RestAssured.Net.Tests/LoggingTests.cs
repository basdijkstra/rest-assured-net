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
    using NUnit.Framework;
    using RestAssured.Logging;
    using RestAssured.Request.Builders;
    using RestAssured.Request.Logging;
    using RestAssured.Response.Logging;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class LoggingTests : TestBase
    {
        private readonly string jsonBody = "{\"id\": 1, \"user\": \"John Doe\"}";

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// JSON request details to the standard output.
        /// </summary>
        [Test]
        public void RequestDetailsCanBeWrittenToStandardOutputForJson()
        {
            this.CreateStubForLoggingJsonResponse();

            var logConfig = new LogConfiguration
            {
                RequestLogLevel = Logging.RequestLogLevel.All,
            };

            Given()
                .Log(logConfig)
                .And()
                .Accept("application/json")
                .Header("CustomHeader", "custom header value")
                .ContentType("application/json")
                .Body(this.jsonBody)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// JSON request details to the standard output.
        /// </summary>
        [Test]
        public void RequestLogLevelCanBeSpecifiedUsingObsoleteMethod()
        {
            this.CreateStubForLoggingJsonResponse();

            Given()
                .Log(RestAssured.Request.Logging.RequestLogLevel.All)
                .And()
                .Accept("application/json")
                .Header("CustomHeader", "custom header value")
                .ContentType("application/json")
                .Body(this.jsonBody)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
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

            var logConfig = new LogConfiguration
            {
                RequestLogLevel = Logging.RequestLogLevel.All,
            };

            Given()
                .Log(logConfig)
                .ContentType("application/xml")
                .Body(this.GetLocationAsXmlString())
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-xml-response")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// all response details to the standard output.
        /// </summary>
        [Test]
        public void ResponseDetailsCanBeWrittenToStandardOutputForJson()
        {
            this.CreateStubForLoggingJsonResponse();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = Logging.ResponseLogLevel.All,
            };

            Given()
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// XML response details to the standard output.
        /// </summary>
        [Test]
        public void ResponseDetailsCanBeWrittenToStandardOutputForXmlUsingObsoleteMethod()
        {
            this.CreateStubForLoggingXmlResponse();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-xml-response")
                .Then()
                .Log(RestAssured.Response.Logging.ResponseLogLevel.All)
                .And()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// XML response details to the standard output.
        /// </summary>
        [Test]
        public void ResponseDetailsCanBeWrittenToStandardOutputForXml()
        {
            this.CreateStubForLoggingXmlResponse();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = Logging.ResponseLogLevel.All,
            };

            Given()
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-xml-response")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output.
        /// This response has no body, which should not throw an exception.
        /// </summary>
        [Test]
        public void NoResponseBodyDoesntThrowNullReferenceException()
        {
            this.CreateStubForLoggingResponseWithoutBody();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = Logging.ResponseLogLevel.All,
            };

            Given()
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-no-response-body")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output.
        /// This request has no body, which should not throw an exception.
        /// </summary>
        [Test]
        public void NoRequestBodyDoesntThrowNullReferenceException()
        {
            this.CreateStubForLoggingResponseWithoutBody();

            var logConfig = new LogConfiguration
            {
                RequestLogLevel = Logging.RequestLogLevel.All,
            };

            Given()
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-no-response-body")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output only if the status
        /// code is 4xx or 5xx.
        /// This test should log details, because the status code is 404.
        /// </summary>
        [Test]
        public void ResponseBodyDetailsAreLoggedOnlyOnErrorResponseCode()
        {
            this.CreateStubForErrorResponse();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = Logging.ResponseLogLevel.OnError,
            };

            Given()
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/error-response-body")
                .Then()
                .StatusCode(404);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output only if the status
        /// code is 4xx or 5xx.
        /// This test should not log details, because the status code is 200.
        /// </summary>
        [Test]
        public void ResponseBodyDetailsAreNotLoggedOnOkResponseCode()
        {
            this.CreateStubForLoggingJsonResponse();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = Logging.ResponseLogLevel.OnError,
            };

            Given()
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output only if the status
        /// code is 4xx or 5xx.
        /// This test should not log details, because the status code is 200.
        /// </summary>
        [Test]
        public void ResponseBodyDetailsAreLoggedIfVerificationFails()
        {
            this.CreateStubForLoggingJsonResponse();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = Logging.ResponseLogLevel.OnVerificationFailure,
            };

            Given()
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output, setting logging configuration
        /// in a request specification.
        /// </summary>
        [Test]
        public void ResponseBodyDetailsAreLoggedCorrectlyUsingRequestSpecificationSettings()
        {
            this.CreateStubForLoggingJsonResponse();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = Logging.ResponseLogLevel.All,
            };

            var requestSpecification = new RequestSpecBuilder()
                .WithLogConfiguration(logConfig)
                .Build();

            Given()
                .Spec(requestSpecification)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output, overwriting logging details from
        /// a request specification with specific settings.
        /// </summary>
        [Test]
        public void ResponseBodyDetailsAreLoggedCorrectlyOverwritingRequestSpecificationSettings()
        {
            this.CreateStubForLoggingJsonResponse();

            var originalLogConfig = new LogConfiguration
            {
                ResponseLogLevel = Logging.ResponseLogLevel.All,
            };

            var requestSpecification = new RequestSpecBuilder()
                .WithLogConfiguration(originalLogConfig)
                .Build();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = Logging.ResponseLogLevel.ResponseTime,
            };

            Given()
                .Spec(requestSpecification)
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details, including an HTTP-only, secure cookie.
        /// </summary>
        [Test]
        public void ResponseCookieDetailsAreLogged()
        {
            this.CreateStubForLoggingResponseWithCookie();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = Logging.ResponseLogLevel.All,
            };

            Given()
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-response-cookie")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for the JSON response body example.
        /// </summary>
        private void CreateStubForLoggingJsonResponse()
        {
            this.Server?.Given(Request.Create().WithPath("/log-json-response").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(this.GetLocation()));
        }

        /// <summary>
        /// Creates the stub response for the XML response body example.
        /// </summary>
        private void CreateStubForLoggingXmlResponse()
        {
            this.Server?.Given(Request.Create().WithPath("/log-xml-response").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/xml")
                .WithBody(this.GetLocationAsXmlString())
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

        /// <summary>
        /// Creates the stub response for the error response body example.
        /// </summary>
        private void CreateStubForErrorResponse()
        {
            this.Server?.Given(Request.Create().WithPath("/error-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(404)
                .WithHeader("Content-Type", "text/plain")
                .WithBody("The resource you requested was not found on this server."));
        }

        /// <summary>
        /// Creates the stub response for the response with cookie example.
        /// </summary>
        private void CreateStubForLoggingResponseWithCookie()
        {
            this.Server?.Given(Request.Create().WithPath("/log-response-cookie").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Set-Cookie", "Auth=123; httponly; secure")
                .WithHeader("Content-Type", "text/plain")
                .WithBody("Cookies are nice!"));
        }
    }
}