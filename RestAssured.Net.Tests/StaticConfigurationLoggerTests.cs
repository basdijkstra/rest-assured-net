// <copyright file="StaticConfigurationLoggerTests.cs" company="On Test Automation">
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
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Tests verifying that a logger set on the global <see cref="RestAssured.Configuration.RestAssuredConfiguration"/>
    /// is used when writing log output for all requests.
    /// </summary>
    [TestFixture]
    public class StaticConfigurationLoggerTests : TestBase
    {
        private readonly string jsonBody = "{\"id\": 1, \"user\": \"John Doe\"}";

        /// <summary>
        /// Reset the global logger after every test so that it does not bleed into other test fixtures.
        /// </summary>
        [TearDown]
        public void ResetStaticConfiguration()
        {
            RestAssuredConfig.Logger = null;
            RestAssuredConfig.LogConfiguration = null;
        }

        /// <summary>
        /// Creates the WireMock stub used by all tests in this fixture.
        /// </summary>
        [SetUp]
        public void CreateStub()
        {
            this.Server?.Given(Request.Create()
                .WithPath("/static-config-logger-test")
                .UsingAnyMethod())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(this.jsonBody));
        }

        /// <summary>
        /// Verifies that a logger set on the global configuration captures the request endpoint line.
        /// </summary>
        [Test]
        public void RequestEndpointIsWrittenToLoggerFromStaticConfiguration()
        {
            var collector = new CollectingLogger();

            RestAssuredConfig.Logger = collector;
            RestAssuredConfig.LogConfiguration = new LogConfiguration { RequestLogLevel = RequestLogLevel.Endpoint };

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/static-config-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(collector.Messages, Has.Some.Contains("GET"));
            Assert.That(collector.Messages, Has.Some.Contains("/static-config-logger-test"));
        }

        /// <summary>
        /// Verifies that a logger set on the global configuration captures the response body.
        /// </summary>
        [Test]
        public void ResponseBodyIsWrittenToLoggerFromStaticConfiguration()
        {
            var collector = new CollectingLogger();

            RestAssuredConfig.Logger = collector;
            RestAssuredConfig.LogConfiguration = new LogConfiguration { ResponseLogLevel = ResponseLogLevel.All };

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/static-config-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(collector.Messages, Has.Some.Contains("John Doe"));
        }

        /// <summary>
        /// Verifies that an explicit Given(logger) takes precedence over the global configuration logger.
        /// </summary>
        [Test]
        public void ExplicitGivenLoggerTakesPrecedenceOverStaticConfigurationLogger()
        {
            var configCollector = new CollectingLogger();
            var givenCollector = new CollectingLogger();

            RestAssuredConfig.Logger = configCollector;
            RestAssuredConfig.LogConfiguration = new LogConfiguration { RequestLogLevel = RequestLogLevel.Endpoint };

            Given(givenCollector)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/static-config-logger-test")
                .Then()
                .StatusCode(200);

            Assert.That(givenCollector.Messages, Has.Some.Contains("GET"));
            Assert.That(configCollector.Messages, Is.Empty);
        }
    }
}
