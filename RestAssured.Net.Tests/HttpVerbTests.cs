// <copyright file="HttpVerbTests.cs" company="On Test Automation">
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
    using NUnit.Framework;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class HttpVerbTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a response status code when performing an HTTP GET.
        /// </summary>
        [Test]
        public void HttpGetCanBeUsed()
        {
            this.CreateStubForHttpGet();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/http-get");
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a response status code when performing an HTTP POST.
        /// </summary>
        [Test]
        public void HttpPostCanBeUsed()
        {
            this.CreateStubForHttpPost();

            Given()
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/http-post");
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a response status code when performing an HTTP PUT.
        /// </summary>
        [Test]
        public void HttpPutCanBeUsed()
        {
            this.CreateStubForHttpPut();

            Given()
                .When()
                .Put($"{MOCK_SERVER_BASE_URL}/http-put");
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a response status code when performing an HTTP PATCH.
        /// </summary>
        [Test]
        public void HttpPatchCanBeUsed()
        {
            this.CreateStubForHttpPatch();

            Given()
                .When()
                .Patch($"{MOCK_SERVER_BASE_URL}/http-patch");
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a response status code when performing an HTTP DELETE.
        /// </summary>
        [Test]
        public void HttpDeleteCanBeUsed()
        {
            this.CreateStubForHttpDelete();

            Given()
                .When()
                .Delete($"{MOCK_SERVER_BASE_URL}/http-delete");
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a response status code when performing an HTTP HEAD.
        /// </summary>
        [Test]
        public void HttpHeadCanBeUsed()
        {
            this.CreateStubForHttpHead();

            Given()
                .When()
                .Head($"{MOCK_SERVER_BASE_URL}/http-head");
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a response status code when performing an HTTP OPTIONS.
        /// </summary>
        [Test]
        public void HttpOptionsCanBeUsed()
        {
            this.CreateStubForHttpOptions();

            Given()
                .When()
                .Options($"{MOCK_SERVER_BASE_URL}/http-options");
        }

        /// <summary>
        /// Creates the stub response for the HTTP GET example.
        /// </summary>
        private void CreateStubForHttpGet()
        {
            this.Server?.Given(Request.Create().WithPath("/http-get").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the HTTP POST example.
        /// </summary>
        private void CreateStubForHttpPost()
        {
            this.Server?.Given(Request.Create().WithPath("/http-post").UsingPost())
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }

        /// <summary>
        /// Creates the stub response for the HTTP PUT example.
        /// </summary>
        private void CreateStubForHttpPut()
        {
            this.Server?.Given(Request.Create().WithPath("/http-put").UsingPut())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the HTTP PATCH example.
        /// </summary>
        private void CreateStubForHttpPatch()
        {
            this.Server?.Given(Request.Create().WithPath("/http-patch").UsingPatch())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the HTTP DELETE example.
        /// </summary>
        private void CreateStubForHttpDelete()
        {
            this.Server?.Given(Request.Create().WithPath("/http-delete").UsingDelete())
                .RespondWith(Response.Create()
                .WithStatusCode(204));
        }

        /// <summary>
        /// Creates the stub response for the HTTP HEAD example.
        /// </summary>
        private void CreateStubForHttpHead()
        {
            this.Server?.Given(Request.Create().WithPath("/http-head").UsingHead())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the HTTP OPTIONS example.
        /// </summary>
        private void CreateStubForHttpOptions()
        {
            this.Server?.Given(Request.Create().WithPath("/http-options").UsingOptions())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}