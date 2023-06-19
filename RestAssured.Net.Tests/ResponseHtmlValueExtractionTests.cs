// <copyright file="ResponseHtmlValueExtractionTests.cs" company="On Test Automation">
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
    using RestAssured.Response;
    using RestAssured.Response.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class ResponseHtmlValueExtractionTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting an
        /// element value from an HTML response body.
        /// </summary>
        [Test]
        public void HtmlResponseBodyElementValueCanBeExtracted()
        {
            this.CreateStubForHtmlResponseBody();

            string title = (string)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/html-response-body")
                .Then()
                .StatusCode(404)
                .Extract().Body("//title");

            Assert.That(title, Is.EqualTo("403 - Forbidden: Access is denied."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting an
        /// element value from an HTML response body.
        /// </summary>
        [Test]
        public void HtmlResponseBodyElementValueCanBeExtractedOverridingResponseContentType()
        {
            this.CreateStubForHtmlResponseBodyWithResponseContentTypeHeaderMismatch();

            string title = (string)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/html-response-body-header-mismatch")
                .Then()
                .StatusCode(404)
                .Extract().Body("//title", ExtractAs.Html);

            Assert.That(title, Is.EqualTo("403 - Forbidden: Access is denied."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a
        /// list of element values from an XML response body.
        /// </summary>
        [Test]
        public void HtmlResponseBodyMultipleElementsCanBeExtracted()
        {
            this.CreateStubForHtmlResponseBody();

            // At least for now, if you want to retrieve multiple
            // HTML response body element values, they will have to be
            // stored in an object of type List<string>.
            List<string> fields = (List<string>)Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/html-response-body")
                .Then()
                .StatusCode(404)
                .Extract().Body("//fieldset/*");

            Assert.That(fields.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that the expected exception is thrown when the XPath does not return results.
        /// </summary>
        [Test]
        public void NoXPathResultsThrowsTheExpectedException()
        {
            this.CreateStubForHtmlResponseBody();

            var ee = Assert.Throws<ExtractionException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/html-response-body")
                    .Then()
                    .StatusCode(404)
                    .Extract().Body("//DoesNotExist");
            });

            Assert.That(ee?.Message, Is.EqualTo("XPath expression '//DoesNotExist' did not yield any results."));
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
        /// Creates the stub response for the HTML response body example.
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