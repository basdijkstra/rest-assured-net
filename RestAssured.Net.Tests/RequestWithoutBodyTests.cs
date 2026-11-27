// <copyright file="RequestWithoutBodyTests.cs" company="On Test Automation">
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
    /// Tests checking which content is sent for requests with and without a body.
    /// </summary>
    [TestFixture]
    public class RequestWithoutBodyTests : TestBase
    {
        /// <summary>
        /// Creates the stub that accepts every request to the endpoint used in this class.
        /// </summary>
        [SetUp]
        public void CreateStub()
        {
            this.Server?.Given(Request.Create().WithPath("/no-body"))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// A test checking that a request without a body is sent without content,
        /// and therefore without a Content-Type header, for every HTTP method.
        /// </summary>
        /// <param name="method">The HTTP method to use.</param>
        [TestCase("GET")]
        [TestCase("POST")]
        public void RequestWithoutBodySendsNoContentHeaders(string method)
        {
            Given()
                .When()
                .Invoke($"{MOCK_SERVER_BASE_URL}/no-body", new HttpMethod(method))
                .Then()
                .StatusCode(200);

            Assert.That(this.GetLastRequestHeaderValues("Content-Type"), Is.Empty);
        }

        /// <summary>
        /// A test checking that an explicitly empty body is sent as content,
        /// with the default Content-Type header.
        /// </summary>
        [Test]
        public void ExplicitEmptyBodyIsSentWithContentType()
        {
            Given()
                .Body(string.Empty)
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/no-body")
                .Then()
                .StatusCode(200);

            Assert.That(this.GetLastRequestHeaderValues("Content-Type"), Is.EqualTo(new[] { "application/json; charset=utf-8" }));
        }
    }
}
