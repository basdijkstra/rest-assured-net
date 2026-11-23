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

        /// <summary>
        /// Creates the logger and the stub used in the tests.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.logger = new CollectingLogger();

            this.Server?.Given(Request.Create().WithPath("/supplied-client").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Resets the global configuration.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            RestAssuredConfig.DisableSslCertificateValidation = false;
        }

        /// <summary>
        /// Verifies that proxy, disabled SSL certificate validation and NTLM authentication set in the request
        /// throw an exception that names all of them when a user-supplied HttpClient is used, and that the request is not sent.
        /// </summary>
        [Test]
        public void UnsupportedRequestSettingsThrowWithSuppliedHttpClient()
        {
            using HttpClient httpClient = new HttpClient();

            var rce = Assert.Throws<RequestCreationException>(() =>
                Given(this.logger, httpClient)
                    .Proxy(new WebProxy("http://localhost:1"))
                    .DisableSslCertificateValidation()
                    .NtlmAuth("user", "password", "domain")
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/supplied-client"));

            Assert.That(rce?.Message, Does.Contain("Proxy"));
            Assert.That(rce?.Message, Does.Contain("DisableSslCertificateValidation"));
            Assert.That(rce?.Message, Does.Contain("NtlmAuth"));
            Assert.That(this.Server.LogEntries, Is.Empty, "The request should not have been sent.");
        }

        /// <summary>
        /// Verifies that a proxy and disabled SSL certificate validation set in a request specification
        /// are reported in a single warning when a user-supplied HttpClient is used, and that the request is still sent.
        /// </summary>
        [Test]
        public void UnsupportedRequestSpecificationSettingsAreReportedAsWarning()
        {
            using HttpClient httpClient = new HttpClient();

            var spec = new RequestSpecBuilder()
                .WithProxy(new WebProxy("http://localhost:1"))
                .WithDisabledSslCertificateValidation()
                .Build();

            Given(this.logger, httpClient)
                .Spec(spec)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/supplied-client")
                .Then()
                .StatusCode(200);

            Assert.That(this.GetWarnings(), Has.Count.EqualTo(1));
            Assert.That(this.GetWarnings()[0], Does.Contain("Proxy (from RequestSpecification)"));
            Assert.That(this.GetWarnings()[0], Does.Contain("DisableSslCertificateValidation (from RequestSpecification)"));
        }

        /// <summary>
        /// Verifies that disabling SSL certificate validation in the global configuration is reported as a warning
        /// when a user-supplied HttpClient is used.
        /// </summary>
        [Test]
        public void GlobalDisabledSslValidationIsReportedAsWarning()
        {
            using HttpClient httpClient = new HttpClient();

            RestAssuredConfig.DisableSslCertificateValidation = true;

            Given(this.logger, httpClient)
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
            using HttpClient httpClient = new HttpClient();

            Given(this.logger, httpClient)
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
