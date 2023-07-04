// <copyright file="CookieExtractionTests.cs" company="On Test Automation">
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
    using RestAssured.Response.Exceptions;
    using RestAssured.Response.Logging;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class CookieExtractionTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a cookie with a single value when sending an HTTP request.
        /// </summary>
        [Test]
        public void HttpOnlySecureCookieValueCanBeExtracted()
        {
            this.CreateStubForHttpOnlySecureCookie();

            string authCookieValue = Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-httponly-secure-cookie")
                .Then()
                .Log(ResponseLogLevel.All)
                .StatusCode(200)
                .And()
                .Extract().Cookie("Auth");

            Assert.That(authCookieValue, Is.EqualTo("123"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a cookie with a single value when sending an HTTP request.
        /// </summary>
        [Test]
        public void CookieNotFoundThrowsTheExpectedException()
        {
            this.CreateStubForHttpOnlySecureCookie();

            ExtractionException ee = Assert.Throws<ExtractionException>(() =>
            {
                Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-httponly-secure-cookie")
                .Then()
                .Log(ResponseLogLevel.All)
                .StatusCode(200)
                .And()
                .Extract().Cookie("not_found");
            });

            Assert.That(ee.Message, Is.EqualTo("Cookie with name 'not_found' could not be found in the response."));
        }

        /// <summary>
        /// Creates the stub returning a response with an HTTP-only secure response cookie.
        /// </summary>
        private void CreateStubForHttpOnlySecureCookie()
        {
            this.Server?.Given(Request.Create().WithPath("/response-with-httponly-secure-cookie").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Set-Cookie", "Auth=123; httponly; secure")
                .WithStatusCode(200));
        }
    }
}