// <copyright file="ResponseBodyHtmlVerificationTests.cs" company="On Test Automation">
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
    using RestAssured.Response;
    using RestAssured.Response.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class ResponseBodyHtmlVerificationTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// an HTML response body using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void HtmlResponseBodyCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForHtmlResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/html-response-body")
                .Then()
                .StatusCode(404)
                .Body(NHamcrest.Is.EqualTo(this.GetHtmlResponseBody()));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// an HTML response body element using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void HtmlResponseBodyElementCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForHtmlResponseBody();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/html-response-body")
                .Then()
                .StatusCode(404)
                .Body("//title", NHamcrest.Is.EqualTo("403 - Forbidden: Access is denied."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// an HTML response body element using an NHamcrest matcher,
        /// overriding the response Content-Type header value.
        /// </summary>
        [Test]
        public void HtmlResponseBodyElementCanBeVerifiedUsingNHamcrestMatcherOverridingResponseContentTypeHeader()
        {
            this.CreateStubForHtmlResponseBodyWithResponseContentTypeHeaderMismatch();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/html-response-body-header-mismatch")
                .Then()
                .StatusCode(404)
                .Body("//title", NHamcrest.Is.EqualTo("403 - Forbidden: Access is denied."), VerifyAs.Html);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the expected exception is thrown when the XPath does not return results.
        /// </summary>
        [Test]
        public void NoXPathResultsThrowsTheExpectedException()
        {
            this.CreateStubForHtmlResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/html-response-body")
                    .Then()
                    .StatusCode(404)
                    .Body("//DoesNotExist", NHamcrest.Is.EqualTo("Some value"));
            });

            Assert.That(rve?.Message, Is.EqualTo("XPath expression '//DoesNotExist' did not yield any results."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the expected exception is thrown when the element value returned
        /// by the XPath does not match the matcher type.
        /// </summary>
        [Test]
        public void ElementValueNotMatchingMatcherTypeThrowsTheExpectedException()
        {
            this.CreateStubForHtmlResponseBody();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/html-response-body")
                    .Then()
                    .StatusCode(404)
                    .Body("//title", NHamcrest.Is.GreaterThanOrEqualTo(100));
            });

            Assert.That(rve?.Message, Is.EqualTo("Response element value 403 - Forbidden: Access is denied. cannot be converted to value of type 'System.Int32'"));
        }

        /// <summary>
        /// Creates the stub response for the HTML response body example.
        /// </summary>
        private void CreateStubForHtmlResponseBody()
        {
            this.Server?.Given(Request.Create().WithPath("/html-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "text/html")
                .WithBody(this.GetHtmlResponseBody())
                .WithStatusCode(404));
        }

        /// <summary>
        /// Creates the stub response for the HTML response body example with a
        /// non-matching response Content-Type header value.
        /// </summary>
        private void CreateStubForHtmlResponseBodyWithResponseContentTypeHeaderMismatch()
        {
            this.Server?.Given(Request.Create().WithPath("/html-response-body-header-mismatch").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "text/plain")
                .WithBody(this.GetHtmlResponseBody())
                .WithStatusCode(404));
        }
    }
}