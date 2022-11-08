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
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NUnit.Framework;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class TimeoutTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for specifying
        /// a custom timeout when sending an HTTP request.
        /// </summary>
        [Test]
        public void CustomTimeoutCanBeSupplied()
        {
            this.CreateStubForTimeoutOk();

            Given()
            .Timeout(TimeSpan.FromSeconds(2))
            .When()
            .Get("http://localhost:9876/timeout-ok")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for specifying
        /// a custom timeout when sending an HTTP request.
        /// </summary>
        [Test]
        public void TimeoutExceededThrowsTheExpectedException()
        {
            this.CreateStubForTimeoutNok();

            RA.Exceptions.HttpRequestProcessorException hrpe = Assert.Throws<RA.Exceptions.HttpRequestProcessorException>(() =>
            {
                Given()
                .Timeout(TimeSpan.FromSeconds(2))
                .When()
                .Get("http://localhost:9876/timeout-nok")
                .Then()
                .StatusCode(200);
            });

            Assert.That(hrpe.Message, Is.EqualTo("Request timeout of 00:00:02 exceeded."));
        }

        /// <summary>
        /// Creates the stub response for the form data example.
        /// </summary>
        private void CreateStubForTimeoutOk()
        {
            this.Server.Given(Request.Create().WithPath("/timeout-ok").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the form data example.
        /// </summary>
        private void CreateStubForTimeoutNok()
        {
            this.Server.Given(Request.Create().WithPath("/timeout-nok").UsingGet())
                .RespondWith(Response.Create()
                .WithDelay(TimeSpan.FromSeconds(3))
                .WithStatusCode(200));
        }
    }
}