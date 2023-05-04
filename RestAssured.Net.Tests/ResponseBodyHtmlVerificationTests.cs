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
        /// an HTML response body element using an NHamcrest matcher.
        /// </summary>
        [Test]
        public void HtmlResponseBodyElementValueCanBeVerifiedUsingNHamcrestMatcher()
        {
            this.CreateStubForHtmlResponseBody();

            Given()
                .When()
                .Get("http://localhost:9876/html-response-body")
                .Then()
                .StatusCode(404)
                .Body(NHamcrest.Is.EqualTo(this.GetHtmlResponseBody()));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting
        /// an HTML response.
        /// </summary>
        [Test]
        public void HtmlResponseCanBeExtracted()
        {
            this.CreateStubForHtmlResponseBody();

            HttpResponseMessage response =

            Given()
                .When()
                .Get("http://localhost:9876/html-response-body")
                .Then()
                .StatusCode(404)
                .Extract().Response();

            Assert.That(response.Content.ReadAsStringAsync().Result, Is.EqualTo(this.GetHtmlResponseBody()));
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
    }
}