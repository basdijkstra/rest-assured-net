// <copyright file="ResponseBodyExtractionTests.cs" company="On Test Automation">
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
    using System.IO;
    using NUnit.Framework;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class ResponseBodyExtractionTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a plaintext response
        /// body as a string.
        /// </summary>
        [Test]
        public void PlainTextResponseBodyCanBeExtractedAsAString()
        {
            this.CreateStubForPlainTextResponse();

            string responseBody = Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/plain-text-response-body")
                .Then()
                .StatusCode(200)
                .Extract().BodyAsString();

            Assert.That(responseBody, Is.EqualTo("Plain text response body."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a plaintext response
        /// body as a byte array.
        /// </summary>
        [Test]
        public void PlainTextResponseBodyCanBeExtractedAsAByteArray()
        {
            this.CreateStubForPlainTextResponse();

            byte[] responseBody = Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/plain-text-response-body")
                .Then()
                .StatusCode(200)
                .Extract().BodyAsByteArray();

            Assert.That(responseBody, Is.EqualTo("Plain text response body."));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for extracting a plaintext response
        /// body as a <see cref="System.IO.Stream"/>.
        /// </summary>
        [Test]
        public void PlainTextResponseBodyCanBeExtractedAsAStream()
        {
            this.CreateStubForPlainTextResponse();

            Stream responseBody = Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/plain-text-response-body")
                .Then()
                .StatusCode(200)
                .Extract().BodyAsStream();

            Assert.That(new StreamReader(responseBody).ReadToEnd(), Is.EqualTo("Plain text response body."));
        }

        /// <summary>
        /// Creates the stub response for the plain text response body example.
        /// </summary>
        private void CreateStubForPlainTextResponse()
        {
            this.Server?.Given(Request.Create().WithPath("/plain-text-response-body").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "text/plain")
                .WithStatusCode(200)
                .WithBody("Plain text response body."));
        }
    }
}