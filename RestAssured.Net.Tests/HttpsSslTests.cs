// <copyright file="HttpsSslTests.cs" company="On Test Automation">
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
    using System.Net;
    using NUnit.Framework;
    using RestAssured.Request.Builders;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class HttpsSslTests : TestBaseHttps
    {
        private RequestSpecification requestSpecification;

        /// <summary>
        /// Creates the RequestSpecification before each test in this class.
        /// </summary>
        [SetUp]
        public void CreateRequestSpecification()
        {
            this.requestSpecification = new RequestSpecBuilder()
                .WithDisabledSslCertificateValidation()
                .Build();
        }

        /// <summary>
        /// A test demonstrating that RestAssured.Net is able to invoke
        /// HTTPS endpoint with a valid server-side certificate without having
        /// to disable SSL checks.
        /// </summary>
        [Test]
        public void HttpsEndpointsCanBeInvokedWithoutExplicitlyDisablingSslVerification()
        {
            Given()
                .When()
                .Get("https://api.zippopotam.us/us/90210")
                .Then()
                .StatusCode(HttpStatusCode.OK)
                .And()
                .Body("$.country", NHamcrest.Is.EqualTo("United States"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for disabling
        /// SSL verification when performing an HTTP call.
        /// </summary>
        [Test]
        public void SslVerificationCanBeDisabledPerRequest()
        {
            this.CreateStubForHttps();

            Given()
                .DisableSslCertificateValidation()
                .When()
                .Get("https://localhost:8443/ssl-endpoint")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for disabling
        /// SSL verification using the deprecated method when performing an HTTP call.
        /// </summary>
        [Test]
        public void SslVerificationCanBeDisabledPerRequestUsingDeprecatedMethod()
        {
            this.CreateStubForHttps();

            Given()
                .DisableSslCertificateValidation()
                .When()
                .Get("https://localhost:8443/ssl-endpoint")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for disabling
        /// SSL verification through the RequestSpecification
        /// when performing an HTTP call.
        /// </summary>
        [Test]
        public void SslVerificationCanBeDisabledThroughRequestSpecification()
        {
            this.CreateStubForHttps();

            Given()
                .Spec(this.requestSpecification!)
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