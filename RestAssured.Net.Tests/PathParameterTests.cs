// <copyright file="PathParameterTests.cs" company="On Test Automation">
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
    using System;
    using System.Collections.Generic;
    using Humanizer.DateTimeHumanizeStrategy;
    using NUnit.Framework;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage, focusing on path parameters.
    /// </summary>
    [TestFixture]
    public class PathParameterTests : TestBase
    {
        private readonly int userId = Faker.RandomNumber.Next(999999);

        private readonly string accountId = Faker.Name.First().ToUpper() + Faker.RandomNumber.Next().ToString();

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for adding
        /// a single query parameter.
        /// </summary>
        [Test]
        public void SinglePathParameterCanBeSpecified()
        {
            this.CreateStubForSinglePathParameter();

            Given()
                .PathParam("userId", this.userId)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/user/[userId]")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for adding
        /// multiple query parameters sequentially.
        /// </summary>
        [Test]
        public void MultiplePathParameterCanBeSpecifiedSequentially()
        {
            this.CreateStubForMultiplePathParameters();

            Given()
                .PathParam("userId", this.userId)
                .PathParam("accountId", this.accountId)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/user/[userId]/account/[accountId]")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for adding
        /// multiple query parameters using a Dictionary.
        /// </summary>
        [Test]
        public void MultiplePathParameterCanBeSpecifiedUsingADictionary()
        {
            Dictionary<string, object> pathParams = new Dictionary<string, object>
            {
                { "userId", this.userId },
                { "accountId", this.accountId },
            };

            this.CreateStubForMultiplePathParameters();

            Given()
                .PathParams(pathParams)
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/user/[userId]/account/[accountId]")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for single path parameter example.
        /// </summary>
        private void CreateStubForSinglePathParameter()
        {
            this.Server?.Given(Request.Create()
                .WithPath($"/user/{this.userId}")
                .UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for multiple path parameters example.
        /// </summary>
        private void CreateStubForMultiplePathParameters()
        {
            this.Server?.Given(Request.Create()
                .WithPath($"/user/{this.userId}/account/{this.accountId}")
                .UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}