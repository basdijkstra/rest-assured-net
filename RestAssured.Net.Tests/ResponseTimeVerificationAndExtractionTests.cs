// <copyright file="ResponseTimeVerificationAndExtractionTests.cs" company="On Test Automation">
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
    using RestAssured.Response.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class ResponseTimeVerificationAndExtractionTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// the response time for a response.
        /// </summary>
        [Test]
        public void ResponseTimeCanBeVerified()
        {
            this.CreateStubForDelayedResponse();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/delayed-response")
                .Then()
                .StatusCode(200)
                .ResponseTime(NHamcrest.Is.GreaterThan(TimeSpan.FromMilliseconds(200)));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// the response time for a response.
        /// </summary>
        [Test]
        public void FailingResponseTimeVerificationThrowsTheExpectedException()
        {
            this.CreateStubForDelayedResponse();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/delayed-response")
                .Then()
                .StatusCode(200)
                .ResponseTime(NHamcrest.Is.LessThan(TimeSpan.FromMilliseconds(200)));
            });

            Assert.That(rve?.Message, Does.Contain("Expected response time to match 'less than 00:00:00.2000000' but was '"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting
        /// the response time for a response.
        /// </summary>
        [Test]
        public void ResponseTimeCanBeExtracted()
        {
            this.CreateStubForDelayedResponse();

            TimeSpan elapsedTime = Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/delayed-response")
                .Then()
                .StatusCode(200)
                .Extract().ResponseTime();

            // 1 means that the actual TimeSpan is longer than 200 milliseconds
            Assert.That(elapsedTime.CompareTo(TimeSpan.FromMilliseconds(200)), Is.EqualTo(1));
        }

        /// <summary>
        /// Creates the stub response for the response time extraction and verification examples.
        /// </summary>
        private void CreateStubForDelayedResponse()
        {
            this.Server?.Given(Request.Create().WithPath("/delayed-response").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithDelay(TimeSpan.FromMilliseconds(200)));
        }
    }
}