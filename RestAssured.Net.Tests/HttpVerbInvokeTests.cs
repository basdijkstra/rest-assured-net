// <copyright file="HttpVerbInvokeTests.cs" company="On Test Automation">
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
    using System.Net;
    using System.Net.Http;
    using NUnit.Framework;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class HttpVerbInvokeTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for invoking an
        /// endpoint using different HTTP verbs and checking the response
        /// status codes (for example to check on Broken Function Level
        /// Authorization from the OWASP API security top 10).
        /// </summary>
        /// <param name="httpMethod">The <see cref="HttpMethod"/> to use in the test.</param>
        /// <param name="expectedStatusCode">The expected <see cref="HttpStatusCode"/> to use in the test.</param>
        [TestCaseSource("HttpMethodTestData")]
        public void InvokeMethodCanBeUsed(HttpMethod httpMethod, HttpStatusCode expectedStatusCode)
        {
            this.CreateStubForHttpGet();
            this.CreateStubForHttpPost();
            this.CreateStubForHttpDelete();
            this.CreateStubForHttpPut();
            this.CreateStubForHttpPatch();
            this.CreateStubForHttpHead();
            this.CreateStubForHttpOptions();

            Given()
                .When()
                .Invoke($"{MOCK_SERVER_BASE_URL}/invoke-endpoint", httpMethod)
                .Then()
                .StatusCode(expectedStatusCode);
        }

        private static IEnumerable<TestCaseData> HttpMethodTestData()
        {
            yield return new TestCaseData(HttpMethod.Get, HttpStatusCode.OK).
                SetName("HTTP GET is allowed");
            yield return new TestCaseData(HttpMethod.Post, HttpStatusCode.MethodNotAllowed).
                SetName("HTTP POST is not allowed");
            yield return new TestCaseData(HttpMethod.Put, HttpStatusCode.MethodNotAllowed).
                SetName("HTTP PUT is not allowed");
            yield return new TestCaseData(HttpMethod.Patch, HttpStatusCode.MethodNotAllowed).
                SetName("HTTP PATCH is not allowed");
            yield return new TestCaseData(HttpMethod.Delete, HttpStatusCode.MethodNotAllowed).
                SetName("HTTP DELETE is not allowed");
            yield return new TestCaseData(HttpMethod.Head, HttpStatusCode.OK).
                SetName("HTTP HEAD is allowed");
            yield return new TestCaseData(HttpMethod.Options, HttpStatusCode.OK).
                SetName("HTTP OPTIONS is allowed");
        }

        /// <summary>
        /// Creates the stub response for the HTTP GET example.
        /// </summary>
        private void CreateStubForHttpGet()
        {
            this.Server?.Given(Request.Create().WithPath("/invoke-endpoint").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the HTTP POST example.
        /// </summary>
        private void CreateStubForHttpPost()
        {
            this.Server?.Given(Request.Create().WithPath("/invoke-endpoint").UsingPost())
                .RespondWith(Response.Create()
                .WithStatusCode(405));
        }

        /// <summary>
        /// Creates the stub response for the HTTP PUT example.
        /// </summary>
        private void CreateStubForHttpPut()
        {
            this.Server?.Given(Request.Create().WithPath("/invoke-endpoint").UsingPut())
                .RespondWith(Response.Create()
                .WithStatusCode(405));
        }

        /// <summary>
        /// Creates the stub response for the HTTP PATCH example.
        /// </summary>
        private void CreateStubForHttpPatch()
        {
            this.Server?.Given(Request.Create().WithPath("/invoke-endpoint").UsingPatch())
                .RespondWith(Response.Create()
                .WithStatusCode(405));
        }

        /// <summary>
        /// Creates the stub response for the HTTP DELETE example.
        /// </summary>
        private void CreateStubForHttpDelete()
        {
            this.Server?.Given(Request.Create().WithPath("/invoke-endpoint").UsingDelete())
                .RespondWith(Response.Create()
                .WithStatusCode(405));
        }

        /// <summary>
        /// Creates the stub response for the HTTP HEAD example.
        /// </summary>
        private void CreateStubForHttpHead()
        {
            this.Server?.Given(Request.Create().WithPath("/invoke-endpoint").UsingHead())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the HTTP OPTIONS example.
        /// </summary>
        private void CreateStubForHttpOptions()
        {
            this.Server?.Given(Request.Create().WithPath("/invoke-endpoint").UsingOptions())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}