// <copyright file="StatusCodeVerificationTests.cs" company="On Test Automation">
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
using NUnit.Framework;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class StatusCodeVerificationTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a 200 response status code when performing an HTTP GET.
        /// </summary>
        [Test]
        public void StatusCodeIndicatingSuccessCanBeVerifiedAsInteger()
        {
            this.CreateStubForHttpOK();

            Given()
            .When()
            .Get("http://localhost:9876/http-status-code-ok")
            .Then()
            .AssertThat() // example of using the AssertThat() syntactic sugar method.
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a 404 response status code when performing an HTTP GET.
        /// </summary>
        [Test]
        public void StatusCodeIndicatingClientSideErrorCanBeVerifiedAsInteger()
        {
            this.CreateStubForHttpNotFound();

            Given()
            .When()
            .Get("http://localhost:9876/http-status-code-not-found")
            .Then()
            .StatusCode(404);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a 503 response status code when performing an HTTP GET.
        /// </summary>
        [Test]
        public void StatusCodeIndicatingServerSideErrorCanBeVerifiedAsInteger()
        {
            this.CreateStubForHttpServiceUnavailable();

            Given()
            .When()
            .Get("http://localhost:9876/http-status-code-service-unavailable")
            .Then()
            .StatusCode(503);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a 200 response status code when performing an HTTP GET.
        /// </summary>
        [Test]
        public void StatusCodeIndicatingSuccessCanBeVerifiedAsHttpStatusCode()
        {
            this.CreateStubForHttpOK();

            Given()
            .When()
            .Get("http://localhost:9876/http-status-code-ok")
            .Then()
            .StatusCode(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Creates the stub response for the HTTP OK example.
        /// </summary>
        private void CreateStubForHttpOK()
        {
            this.Server.Given(Request.Create().WithPath("/http-status-code-ok").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the HTTP Not Found example.
        /// </summary>
        private void CreateStubForHttpNotFound()
        {
            this.Server.Given(Request.Create().WithPath("/http-status-code-not-found").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(404));
        }

        /// <summary>
        /// Creates the stub response for the HTTP Service Unavailable example.
        /// </summary>
        private void CreateStubForHttpServiceUnavailable()
        {
            this.Server.Given(Request.Create().WithPath("/http-status-code-service-unavailable").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(503));
        }
    }
}