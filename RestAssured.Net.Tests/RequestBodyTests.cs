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
namespace RestAssured.Tests
{
    using NUnit.Framework;
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class RequestBodyTests : TestBase
    {
        private readonly string jsonStringRequestBody = "{\"id\": " + Faker.RandomNumber.Next().ToString()
                                             + ", \"user\": \"" + Faker.Name.FullName() + "\"}";

        private string plaintextRequestBody;

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for sending
        /// a plaintext request body when performing an HTTP POST.
        /// </summary>
        [TestCase("small", TestName = "Sending small plaintext request body")]
        [TestCase("medium", TestName = "Sending medium plaintext request body")]
        [TestCase("large", TestName = "Sending large plaintext request body")]
        public void PlaintextRequestBodyCanBeSupplied(string bodySize)
        {
            this.CreateStubForPlaintextRequestBody(bodySize);

            Given()
                .Body(this.plaintextRequestBody)
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/plaintext-request-body")
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
                .Post($"{MOCK_SERVER_BASE_URL}/json-string-request-body")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// Creates the stub response for the JSON string request body example.
        /// </summary>
        private void CreateStubForPlaintextRequestBody(string bodySize)
        {
            switch (bodySize)
            {
                case "small":
                    this.plaintextRequestBody = Faker.Lorem.Paragraph(Faker.RandomNumber.Next(5, 10));
                    break;
                case "medium":
                    this.plaintextRequestBody = Faker.Lorem.Paragraph(Faker.RandomNumber.Next(50, 70));
                    break;
                case "large":
                    this.plaintextRequestBody = Faker.Lorem.Paragraph(Faker.RandomNumber.Next(300, 400));
                    break;
            }

            this.Server?.Given(Request.Create().WithPath("/plaintext-request-body").UsingPost()
                .WithBody(new ExactMatcher(this.plaintextRequestBody)))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }

        /// <summary>
        /// Creates the stub response for the JSON string request body example.
        /// </summary>
        private void CreateStubForJsonStringRequestBody()
        {
            this.Server?.Given(Request.Create().WithPath("/json-string-request-body").UsingPost()
                .WithBody(new JsonMatcher(this.jsonStringRequestBody)))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }
    }
}