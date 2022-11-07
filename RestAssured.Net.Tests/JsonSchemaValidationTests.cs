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
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class JsonSchemaValidationTests : TestBase
    {
        private readonly string jsonSchema = @"{ 'type': 'object', 'properties': { 'name': { 'type':'string'}, 'hobbies': { 'type': 'array', 'items': { 'type': 'string' } } } }";

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
            .Post("http://localhost:9876/json-schema-validation")
            .Then()
            .StatusCode(201)
            .And()
            .MatchesJsonSchema(this.jsonSchema);
        }

        /// <summary>
        /// A test checking that a JSON schema mismatch throws the expected exception.
        /// </summary>
        [Test]
        public void MismatchWithJsonSchemaThrowsTheExpectedExecption()
        {
            this.CreateStubForJsonSchemaValidationMismatch();

            RA.Exceptions.AssertionException ae = Assert.Throws<RA.Exceptions.AssertionException>(() =>
            {
                Given()
                .When()
                .Post("http://localhost:9876/json-schema-validation-mismatch")
                .Then()
                .StatusCode(201)
                .And()
                .MatchesJsonSchema(this.jsonSchema);
            });

            Assert.That(ae.Message, Does.Contain("Response body did not match JSON schema supplied: Invalid type. Expected String but got Integer."));
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

            this.Server.Given(Request.Create().WithPath("/json-schema-validation").UsingPost())
                .RespondWith(Response.Create()
                .WithBodyAsJson(responseData)
                .WithStatusCode(201));
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

            this.Server.Given(Request.Create().WithPath("/json-schema-validation-mismatch").UsingPost())
                .RespondWith(Response.Create()
                .WithBodyAsJson(responseData)
                .WithStatusCode(201));
        }
    }
}