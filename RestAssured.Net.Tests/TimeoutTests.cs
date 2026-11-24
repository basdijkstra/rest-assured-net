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
    using System.Net.Http;
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
                .WithBaseUri("http://localhost")
                .WithPort(MOCK_SERVER_PORT)
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
        /// A test checking that a timeout set in the request takes precedence
        /// over a (shorter) timeout set in the request specification.
        /// </summary>
        [Test]
        public void TimeoutInTestTakesPrecedenceOverTimeoutInRequestSpecification()
        {
            this.CreateStubForShortTimeoutNok();

            var shortTimeoutSpecification = new RequestSpecBuilder()
                .WithTimeout(TimeSpan.FromMilliseconds(200))
                .Build();

            Given()
                .Spec(shortTimeoutSpecification)
                .Timeout(TimeSpan.FromSeconds(5))
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/short-timeout-nok")
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
        /// A test checking that a timeout set in the request or in a request specification
        /// does not change the timeout of an <see cref="HttpClient"/> supplied by the user,
        /// and can be used with a client that has already sent a request
        /// (changing the timeout of such a client would throw an <see cref="InvalidOperationException"/>).
        /// </summary>
        /// <param name="timeoutInSpec">True to set the timeout in a request specification, false to set it in the request.</param>
        [TestCase(false, TestName = "Timeout set in request does not change supplied HttpClient")]
        [TestCase(true, TestName = "Timeout set in request specification does not change supplied HttpClient")]
        public void TimeoutDoesNotChangeSuppliedHttpClient(bool timeoutInSpec)
        {
            this.CreateStubForTimeoutOk();

            using HttpClient httpClient = new HttpClient();
            TimeSpan originalTimeout = httpClient.Timeout;

            // Use the client once, so that changing its timeout would no longer be allowed.
            Given(httpClient)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/timeout-ok")
                .Then()
                .StatusCode(200);

            if (timeoutInSpec)
            {
                var spec = new RequestSpecBuilder()
                    .WithTimeout(TimeSpan.FromSeconds(1))
                    .Build();

                Given(httpClient)
                    .Spec(spec)
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/timeout-ok")
                    .Then()
                    .StatusCode(200);
            }
            else
            {
                Given(httpClient)
                    .Timeout(TimeSpan.FromSeconds(1))
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/timeout-ok")
                    .Then()
                    .StatusCode(200);
            }

            Assert.That(httpClient.Timeout, Is.EqualTo(originalTimeout));
        }

        /// <summary>
        /// A test checking that a timeout set in the request is applied when using an <see cref="HttpClient"/> supplied by the user.
        /// </summary>
        [Test]
        public void TimeoutIsAppliedWithSuppliedHttpClient()
        {
            this.CreateStubForShortTimeoutNok();

            using HttpClient httpClient = new HttpClient();

            var hrpe = Assert.Throws<HttpRequestProcessorException>(() =>
                Given(httpClient)
                    .Timeout(TimeSpan.FromMilliseconds(200))
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/short-timeout-nok"));

            Assert.That(hrpe?.Message, Is.EqualTo("Request timeout of 00:00:00.2000000 exceeded."));
        }

        /// <summary>
        /// A test checking that the (shorter) timeout of an <see cref="HttpClient"/> supplied by the user
        /// applies and is reported when it is exceeded, both when no timeout was set in the request
        /// and when a longer timeout was set in the request.
        /// </summary>
        /// <param name="requestTimeoutInSeconds">The timeout to set in the request, or null to not set one.</param>
        [TestCase(null, TestName = "Timeout of supplied HttpClient is reported when no timeout is set in request")]
        [TestCase(10, TestName = "Shorter timeout of supplied HttpClient takes precedence over timeout set in request")]
        public void TimeoutOfSuppliedHttpClientIsReportedWhenExceeded(int? requestTimeoutInSeconds)
        {
            this.CreateStubForShortTimeoutNok();

            using HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromMilliseconds(200) };

            var request = Given(httpClient);

            if (requestTimeoutInSeconds != null)
            {
                request = request.Timeout(TimeSpan.FromSeconds((int)requestTimeoutInSeconds));
            }

            var hrpe = Assert.Throws<HttpRequestProcessorException>(() =>
                request
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/short-timeout-nok"));

            Assert.That(hrpe?.Message, Is.EqualTo("Request timeout of 00:00:00.2000000 exceeded."));
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

        /// <summary>
        /// Creates the stub response with a delay of 1 second, for tests with a short timeout.
        /// </summary>
        private void CreateStubForShortTimeoutNok()
        {
            this.Server?.Given(Request.Create().WithPath("/short-timeout-nok").UsingGet())
                .RespondWith(Response.Create()
                .WithDelay(TimeSpan.FromSeconds(1))
                .WithStatusCode(200));
        }
    }
}