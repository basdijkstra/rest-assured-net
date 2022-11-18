// <copyright file="RequestBodyTests.cs" company="On Test Automation">
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

using NUnit.Framework;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssured.Net.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class RequestBodyTests : TestBase
    {
        private readonly string plaintextRequestBody = "Here's a plaintext request body.";

        private readonly string jsonStringRequestBody = "{\"id\": 1, \"user\": \"John Doe\"}";

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for sending
        /// a plaintext request body when performing an HTTP POST.
        /// </summary>
        [Test]
        public void PlaintextRequestBodyCanBeSupplied()
        {
            this.CreateStubForPlaintextRequestBody();

            Given()
            .Body(this.plaintextRequestBody)
            .When()
            .Post("http://localhost:9876/plaintext-request-body")
            .Then()
            .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for sending
        /// a JSON string request body when performing an HTTP POST.
        /// </summary>
        [Test]
        public void JsonStringRequestBodyCanBeSupplied()
        {
            this.CreateStubForJsonStringRequestBody();

            Given()
            .Body(this.jsonStringRequestBody)
            .When()
            .Post("http://localhost:9876/json-string-request-body")
            .Then()
            .StatusCode(201);
        }

        /// <summary>
        /// Creates the stub response for the JSON string request body example.
        /// </summary>
        private void CreateStubForPlaintextRequestBody()
        {
            this.Server.Given(Request.Create().WithPath("/plaintext-request-body").UsingPost()
                .WithBody(new ExactMatcher(this.plaintextRequestBody)))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }

        /// <summary>
        /// Creates the stub response for the JSON string request body example.
        /// </summary>
        private void CreateStubForJsonStringRequestBody()
        {
            this.Server.Given(Request.Create().WithPath("/json-string-request-body").UsingPost()
                .WithBody(new JsonMatcher(this.jsonStringRequestBody)))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }
    }
}