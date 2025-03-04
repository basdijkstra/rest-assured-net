// <copyright file="StaticConfigurationTests.cs" company="On Test Automation">
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
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class StaticConfigurationTests : TestBaseHttps
    {
        /// <summary>
        /// Set the RestAssured.Net configuration once before all tests.
        /// </summary>
        [OneTimeSetUp]
        public void SetRestAssuredNetConfiguration()
        {
            var logConfig = new LogConfiguration
            {
                RequestLogLevel = RequestLogLevel.All,
            };

            RestAssuredConfig.LogConfiguration = logConfig;
            RestAssuredConfig.DisableSslCertificateValidation = true;
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for disabling
        /// SSL verification when performing an HTTP call.
        /// </summary>
        [Test]
        [Repeat(3)]
        public void SslVerificationCanBeDisabledUsingStaticConfiguration()
        {
            this.CreateStubForHttps();

            Given()
                .When()
                .Get("https://localhost:8443/ssl-endpoint")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for the HTTPS example.
        /// </summary>
        private void CreateStubForHttps()
        {
            this.Server?.Given(Request.Create().WithPath("/ssl-endpoint").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}