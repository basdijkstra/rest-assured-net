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
    using RestAssured.Response.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage, focusing on logging request and response details.
    /// The log output is captured with a <see cref="CollectingLogger"/>, which also writes it to the standard output.
    /// </summary>
    [TestFixture]
    public class LoggingTests : TestBase
    {
        private const string ResponseTimePattern = @"^Response time: \d+(\.\d+)? ms$";

        private readonly string jsonBody = "{\"id\": 1, \"user\": \"John Doe\"}";

        private CollectingLogger logger;

        /// <summary>
        /// Creates the logger that captures the log output of each test.
        /// </summary>
        [SetUp]
        public void CreateLogger()
        {
            this.logger = new CollectingLogger();
        }

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
                RequestLogLevel = RequestLogLevel.All,
            };

            Given(this.logger)
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

            // Only the request is logged, with the endpoint, all headers and the pretty-printed body.
            Assert.That(this.logger.Messages, Has.Count.EqualTo(1));
            Assert.That(this.logger.Lines, Has.Member($"GET {MOCK_SERVER_BASE_URL}/log-json-response"));
            Assert.That(this.logger.Lines, Has.Member("Content-Type: application/json; charset=utf-8"));
            Assert.That(this.logger.Lines, Has.Member($"Content-Length: {this.jsonBody.Length}"));
            Assert.That(this.logger.Lines, Has.Member("Accept: application/json"));
            Assert.That(this.logger.Lines, Has.Member("CustomHeader: custom header value"));
            Assert.That(this.logger.Lines, Has.Member("  \"id\": 1,"));
            Assert.That(this.logger.Lines, Has.Member("  \"user\": \"John Doe\""));
            Assert.That(this.logger.Lines, Has.None.StartsWith("HTTP "));
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
                RequestLogLevel = RequestLogLevel.All,
            };

            Given(this.logger)
                .Log(logConfig)
                .ContentType("application/xml")
                .Body(this.GetLocationAsXmlString())
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-xml-response")
                .Then()
                .StatusCode(200);

            // The XML request body is pretty-printed.
            Assert.That(this.logger.Lines, Has.Member("Content-Type: application/xml; charset=utf-8"));
            Assert.That(this.logger.Lines, Has.Member("  <Country>United States</Country>"));
            Assert.That(this.logger.Lines, Has.Member("      <Name>Sun City</Name>"));
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
                ResponseLogLevel = ResponseLogLevel.All,
            };

            Given(this.logger)
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);

            // Only the response is logged, with the status, content headers, pretty-printed body and response time.
            Assert.That(this.logger.Messages, Has.Count.EqualTo(1));
            Assert.That(this.logger.Lines, Has.Member("HTTP 200 (OK)"));
            Assert.That(this.logger.Lines, Has.Member("Content-Type: application/json"));
            Assert.That(this.logger.Lines, Has.Some.Match(@"^Content-Length: \d+$"));
            Assert.That(this.logger.Lines, Has.Member("  \"Country\": \"United States\","));
            Assert.That(this.logger.Lines, Has.Some.Match(ResponseTimePattern));
            Assert.That(this.logger.Lines, Has.None.StartsWith("GET "));
        }

        /// <summary>
        /// A test verifying that the response time is logged using the invariant culture,
        /// also when the current culture uses a different decimal separator.
        /// </summary>
        [Test]
        [SetCulture("nl-NL")]
        public void ResponseTimeIsLoggedUsingInvariantCulture()
        {
            this.CreateStubForLoggingJsonResponse();

            Given(this.logger)
                .Log(new LogConfiguration { ResponseLogLevel = ResponseLogLevel.ResponseTime })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Lines, Has.Some.Match(ResponseTimePattern));
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
                ResponseLogLevel = ResponseLogLevel.All,
            };

            Given(this.logger)
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-xml-response")
                .Then()
                .StatusCode(200);

            // The XML response body is pretty-printed.
            Assert.That(this.logger.Lines, Has.Member("HTTP 200 (OK)"));
            Assert.That(this.logger.Lines, Has.Member("Content-Type: application/xml"));
            Assert.That(this.logger.Lines, Has.Member("  <Country>United States</Country>"));
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
                ResponseLogLevel = ResponseLogLevel.All,
            };

            Given(this.logger)
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-no-response-body")
                .Then()
                .StatusCode(200);

            // An empty body is not logged at all.
            Assert.That(this.logger.Lines, Has.Member("HTTP 200 (OK)"));
            Assert.That(this.logger.Lines, Has.Member("Content-Length: 0"));
            Assert.That(this.logger.Lines, Has.None.StartsWith("Content-Type:"), "A response without a Content-Type header is logged without a Content-Type line.");
            Assert.That(this.logger.Lines, Has.None.Contains("Could not read payload"));
            Assert.That(this.logger.Lines, Has.None.Empty);
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
                RequestLogLevel = RequestLogLevel.All,
            };

            Given(this.logger)
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-no-response-body")
                .Then()
                .StatusCode(200);

            // An empty request body is not logged at all.
            Assert.That(this.logger.Lines, Has.Member($"GET {MOCK_SERVER_BASE_URL}/log-no-response-body"));
            Assert.That(this.logger.Lines, Has.None.Contains("Could not read payload"));
            Assert.That(this.logger.Lines, Has.None.Empty);
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
                ResponseLogLevel = ResponseLogLevel.OnError,
            };

            Given(this.logger)
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/error-response-body")
                .Then()
                .StatusCode(404);

            Assert.That(this.logger.Messages, Has.Count.EqualTo(1));
            Assert.That(this.logger.Lines, Has.Member("HTTP 404 (NotFound)"));
            Assert.That(this.logger.Lines, Has.Member("The resource you requested was not found on this server."));
        }

        /// <summary>
        /// A test checking that response details are logged for status code 400,
        /// the lowest status code that is considered an error.
        /// </summary>
        [Test]
        public void ResponseDetailsAreLoggedOnErrorForStatusCode400()
        {
            this.Server?.Given(Request.Create().WithPath("/bad-request").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(400));

            Given(this.logger)
                .Log(new LogConfiguration { ResponseLogLevel = ResponseLogLevel.OnError })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/bad-request")
                .Then()
                .StatusCode(400);

            Assert.That(this.logger.Lines, Has.Member("HTTP 400 (BadRequest)"));
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
                ResponseLogLevel = ResponseLogLevel.OnError,
            };

            Given(this.logger)
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Is.Empty);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for logging
        /// response details to the standard output only if a verification fails.
        /// This test should log details, because the expected status code (201)
        /// does not match the actual status code (200).
        /// </summary>
        [Test]
        public void ResponseBodyDetailsAreLoggedIfVerificationFails()
        {
            this.CreateStubForLoggingJsonResponse();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = ResponseLogLevel.OnVerificationFailure,
            };

            Assert.Throws<ResponseVerificationException>(() =>
                Given(this.logger)
                    .Log(logConfig)
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                    .Then()
                    .StatusCode(201));

            Assert.That(this.logger.Lines, Has.Member("HTTP 200 (OK)"));
            Assert.That(this.logger.Lines, Has.Member("  \"Country\": \"United States\","));
        }

        /// <summary>
        /// A test checking that response details are not logged
        /// when logging on verification failure and all verifications pass.
        /// </summary>
        [Test]
        public void ResponseBodyDetailsAreNotLoggedIfVerificationPasses()
        {
            this.CreateStubForLoggingJsonResponse();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = ResponseLogLevel.OnVerificationFailure,
            };

            Given(this.logger)
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Is.Empty);
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
                ResponseLogLevel = ResponseLogLevel.All,
            };

            var requestSpecification = new RequestSpecBuilder()
                .WithLogConfiguration(logConfig)
                .Build();

            Given(this.logger)
                .Spec(requestSpecification)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);

            // The log level from the request specification is applied.
            Assert.That(this.logger.Lines, Has.Member("HTTP 200 (OK)"));
            Assert.That(this.logger.Lines, Has.Member("  \"Country\": \"United States\","));
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
                ResponseLogLevel = ResponseLogLevel.All,
            };

            var requestSpecification = new RequestSpecBuilder()
                .WithLogConfiguration(originalLogConfig)
                .Build();

            var logConfig = new LogConfiguration
            {
                ResponseLogLevel = ResponseLogLevel.ResponseTime,
            };

            Given(this.logger)
                .Spec(requestSpecification)
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);

            // The log level set in the request (response time only) overrides the one in the request specification (all).
            Assert.That(this.logger.Lines, Has.Some.Match(ResponseTimePattern));
            Assert.That(this.logger.Lines, Has.None.StartsWith("Content-Type:"));
            Assert.That(this.logger.Lines, Has.None.Contains("United States"));
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
                ResponseLogLevel = ResponseLogLevel.All,
            };

            Given(this.logger)
                .Log(logConfig)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-response-cookie")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Lines, Has.Member("Set-Cookie: Auth=123; httponly; secure"));
            Assert.That(this.logger.Lines, Has.Member("Cookie: Auth=123, Domain: localhost, HTTP-only: True, Secure: True"));
            Assert.That(this.logger.Lines, Has.Member("Cookies are nice!"));
        }

        /// <summary>
        /// A test checking that content headers other than Content-Type and Content-Length
        /// are included when response headers are logged.
        /// </summary>
        [Test]
        public void ResponseContentHeadersAreLogged()
        {
            this.Server?.Given(Request.Create().WithPath("/log-content-headers").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "text/plain")
                .WithHeader("Content-Disposition", "attachment; filename=a.txt")
                .WithBody("Hello, world!"));

            Given(this.logger)
                .Log(new LogConfiguration { ResponseLogLevel = ResponseLogLevel.Headers })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-content-headers")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Lines, Has.Member("Content-Disposition: attachment; filename=a.txt"));
        }

        /// <summary>
        /// A test checking that a request cookie without a domain is logged
        /// with the domain it is sent for: the host of the request.
        /// </summary>
        [Test]
        public void RequestCookieIsLoggedWithDomainOfRequest()
        {
            this.CreateStubForLoggingJsonResponse();

            Given(this.logger)
                .Log(new LogConfiguration { RequestLogLevel = RequestLogLevel.All })
                .Cookie("session", "abc")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/log-json-response")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Lines, Has.Member("Cookie: session=abc, Domain: localhost, HTTP-only: False, Secure: False"));
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