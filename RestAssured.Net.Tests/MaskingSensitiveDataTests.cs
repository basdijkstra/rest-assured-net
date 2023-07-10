// <copyright file="MaskingSensitiveDataTests.cs" company="On Test Automation">
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
    using NUnit.Framework;
    using RestAssured.Request.Logging;
    using RestAssured.Response.Logging;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class MaskingSensitiveDataTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for masking
        /// sensitive headers when logging request details.
        /// </summary>
        [Test]
        public void SingleSensitiveRequestHeaderIsMasked()
        {
            this.CreateStubForMaskingSensitiveData();

            Given()
                .Log(RequestLogLevel.All)
                .Mask("SensitiveRequestHeader")
                .And()
                .Header("NonsensitiveRequestHeader", "This one is printed")
                .Header("SensitiveRequestHeader", "This one is masked")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/masking-sensitive-data")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for masking
        /// sensitive headers when logging response details.
        /// </summary>
        [Test]
        public void SingleSensitiveResponseHeaderIsMasked()
        {
            this.CreateStubForMaskingSensitiveData();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/masking-sensitive-data")
                .Then()
                .Mask(new List<string>() { "SensitiveResponseHeader", "SensitiveResponseCookie" })
                .Log(ResponseLogLevel.All)
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for masking
        /// sensitive cookies when logging request details.
        /// </summary>
        [Test]
        public void SingleSensitiveRequestCookieIsMasked()
        {
            this.CreateStubForMaskingSensitiveData();

            Given()
                .Log(RequestLogLevel.All)
                .Mask("SensitiveRequestCookie")
                .And()
                .Cookie("NonsensitiveRequestCookie", "This one is printed")
                .Cookie("SensitiveRequestCookie", "This one is masked")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/masking-sensitive-data")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for masking
        /// sensitive headers when logging request details.
        /// </summary>
        [Test]
        public void MultipleSensitiveRequestHeadersAreMasked()
        {
            this.CreateStubForMaskingSensitiveData();

            Given()
                .Log(RequestLogLevel.All)
                .Mask(new List<string>() { "SensitiveRequestHeader", "AnotherSensitiveRequestHeader" })
                .And()
                .Header("NonsensitiveRequestHeader", "This one is printed")
                .Header("SensitiveRequestHeader", "This one is masked")
                .Header("AnotherSensitiveRequestHeader", "This one is masked as well")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/masking-sensitive-data")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for the masking sensitive data example.
        /// </summary>
        private void CreateStubForMaskingSensitiveData()
        {
            this.Server?.Given(Request.Create().WithPath("/masking-sensitive-data").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Set-Cookie", "NonSensitiveResponseCookie=123")
                .WithHeader("Set-Cookie", "SensitiveResponseCookie=some_secret_value")
                .WithHeader("NonSensitiveResponseHeader", "This one is printed")
                .WithHeader("SensitiveResponseHeader", "This one is masked")
                .WithHeader("Content-Type", "text/plain")
                .WithBody("Response data"));
        }
    }
}