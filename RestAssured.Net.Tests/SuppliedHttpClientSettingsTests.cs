// <copyright file="SuppliedHttpClientSettingsTests.cs" company="On Test Automation">
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
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using NUnit.Framework;
    using RestAssured.Request.Builders;
    using RestAssured.Request.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Tests for settings that cannot be applied to an <see cref="HttpClient"/> supplied by the user
    /// (proxy, disabled SSL certificate validation and NTLM authentication).
    /// Set in the request itself, they cause an exception. Set in a request specification
    /// or in the global configuration, they cause a warning in the log.
    /// </summary>
    [TestFixture]
    public class SuppliedHttpClientSettingsTests : TestBase
    {
        private CollectingLogger logger;

        private HttpClient httpClient;

        /// <summary>
        /// Creates the logger, the user-supplied HttpClient and the stub used in the tests.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.logger = new CollectingLogger();
            this.httpClient = new HttpClient();

            this.Server?.Given(Request.Create().WithPath("/supplied-client").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Disposes the user-supplied HttpClient and resets the global configuration.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.httpClient.Dispose();
            RestAssuredConfig.DisableSslCertificateValidation = false;
        }

        /// <summary>
        /// Verifies that setting a proxy in the request throws an exception when a user-supplied HttpClient is used.
        /// </summary>
        [Test]
        public void ProxySetInRequestThrowsWithSuppliedHttpClient()
        {
            var rce = Assert.Throws<RequestCreationException>(() =>
                Given(this.logger, this.httpClient)
                    .Proxy(new WebProxy("http://localhost:1"))
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/supplied-client"));

            Assert.That(rce?.Message, Does.Contain("Proxy"));
            Assert.That(this.Server.LogEntries, Is.Empty, "The request should not have been sent.");
        }

        /// <summary>
        /// Verifies that disabling SSL certificate validation in the request throws an exception when a user-supplied HttpClient is used.
        /// </summary>
        [Test]
        public void DisabledSslValidationSetInRequestThrowsWithSuppliedHttpClient()
        {
            var rce = Assert.Throws<RequestCreationException>(() =>
                Given(this.logger, this.httpClient)
                    .DisableSslCertificateValidation()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/supplied-client"));

            Assert.That(rce?.Message, Does.Contain("DisableSslCertificateValidation"));
            Assert.That(this.Server.LogEntries, Is.Empty, "The request should not have been sent.");
        }

        /// <summary>
        /// Verifies that setting NTLM authentication in the request throws an exception when a user-supplied HttpClient is used.
        /// </summary>
        [Test]
        public void NtlmAuthSetInRequestThrowsWithSuppliedHttpClient()
        {
            var rce = Assert.Throws<RequestCreationException>(() =>
                Given(this.logger, this.httpClient)
                    .NtlmAuth("user", "password", "domain")
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/supplied-client"));

            Assert.That(rce?.Message, Does.Contain("NtlmAuth"));
            Assert.That(this.Server.LogEntries, Is.Empty, "The request should not have been sent.");
        }

        /// <summary>
        /// Verifies that all settings in the request that cannot be applied are named in the exception message.
        /// </summary>
        [Test]
        public void AllUnsupportedRequestSettingsAreNamed()
        {
            var rce = Assert.Throws<RequestCreationException>(() =>
                Given(this.logger, this.httpClient)
                    .Proxy(new WebProxy("http://localhost:1"))
                    .NtlmAuth("user", "password", "domain")
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/supplied-client"));

            Assert.That(rce?.Message, Does.Contain("Proxy"));
            Assert.That(rce?.Message, Does.Contain("NtlmAuth"));
        }

        /// <summary>
        /// Verifies that a proxy set in a request specification is reported as a warning when a user-supplied HttpClient is used.
        /// </summary>
        [Test]
        public void ProxyInRequestSpecificationIsReportedAsWarning()
        {
            var spec = new RequestSpecBuilder()
                .WithProxy(new WebProxy("http://localhost:1"))
                .Build();

            Given(this.logger, this.httpClient)
                .Spec(spec)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/supplied-client")
                .Then()
                .StatusCode(200);

            Assert.That(this.GetWarnings(), Has.Count.EqualTo(1));
            Assert.That(this.GetWarnings(), Has.Some.Contains("Proxy"));
        }

        /// <summary>
        /// Verifies that disabling SSL certificate validation in a request specification is reported as a warning
        /// when a user-supplied HttpClient is used.
        /// </summary>
        [Test]
        public void DisabledSslValidationInRequestSpecificationIsReportedAsWarning()
        {
            var spec = new RequestSpecBuilder()
                .WithDisabledSslCertificateValidation()
                .Build();

            Given(this.logger, this.httpClient)
                .Spec(spec)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/supplied-client")
                .Then()
                .StatusCode(200);

            Assert.That(this.GetWarnings(), Has.Count.EqualTo(1));
            Assert.That(this.GetWarnings(), Has.Some.Contains("DisableSslCertificateValidation"));
        }

        /// <summary>
        /// Verifies that disabling SSL certificate validation in the global configuration is reported as a warning
        /// when a user-supplied HttpClient is used.
        /// </summary>
        [Test]
        public void GlobalDisabledSslValidationIsReportedAsWarning()
        {
            RestAssuredConfig.DisableSslCertificateValidation = true;

            Given(this.logger, this.httpClient)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/supplied-client")
                .Then()
                .StatusCode(200);

            Assert.That(this.GetWarnings(), Has.Count.EqualTo(1));
            Assert.That(this.GetWarnings(), Has.Some.Contains("DisableSslCertificateValidation"));
        }

        /// <summary>
        /// Verifies that no warning is logged when a user-supplied HttpClient is used without settings that cannot be applied.
        /// </summary>
        [Test]
        public void NoWarningIsLoggedWithoutUnsupportedSettings()
        {
            Given(this.logger, this.httpClient)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/supplied-client")
                .Then()
                .StatusCode(200);

            Assert.That(this.logger.Messages, Is.Empty);
        }

        /// <summary>
        /// Returns the warning lines written to the logger.
        /// </summary>
        /// <returns>The warning lines.</returns>
        private List<string> GetWarnings()
        {
            return this.logger.Lines.Where(line => line.StartsWith("Warning:")).ToList();
        }
    }
}
