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
    using System;
    using System.Net;
    using System.Net.Http;
    using NUnit.Framework;
    using RestAssured.Request.Cookies;
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
        /// Verify that cookie domain is properly set when it hasn't been specified already.
        /// </summary>
        [Test]
        public void CookieDomainIsSetToDefaultValueWhenNotSpecified()
        {
            Cookie cookie = new Cookie("cookie_name", "cookie_value");

            CookieUtils cookieUtils = new CookieUtils();

            cookie = cookieUtils.CopyWithDomain(cookie, "localhost");

            Assert.That(cookie.Domain, Is.EqualTo("localhost"));
        }

        /// <summary>
        /// Verify that cookie domain is not updated when it has been specified already.
        /// </summary>
        [Test]
        public void CookieDomainIsUnchangedWhenSpecifiedAlready()
        {
            Cookie cookie = new Cookie("cookie_name", "cookie_value", "/my_path", "strawberry.com");

            CookieUtils cookieUtils = new CookieUtils();

            cookie = cookieUtils.CopyWithDomain(cookie, "localhost");

            Assert.That(cookie.Domain, Is.EqualTo("strawberry.com"));
        }

        /// <summary>
        /// Verify that a copied cookie keeps all properties that affect sending it, and is a different object.
        /// </summary>
        [Test]
        public void CopiedCookieKeepsAllProperties()
        {
            Cookie cookie = new Cookie("cookie_name", "cookie_value", "/my_path", "strawberry.com")
            {
                Secure = true,
                HttpOnly = true,
                Expires = new DateTime(2030, 1, 2, 3, 4, 5, DateTimeKind.Utc),
                Version = 1,
                Comment = "A comment",
                CommentUri = new Uri("http://strawberry.com/comment"),
                Discard = true,
                Port = "\"8080\"",
            };

            Cookie copy = new CookieUtils().CopyWithDomain(cookie, "localhost");

            Assert.That(copy, Is.Not.SameAs(cookie));
            Assert.That(copy.Name, Is.EqualTo(cookie.Name));
            Assert.That(copy.Value, Is.EqualTo(cookie.Value));
            Assert.That(copy.Path, Is.EqualTo(cookie.Path));
            Assert.That(copy.Domain, Is.EqualTo(cookie.Domain));
            Assert.That(copy.Secure, Is.EqualTo(cookie.Secure));
            Assert.That(copy.HttpOnly, Is.EqualTo(cookie.HttpOnly));
            Assert.That(copy.Expires, Is.EqualTo(cookie.Expires));
            Assert.That(copy.Version, Is.EqualTo(cookie.Version));
            Assert.That(copy.Comment, Is.EqualTo(cookie.Comment));
            Assert.That(copy.CommentUri, Is.EqualTo(cookie.CommentUri));
            Assert.That(copy.Discard, Is.EqualTo(cookie.Discard));
            Assert.That(copy.Port, Is.EqualTo(cookie.Port));
        }

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
        /// Verify that multiple request cookies are sent when the user supplies their own <see cref="HttpClient"/>.
        /// </summary>
        [Test]
        public void MultipleRequestCookiesAreSentWithSuppliedHttpClient()
        {
            using HttpClient httpClient = new HttpClient();

            CookieCollection cookies = new CookieCollection()
            {
                new Cookie("a", "1"),
                new Cookie("b", "2"),
            };

            Given(httpClient)
                .Cookie(cookies)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/cookies");

            Assert.That(this.GetLastRequestCookies(), Is.EquivalentTo(new[] { "a=1", "b=2" }));
        }

        /// <summary>
        /// Verify that request cookies are combined with the cookies that a user-supplied <see cref="HttpClient"/> sends itself.
        /// </summary>
        [Test]
        public void RequestCookiesAreCombinedWithCookiesOfSuppliedHttpClient()
        {
            CookieContainer cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri(MOCK_SERVER_BASE_URL), new Cookie("own", "1"));

            using HttpClient httpClient = new HttpClient(new HttpClientHandler { CookieContainer = cookieContainer });

            Given(httpClient)
                .Cookie("ra", "2")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/cookies");

            Assert.That(this.GetLastRequestCookies(), Is.EquivalentTo(new[] { "own=1", "ra=2" }));
        }

        /// <summary>
        /// Verify that request cookies are combined with a Cookie header set in the request
        /// when the user supplies their own <see cref="HttpClient"/>.
        /// </summary>
        [Test]
        public void RequestCookiesAreCombinedWithCookieHeaderSetInRequest()
        {
            using HttpClient httpClient = new HttpClient();

            Given(httpClient)
                .Header("Cookie", "x=1")
                .Cookie("y", "2")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/cookies");

            Assert.That(this.GetLastRequestHeaderValues("Cookie"), Has.Count.EqualTo(1));
            Assert.That(this.GetLastRequestCookies(), Is.EquivalentTo(new[] { "x=1", "y=2" }));
        }

        /// <summary>
        /// Verify that a secure request cookie is not sent over HTTP when the user supplies their own <see cref="HttpClient"/>,
        /// in the same way as when RestAssured.Net creates the <see cref="HttpClient"/>.
        /// </summary>
        [Test]
        public void SecureRequestCookieIsNotSentOverHttpWithSuppliedHttpClient()
        {
            using HttpClient httpClient = new HttpClient();

            Given(httpClient)
                .Cookie(new Cookie("s", "1") { Secure = true })
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/cookies");

            Assert.That(this.GetLastRequestHeaderValues("Cookie"), Is.Empty);
        }

        /// <summary>
        /// Verify that a request cookie is sent exactly once when RestAssured.Net creates the <see cref="HttpClient"/>.
        /// </summary>
        [Test]
        public void RequestCookieIsSentOnceWithLibraryHttpClient()
        {
            Given()
                .Cookie("a", "1")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/cookies");

            Assert.That(this.GetLastRequestHeaderValues("Cookie"), Is.EqualTo(new[] { "a=1" }));
        }

        /// <summary>
        /// Verify that a <see cref="Cookie"/> object supplied by the user is not modified when the request is sent,
        /// so that it can be reused in requests to other hosts.
        /// </summary>
        [Test]
        public void SuppliedCookieIsNotModifiedBySendingRequest()
        {
            Cookie cookie = new Cookie("a", "1");

            Given()
                .Cookie(cookie)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/cookies");

            Assert.That(this.GetLastRequestHeaderValues("Cookie"), Is.EqualTo(new[] { "a=1" }));
            Assert.That(cookie.Domain, Is.Empty);
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