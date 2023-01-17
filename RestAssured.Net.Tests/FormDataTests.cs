// <copyright file="FormDataTests.cs" company="On Test Automation">
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
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class FormDataTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// x-www-form-urlencoded form data when sending an HTTP request.
        /// </summary>
        [Test]
        public void FormDataCanBeSupplied()
        {
            this.CreateStubForFormData();

            var formData = new[]
            {
                new KeyValuePair<string, string>("name", "John Doe"),
                new KeyValuePair<string, string>("email", "johndoe@example.com"),
            };

            Given()
                .FormData(formData)
                .When()
                .Post("http://localhost:9876/form-data")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// Creates the stub response for the form data example.
        /// </summary>
        private void CreateStubForFormData()
        {
            this.Server?.Given(Request.Create().WithPath("/form-data").UsingPost()
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .WithBody(new ExactMatcher("name=John+Doe&email=johndoe%40example.com")))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }
    }
}