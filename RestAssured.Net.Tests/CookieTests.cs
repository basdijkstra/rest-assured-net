// <copyright file="CookieTests.cs" company="On Test Automation">
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
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class CookieTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a cookie with a single value when sending an HTTP request.
        /// </summary>
        [Test]
        public void CookieAsStringWithASingleValueCanBeSupplied()
        {
            this.CreateStubForSingleCookieValue();

            Given()
                .Cookie("my_cookie", "my_cookie_value")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/single-cookie-value")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a <see cref="Cookie"/> with a single value when sending an HTTP request.
        /// </summary>
        [Test]
        public void CookieObjectWithASingleValueCanBeSupplied()
        {
            this.CreateStubForSingleCookieValue();

            Given()
                .Cookie(new Cookie("my_cookie", "my_cookie_value"))
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/single-cookie-value")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a <see cref="Cookie"/> with a single value when sending an HTTP request.
        /// </summary>
        [Test]
        public void CookieCollectionWithASingleCookieCanBeSupplied()
        {
            this.CreateStubForSingleCookieValue();

            CookieCollection cookies = new CookieCollection()
            {
                new Cookie("my_cookie", "my_cookie_value"),
            };

            Given()
                .Cookie(cookies)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/single-cookie-value")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for the single cookie value example.
        /// </summary>
        private void CreateStubForSingleCookieValue()
        {
            this.Server?.Given(Request.Create().WithPath("/single-cookie-value").UsingGet()
                .WithCookie("my_cookie", "my_cookie_value"))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}