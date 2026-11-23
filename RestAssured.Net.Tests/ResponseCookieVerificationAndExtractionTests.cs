// <copyright file="ResponseCookieVerificationAndExtractionTests.cs" company="On Test Automation">
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
    using System.Net.Http;
    using NUnit.Framework;
    using RestAssured.Response.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class ResponseCookieVerificationAndExtractionTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a generic cookie in a response.
        /// </summary>
        [Test]
        public void GenericCookieValueCanBeVerified()
        {
            this.CreateStubForGenericCookie();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-generic-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Cookie("my_cookie", NHamcrest.Is.EqualTo("my_value"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// an HTTP-only, secure cookie in a response.
        /// </summary>
        [Test]
        public void HttpOnlySecureCookieValueCanBeVerified()
        {
            this.CreateStubForHttpOnlySecureCookie();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-httponly-secure-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Cookie("Auth", NHamcrest.Is.EqualTo("123"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting
        /// a generic cookie from a response.
        /// </summary>
        [Test]
        public void GenericCookieValueCanBeExtracted()
        {
            this.CreateStubForGenericCookie();

            string cookieValue = Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-generic-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Extract().Cookie("my_cookie");

            Assert.That(cookieValue, Is.EqualTo("my_value"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting
        /// an HTTP-only, secure cookie from a response.
        /// </summary>
        [Test]
        public void HttpOnlySecureCookieValueCanBeExtracted()
        {
            this.CreateStubForHttpOnlySecureCookie();

            string authCookieValue = Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-httponly-secure-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Extract().Cookie("Auth");

            Assert.That(authCookieValue, Is.EqualTo("123"));
        }

        /// <summary>
        /// A test checking that a cookie value verification mismatch
        /// throws the expected exception.
        /// </summary>
        [Test]
        public void CookieValueMismatchThrowsTheExpectedExceptionWhenVerifying()
        {
            this.CreateStubForGenericCookie();

            ResponseVerificationException rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-generic-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Cookie("my_cookie", NHamcrest.Is.EqualTo("some_other_value"));
            });

            Assert.That(rve.Message, Is.EqualTo("Expected value for cookie with name 'my_cookie' to match '\"some_other_value\"', but was 'my_value'."));
        }

        /// <summary>
        /// A test checking that a cookie not being found when verifying
        /// throws the expected exception.
        /// </summary>
        [Test]
        public void CookieNotFoundThrowsTheExpectedExceptionWhenVerifying()
        {
            this.CreateStubForGenericCookie();

            ResponseVerificationException rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-generic-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Cookie("not_found", NHamcrest.Is.EqualTo("my_value"));
            });

            Assert.That(rve.Message, Is.EqualTo("Cookie with name 'not_found' could not be found in the response."));
        }

        /// <summary>
        /// A test checking that a cookie not being found when extracting
        /// throws the expected exception.
        /// </summary>
        [Test]
        public void CookieNotFoundThrowsTheExpectedExceptionWhenExtracting()
        {
            this.CreateStubForHttpOnlySecureCookie();

            ExtractionException ee = Assert.Throws<ExtractionException>(() =>
            {
                Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-httponly-secure-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Extract().Cookie("not_found");
            });

            Assert.That(ee.Message, Is.EqualTo("Cookie with name 'not_found' could not be found in the response."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a cookie value using a templated custom error message,
        /// that the exception contains the correctly formatted message.
        /// </summary>
        [Test]
        public void TemplatedCustomErrorMessageCanBeSpecifiedWhenVerifyingCookieValue()
        {
            this.CreateStubForGenericCookie();

            ResponseVerificationException rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-generic-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Cookie("my_cookie", NHamcrest.Is.EqualTo("wrong_value"), "Expected [expected] but got [actual]");
            });

            Assert.That(rve.Message, Is.EqualTo("Expected \"wrong_value\" but got my_value"));
        }

        /// <summary>
        /// A test checking that a cookie that was sent with the request,
        /// but not set by the server, is not found when verifying response cookies.
        /// </summary>
        [Test]
        public void RequestCookieIsNotReportedAsResponseCookieWhenVerifying()
        {
            this.CreateStubForResponseWithoutCookie();

            ResponseVerificationException rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                .Cookie("session", "abc")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-without-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Cookie("session", NHamcrest.Is.EqualTo("abc"));
            });

            Assert.That(rve.Message, Is.EqualTo("Cookie with name 'session' could not be found in the response."));
        }

        /// <summary>
        /// A test checking that a cookie set by the server overwrites
        /// a cookie with the same name that was sent with the request.
        /// </summary>
        [Test]
        public void ResponseCookieOverwritingRequestCookieCanBeVerified()
        {
            this.CreateStubForGenericCookie();

            Given()
                .Cookie("my_cookie", "old_value")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-generic-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Cookie("my_cookie", NHamcrest.Is.EqualTo("my_value"));
        }

        /// <summary>
        /// A test checking that a cookie set by the server is found, even when
        /// a cookie with the same name and value was sent with the request.
        /// </summary>
        [Test]
        public void ResponseCookieWithSameNameAndValueAsRequestCookieCanBeVerified()
        {
            this.CreateStubForGenericCookie();

            Given()
                .Cookie("my_cookie", "my_value")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-generic-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Cookie("my_cookie", NHamcrest.Is.EqualTo("my_value"));
        }

        /// <summary>
        /// A test checking that a cookie set by the server on a redirect response
        /// is found when verifying the cookies of the final response.
        /// </summary>
        [Test]
        public void CookieSetDuringRedirectCanBeVerified()
        {
            this.CreateStubForResponseWithoutCookie();
            this.CreateStubForRedirectWithCookie();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/redirect-with-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Cookie("redirect_cookie", NHamcrest.Is.EqualTo("redirect_value"));
        }

        /// <summary>
        /// A test checking that a cookie set by the server can be verified
        /// when the request is sent using a custom <see cref="HttpClient"/>.
        /// </summary>
        [Test]
        public void ResponseCookieCanBeVerifiedWithCustomHttpClient()
        {
            this.CreateStubForGenericCookie();

            using HttpClient httpClient = new HttpClient();

            Given(httpClient)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/response-with-generic-cookie")
                .Then()
                .StatusCode(200)
                .And()
                .Cookie("my_cookie", NHamcrest.Is.EqualTo("my_value"));
        }

        /// <summary>
        /// Creates the stub returning a response with a generic response cookie.
        /// </summary>
        private void CreateStubForGenericCookie()
        {
            this.Server?.Given(Request.Create().WithPath("/response-with-generic-cookie").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Set-Cookie", "my_cookie=my_value")
                .WithStatusCode(200));
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

        /// <summary>
        /// Creates the stub returning a response without any response cookies.
        /// </summary>
        private void CreateStubForResponseWithoutCookie()
        {
            this.Server?.Given(Request.Create().WithPath("/response-without-cookie").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub returning a redirect response that sets a cookie.
        /// </summary>
        private void CreateStubForRedirectWithCookie()
        {
            this.Server?.Given(Request.Create().WithPath("/redirect-with-cookie").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Set-Cookie", "redirect_cookie=redirect_value")
                .WithHeader("Location", "/response-without-cookie")
                .WithStatusCode(302));
        }
    }
}