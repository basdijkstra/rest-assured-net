// <copyright file="JsonSchemaValidationTests.cs" company="On Test Automation">
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
    using NJsonSchema;
    using NUnit.Framework;
    using RestAssured.Response.Exceptions;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class JsonSchemaValidationTests : TestBase
    {
        private readonly string jsonSchema = @"{ 'type': 'object', 'properties': { 'name': { 'type':'string'}, 'hobbies': { 'type': 'array', 'items': { 'type': 'string' } } } }";
        private readonly string invalidJsonSchema = @"{ 'object', 'properties': { 'name': { 'type':'string'}, 'hobbies': { 'type': 'array', 'items': { 'type': 'string' } } } }";

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for validating a response
        /// against a JSON schema supplied as a string.
        /// </summary>
        [Test]
        public void JsonSchemaCanBeSuppliedAndVerifiedAsString()
        {
            this.CreateStubForJsonSchemaValidation();

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-schema-validation")
                .Then()
                .StatusCode(200)
                .And()
                .MatchesJsonSchema(this.jsonSchema);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for validating a response
        /// against a JSON schema supplied as a JsonSchema.
        /// </summary>
        [Test]
        public void JsonSchemaCanBeSuppliedAndVerifiedAsJsonSchema()
        {
            this.CreateStubForJsonSchemaValidation();

            JsonSchema parsedSchema = JsonSchema.FromJsonAsync(this.jsonSchema).Result;

            Given()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/json-schema-validation")
                .Then()
                .StatusCode(200)
                .And()
                .MatchesJsonSchema(parsedSchema);
        }

        /// <summary>
        /// A test checking that a JSON schema mismatch throws the expected exception.
        /// </summary>
        [Test]
        public void MismatchWithJsonSchemaThrowsTheExpectedException()
        {
            this.CreateStubForJsonSchemaValidationMismatch();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/json-schema-validation-mismatch")
                    .Then()
                    .StatusCode(200)
                    .And()
                    .MatchesJsonSchema(this.jsonSchema);
            });

            Assert.That(rve?.Message, Does.Contain("Response body did not match JSON schema supplied. Error: 'StringExpected: #/name'"));
        }

        /// <summary>
        /// A test checking that supplying an invalid JSON schema throws the expected exception.
        /// </summary>
        [Test]
        public void SupplyingInvalidJsonSchemaThrowsTheExpectedException()
        {
            this.CreateStubForJsonSchemaValidationMismatch();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/json-schema-validation-mismatch")
                    .Then()
                    .StatusCode(200)
                    .And()
                    .MatchesJsonSchema(this.invalidJsonSchema);
            });

            Assert.That(rve?.Message, Does.Contain("Could not parse supplied JSON schema. Error:"));
        }

        /// <summary>
        /// A test checking that a response with an unexpected Content-Type throws the expected exception.
        /// </summary>
        [Test]
        public void UnexpectedResponseContentTypeThrowsTheExpectedException()
        {
            this.CreateStubForJsonSchemaUnexpectedResponseContentType();

            var rve = Assert.Throws<ResponseVerificationException>(() =>
            {
                Given()
                    .When()
                    .Get($"{MOCK_SERVER_BASE_URL}/json-schema-unexpected-content-type")
                    .Then()
                    .StatusCode(200)
                    .And()
                    .MatchesJsonSchema(this.jsonSchema);
            });

            Assert.That(rve?.Message, Is.EqualTo("Expected response Content-Type header to contain 'json', but was 'application/something'"));
        }

        /// <summary>
        /// Creates the stub response for the JSON schema validation example.
        /// </summary>
        private void CreateStubForJsonSchemaValidation()
        {
            var responseData = new
            {
                name = "John Smith",
                hobbies = new[] { "Running", "Reading", "C#", "Software testing" },
            };

            this.Server?.Given(Request.Create().WithPath("/json-schema-validation").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(responseData)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON schema validation mismatch example.
        /// </summary>
        private void CreateStubForJsonSchemaValidationMismatch()
        {
            var responseData = new
            {
                name = 12345,
                hobbies = new[] { "Running", "Reading", "C#", "Software testing" },
            };

            this.Server?.Given(Request.Create().WithPath("/json-schema-validation-mismatch").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(responseData)
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the JSON schema validation example
        /// with an unexpected response Content-Type header value.
        /// </summary>
        private void CreateStubForJsonSchemaUnexpectedResponseContentType()
        {
            this.Server?.Given(Request.Create().WithPath("/json-schema-unexpected-content-type").UsingGet())
                .RespondWith(Response.Create()
                .WithHeader("Content-Type", "application/something")
                .WithBody("Something")
                .WithStatusCode(200));
        }
    }
}