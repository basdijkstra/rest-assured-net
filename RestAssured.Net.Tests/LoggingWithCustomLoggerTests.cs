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
        private readonly string jsonBody = "{\"id\": 1, \"user\": \"John Doe\"}";

        /// <summary>
        /// Creates the WireMock stub used by all tests in this fixture.
        /// </summary>
        [SetUp]
        public void CreateStub()
        {
            this.Server?.Given(Request.Create()
                .WithPath("/custom-logger-test")
                .UsingAnyMethod())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(this.jsonBody));
        }

        /// <summary>
        /// Verifies that the request endpoint line is captured by a custom logger.
        /// </summary>
        [Test]
        public void RequestEndpointIsWrittenToCustomLogger()
        {
            var collector = new CollectingLogger();

            Given(collector)
                .Log(new LogConfiguration { RequestLogLevel = RequestLogLevel.Endpoint })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(collector.Messages, Has.Some.Contains("GET"));
            Assert.That(collector.Messages, Has.Some.Contains("/custom-logger-test"));
        }

        /// <summary>
        /// Verifies that request headers are written to a custom logger.
        /// </summary>
        [Test]
        public void RequestHeadersAreWrittenToCustomLogger()
        {
            var collector = new CollectingLogger();

            Given(collector)
                .Log(new LogConfiguration { RequestLogLevel = RequestLogLevel.All })
                .Header("X-Custom-Header", "custom-value")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(collector.Messages, Has.Some.Contains("X-Custom-Header: custom-value"));
        }

        /// <summary>
        /// Verifies that a sensitive request header value is masked when written to a custom logger.
        /// </summary>
        [Test]
        public void SensitiveRequestHeaderIsMaskedInCustomLogger()
        {
            var collector = new CollectingLogger();

            Given(collector)
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

            Assert.That(collector.Messages, Has.Some.Contains("Authorization: *****"));
            Assert.That(collector.Messages, Has.None.Contains("supersecret"));
        }

        /// <summary>
        /// Verifies that the response status code is written to a custom logger.
        /// </summary>
        [Test]
        public void ResponseStatusCodeIsWrittenToCustomLogger()
        {
            var collector = new CollectingLogger();

            Given(collector)
                .Log(new LogConfiguration { ResponseLogLevel = ResponseLogLevel.Headers })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(collector.Messages, Has.Some.Contains("HTTP 200"));
        }

        /// <summary>
        /// Verifies that the response body is written to a custom logger.
        /// </summary>
        [Test]
        public void ResponseBodyIsWrittenToCustomLogger()
        {
            var collector = new CollectingLogger();

            Given(collector)
                .Log(new LogConfiguration { ResponseLogLevel = ResponseLogLevel.All })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(collector.Messages, Has.Some.Contains("John Doe"));
        }

        /// <summary>
        /// Verifies that a request body is written to a custom logger when logging level is set to Body.
        /// </summary>
        [Test]
        public void RequestBodyIsWrittenToCustomLogger()
        {
            var collector = new CollectingLogger();

            Given(collector)
                .Log(new LogConfiguration { RequestLogLevel = RequestLogLevel.Body })
                .ContentType("application/json")
                .Body(this.jsonBody)
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(collector.Messages, Has.Some.Contains("John Doe"));
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
        /// Verifies that the request endpoint line is captured by a logger set on a RequestSpecification.
        /// </summary>
        [Test]
        public void RequestEndpointIsWrittenToLoggerFromRequestSpecification()
        {
            var collector = new CollectingLogger();

            var spec = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithLogConfiguration(new LogConfiguration { RequestLogLevel = RequestLogLevel.Endpoint })
                .WithLogger(collector)
                .Build();

            Given()
                .Spec(spec)
                .When()
                .Get("/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(collector.Messages, Has.Some.Contains("GET"));
            Assert.That(collector.Messages, Has.Some.Contains("/custom-logger-test"));
        }

        /// <summary>
        /// Verifies that request headers are captured by a logger set on a RequestSpecification.
        /// </summary>
        [Test]
        public void RequestHeadersAreWrittenToLoggerFromRequestSpecification()
        {
            var collector = new CollectingLogger();

            var spec = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithLogConfiguration(new LogConfiguration { RequestLogLevel = RequestLogLevel.All })
                .WithLogger(collector)
                .Build();

            Given()
                .Spec(spec)
                .Header("X-Custom-Header", "custom-value")
                .When()
                .Get("/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(collector.Messages, Has.Some.Contains("X-Custom-Header: custom-value"));
        }

        /// <summary>
        /// Verifies that the response body is captured by a logger set on a RequestSpecification.
        /// </summary>
        [Test]
        public void ResponseBodyIsWrittenToLoggerFromRequestSpecification()
        {
            var collector = new CollectingLogger();

            var spec = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithLogConfiguration(new LogConfiguration { ResponseLogLevel = ResponseLogLevel.All })
                .WithLogger(collector)
                .Build();

            Given()
                .Spec(spec)
                .When()
                .Get("/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(collector.Messages, Has.Some.Contains("John Doe"));
        }

        /// <summary>
        /// Verifies that an explicit Given(logger) takes precedence over a logger set on a RequestSpecification.
        /// </summary>
        [Test]
        public void ExplicitGivenLoggerTakesPrecedenceOverRequestSpecificationLogger()
        {
            var specCollector = new CollectingLogger();
            var givenCollector = new CollectingLogger();

            var spec = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithLogConfiguration(new LogConfiguration { RequestLogLevel = RequestLogLevel.Endpoint })
                .WithLogger(specCollector)
                .Build();

            Given(givenCollector)
                .Spec(spec)
                .When()
                .Get("/custom-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(givenCollector.Messages, Has.Some.Contains("GET"));
            Assert.That(specCollector.Messages, Is.Empty);
        }

        /// <summary>
        /// Verifies that response details are written to a custom logger on verification failure.
        /// </summary>
        [Test]
        public void ResponseIsWrittenToCustomLoggerOnVerificationFailure()
        {
            var collector = new CollectingLogger();

            Assert.Throws<ResponseVerificationException>(() =>
                Given(collector)
                    .Log(new LogConfiguration { ResponseLogLevel = ResponseLogLevel.OnVerificationFailure })
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/custom-logger-test")
                    .Then()
                    .StatusCode(500));

            Assert.That(collector.Messages, Has.Some.Contains("HTTP 200"));
        }
    }
}
