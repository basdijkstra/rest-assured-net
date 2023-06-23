// <copyright file="TimeoutTests.cs" company="On Test Automation">
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
    using NUnit.Framework;
    using RestAssured.Request.Builders;
    using RestAssured.Request.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class TimeoutTests : TestBase
    {
        private RequestSpecification requestSpecification;

        /// <summary>
        /// Creates the <see cref="RequestSpecification"/> instances to be used in the tests in this class.
        /// </summary>
        [SetUp]
        public void CreateRequestSpecifications()
        {
            this.requestSpecification = new RequestSpecBuilder()
                .WithScheme("http")
                .WithHostName("localhost")
                .WithPort(9876)
                .WithTimeout(TimeSpan.FromSeconds(2))
                .Build();
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for specifying
        /// a custom timeout for a specific request when sending an HTTP request.
        /// </summary>
        [Test]
        public void CustomTimeoutCanBeSuppliedInTest()
        {
            this.CreateStubForTimeoutOk();

            Given()
                .Timeout(TimeSpan.FromSeconds(2))
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/timeout-ok")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for specifying
        /// a custom timeout in a request specification when sending an HTTP request.
        /// </summary>
        [Test]
        public void CustomTimeoutCanBeSuppliedInRequestSpecification()
        {
            this.CreateStubForTimeoutOk();

            Given()
                .Spec(this.requestSpecification!)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/timeout-ok")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// Test that exceeding a timeout specified in a test
        /// throws the expected exception.
        /// </summary>
        [Test]
        public void TimeoutInTestExceededThrowsTheExpectedException()
        {
            this.CreateStubForTimeoutNok();

            var hrpe = Assert.Throws<HttpRequestProcessorException>(() =>
            {
                Given()
                    .Timeout(TimeSpan.FromSeconds(2))
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/timeout-nok")
                    .Then()
                    .StatusCode(200);
            });

            Assert.That(hrpe?.Message, Is.EqualTo("Request timeout of 00:00:02 exceeded."));
        }

        /// <summary>
        /// Test that exceeding a timeout specified in a request specification
        /// throws the expected exception.
        /// </summary>
        [Test]
        public void TimeoutInRequestSpecificationExceededThrowsTheExpectedException()
        {
            this.CreateStubForTimeoutNok();

            var hrpe = Assert.Throws<HttpRequestProcessorException>(() =>
            {
                Given()
                    .Spec(this.requestSpecification!)
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/timeout-nok")
                    .Then()
                    .StatusCode(200);
            });

            Assert.That(hrpe?.Message, Is.EqualTo("Request timeout of 00:00:02 exceeded."));
        }

        /// <summary>
        /// Creates the stub response for the form data example.
        /// </summary>
        private void CreateStubForTimeoutOk()
        {
            this.Server?.Given(Request.Create().WithPath("/timeout-ok").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the form data example.
        /// </summary>
        private void CreateStubForTimeoutNok()
        {
            this.Server?.Given(Request.Create().WithPath("/timeout-nok").UsingGet())
                .RespondWith(Response.Create()
                .WithDelay(TimeSpan.FromSeconds(3))
                .WithStatusCode(200));
        }
    }
}