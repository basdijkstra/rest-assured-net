// <copyright file="LoggingWithCustomLoggerTests.cs" company="On Test Automation">
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
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RestAssured.Logging;
    using RestAssured.Request.Builders;
    using RestAssured.Response.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Tests verifying that log output can be captured and asserted on via a custom <see cref="IRestAssuredNetLogger"/>.
    /// </summary>
    [TestFixture]
    public class LoggingWithCustomLoggerTests : TestBase
    {
        private RequestSpecification requestSpecification;
        private CollectingLogger logger;

        /// <summary>
        /// Creates the WireMock stub used by all tests in this fixture.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.logger = new CollectingLogger();

            this.requestSpecification = new RequestSpecBuilder()
                .WithLogger(this.logger)
                .Build();

            this.Server?.Given(Request.Create()
                .WithPath("/custom-logger-test")
                .UsingAnyMethod())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(new { id = 1, user = "John Doe" }));
        }

        /// <summary>
        /// Verifies that request and response are captured for logging in their entirety, each in their own entry.
        /// </summary>
        [Test]
        public void RequestAndResponseAreCapturedAsTwoLoggingEntries()
        {
            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration()
                {
                    RequestLogLevel = RequestLogLevel.All,
                    ResponseLogLevel = ResponseLogLevel.All,
                })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Verifies that the request endpoint line is captured by a custom logger.
        /// </summary>
        [Test]
        public void RequestEndpointIsWrittenToCustomLogger()
        {
            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration { RequestLogLevel = RequestLogLevel.Endpoint })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("GET"));
            Assert.That(this.logger.Messages, Has.Some.Contains("/custom-logger-test"));
        }

        /// <summary>
        /// Verifies that request headers are written to a custom logger.
        /// </summary>
        [Test]
        public void RequestHeadersAreWrittenToCustomLogger()
        {
            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration { RequestLogLevel = RequestLogLevel.All })
                .Header("X-Custom-Header", "custom-value")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("X-Custom-Header: custom-value"));
        }

        /// <summary>
        /// Verifies that a sensitive request header value is masked when written to a custom logger.
        /// </summary>
        [Test]
        public void SensitiveRequestHeaderIsMaskedInCustomLogger()
        {
            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration
                {
                    RequestLogLevel = RequestLogLevel.All,
                    SensitiveRequestHeadersAndCookies = new List<string> { "Authorization" },
                })
                .Header("Authorization", "Bearer supersecret")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("Authorization: *****"));
            Assert.That(this.logger.Messages, Has.None.Contains("supersecret"));
        }

        /// <summary>
        /// Verifies that a cookie sent with the request, but not set by the server,
        /// is written to the request log and not to the response log.
        /// </summary>
        [Test]
        public void RequestCookieIsNotLoggedAsResponseCookie()
        {
            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration()
                {
                    RequestLogLevel = RequestLogLevel.All,
                    ResponseLogLevel = ResponseLogLevel.All,
                })
                .Cookie("request_cookie", "request_cookie_value")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages.Count, Is.EqualTo(2));
            Assert.That(this.logger.Messages[0], Does.Contain("request_cookie_value"));
            Assert.That(this.logger.Messages[1], Does.Not.Contain("request_cookie_value"));
        }

        /// <summary>
        /// Verifies that sensitive request headers are masked when the configured names differ in case from the header names.
        /// </summary>
        [Test]
        public void SensitiveRequestHeaderIsMaskedRegardlessOfCase()
        {
            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration
                {
                    RequestLogLevel = RequestLogLevel.All,
                    SensitiveRequestHeadersAndCookies = new List<string> { "authorization", "x-api-key" },
                })
                .OAuth2("SUPER-SECRET-TOKEN")
                .Header("X-Api-Key", "SECRET-API-KEY")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("Authorization: *****"));
            Assert.That(this.logger.Messages, Has.Some.Contains("X-Api-Key: *****"));
            Assert.That(this.logger.Messages, Has.None.Contains("SUPER-SECRET-TOKEN"));
            Assert.That(this.logger.Messages, Has.None.Contains("SECRET-API-KEY"));
        }

        /// <summary>
        /// Verifies that sensitive response headers are masked when the configured names differ in case from the header names.
        /// </summary>
        [Test]
        public void SensitiveResponseHeaderIsMaskedRegardlessOfCase()
        {
            this.CreateStubForMaskingResponse();

            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration
                {
                    ResponseLogLevel = ResponseLogLevel.Headers,
                    SensitiveResponseHeadersAndCookies = new List<string> { "x-secret-header" },
                })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/masking-response")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("X-Secret-Header: *****"));
            Assert.That(this.logger.Messages, Has.None.Contains("SECRET-HEADER-VALUE"));
        }

        /// <summary>
        /// Verifies that the value of a sensitive cookie is masked in the Set-Cookie response header.
        /// </summary>
        [Test]
        public void SensitiveResponseCookieIsMaskedInSetCookieHeader()
        {
            this.CreateStubForMaskingResponse();

            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration
                {
                    ResponseLogLevel = ResponseLogLevel.Headers,
                    SensitiveResponseHeadersAndCookies = new List<string> { "session" },
                })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/masking-response")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("session=*****; Path=/"));
            Assert.That(this.logger.Messages, Has.None.Contains("TOPSECRET123"));
        }

        /// <summary>
        /// Verifies that the value of a cookie that is not marked as sensitive is not masked in the Set-Cookie response header.
        /// </summary>
        [Test]
        public void NonSensitiveResponseCookieIsNotMaskedInSetCookieHeader()
        {
            this.CreateStubForMaskingResponse();

            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration
                {
                    ResponseLogLevel = ResponseLogLevel.Headers,
                    SensitiveResponseHeadersAndCookies = new List<string> { "session" },
                })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/masking-response")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("other=visible"));
        }

        /// <summary>
        /// Verifies that sensitive cookies are masked when the configured name differs in case from the cookie name.
        /// </summary>
        [Test]
        public void SensitiveCookieIsMaskedRegardlessOfCase()
        {
            this.CreateStubForMaskingResponse();

            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration
                {
                    ResponseLogLevel = ResponseLogLevel.Headers,
                    SensitiveResponseHeadersAndCookies = new List<string> { "Session" },
                })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/masking-response")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("session=*****; Path=/"));
            Assert.That(this.logger.Messages, Has.Some.Contains("Cookie: session=*****"));
            Assert.That(this.logger.Messages, Has.None.Contains("TOPSECRET123"));
        }

        /// <summary>
        /// Verifies that the value of a sensitive cookie is masked in a Cookie request header,
        /// and that other cookies in the same header are not masked.
        /// </summary>
        [Test]
        public void SensitiveRequestCookieIsMaskedInCookieHeader()
        {
            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration
                {
                    RequestLogLevel = RequestLogLevel.Headers,
                    SensitiveRequestHeadersAndCookies = new List<string> { "session" },
                })
                .Header("Cookie", "session=REQUEST-SECRET; other=1")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("Cookie: session=*****; other=1"));
            Assert.That(this.logger.Messages, Has.None.Contains("REQUEST-SECRET"));
        }

        /// <summary>
        /// Verifies that the response status code is written to a custom logger.
        /// </summary>
        [Test]
        public void ResponseStatusCodeIsWrittenToCustomLogger()
        {
            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration { ResponseLogLevel = ResponseLogLevel.Headers })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("HTTP 200"));
        }

        /// <summary>
        /// Verifies that the response body is written to a custom logger.
        /// </summary>
        [Test]
        public void ResponseBodyIsWrittenToCustomLogger()
        {
            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration { ResponseLogLevel = ResponseLogLevel.All })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("John Doe"));
        }

        /// <summary>
        /// Verifies that a request body is written to a custom logger when logging level is set to Body.
        /// </summary>
        [Test]
        public void RequestBodyIsWrittenToCustomLogger()
        {
            Given()
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration { RequestLogLevel = RequestLogLevel.Body })
                .ContentType("application/json")
                .Body(new { id = 1, name = "John Doe" })
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Has.Some.Contains("John Doe"));
        }

        /// <summary>
        /// Verifies that passing a null logger to Given throws an ArgumentNullException.
        /// </summary>
        [Test]
        public void GivenWithNullLoggerThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Given((IRestAssuredNetLogger)null!));
        }

        /// <summary>
        /// Verifies that passing a null logger to Given throws an ArgumentNullException.
        /// </summary>
        [Test]
        public void PassingCustomLoggerThroughGivenIsPossible()
        {
            var loggerPassedInGiven = new CollectingLogger();

            Given(loggerPassedInGiven)
                .Spec(this.requestSpecification)
                .Log(new LogConfiguration { RequestLogLevel = RequestLogLevel.Endpoint })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(loggerPassedInGiven.Messages, Has.Some.Contains("GET"));
            Assert.That(loggerPassedInGiven.Messages, Has.Some.Contains("/custom-logger-test"));
        }

        /// <summary>
        /// Verifies that response details are written to a custom logger on verification failure.
        /// </summary>
        [Test]
        public void ResponseIsWrittenToCustomLoggerOnVerificationFailure()
        {
            Assert.Throws<ResponseVerificationException>(() =>
                Given()
                    .Spec(this.requestSpecification)
                    .Log(new LogConfiguration { ResponseLogLevel = ResponseLogLevel.OnVerificationFailure })
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                    .Then()
                    .StatusCode(500));

            Assert.That(this.logger.Messages, Has.Some.Contains("HTTP 200"));
        }

        /// <summary>
        /// Creates the stub returning a response with a sensitive header and two cookies.
        /// </summary>
        private void CreateStubForMaskingResponse()
        {
            this.Server?.Given(Request.Create()
                .WithPath("/masking-response")
                .UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("X-Secret-Header", "SECRET-HEADER-VALUE")
                .WithHeader("Set-Cookie", "session=TOPSECRET123; Path=/")
                .WithHeader("Set-Cookie", "other=visible"));
        }
    }
}
