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
    using System.Linq;
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
        /// A test checking that a path parameter value is not HTML-encoded.
        /// </summary>
        [Test]
        public void PathParameterValueIsNotHtmlEncoded()
        {
            Given()
                .PathParam("id", "a&b")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/items/[id]");

            Assert.That(this.GetLastRequestedPath(), Is.EqualTo("/items/a&b"));
        }

        /// <summary>
        /// A test checking that text between square brackets that is not a path parameter
        /// is left unchanged, for example in a query string.
        /// </summary>
        [Test]
        public void BracketsThatAreNotPathParametersAreLeftUnchanged()
        {
            Given()
                .PathParam("id", "7")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/items/[id]?filter[name]=x");

            Assert.That(this.GetLastRequestedPath(), Is.EqualTo("/items/7"));
            Assert.That(this.GetLastRequestedQuery(), Does.ContainKey("filter[name]"));
        }

        /// <summary>
        /// A test checking that a placeholder for which no path parameter value was supplied
        /// is left in the endpoint, instead of being removed.
        /// </summary>
        [Test]
        public void PlaceholderWithoutPathParameterValueIsLeftUnchanged()
        {
            Given()
                .PathParam("userid", "7")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/users/[userId]");

            Assert.That(this.GetLastRequestedPath(), Does.Contain("userId"));
        }

        /// <summary>
        /// A test checking that a path parameter that is used more than once in an endpoint is replaced everywhere.
        /// </summary>
        [Test]
        public void SamePathParameterCanBeUsedMoreThanOnce()
        {
            Given()
                .PathParam("id", "5")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/items/[id]/copy/[id]");

            Assert.That(this.GetLastRequestedPath(), Is.EqualTo("/items/5/copy/5"));
        }

        /// <summary>
        /// Returns the (decoded) path of the last request received by the mock server.
        /// </summary>
        /// <returns>The path of the last request received by the mock server.</returns>
        private string GetLastRequestedPath()
        {
            return this.Server.LogEntries.Last().RequestMessage.Path;
        }

        /// <summary>
        /// Returns the query parameters of the last request received by the mock server.
        /// </summary>
        /// <returns>The query parameters of the last request received by the mock server.</returns>
        private IDictionary<string, WireMock.Types.WireMockList<string>> GetLastRequestedQuery()
        {
            return this.Server.LogEntries.Last().RequestMessage.Query;
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