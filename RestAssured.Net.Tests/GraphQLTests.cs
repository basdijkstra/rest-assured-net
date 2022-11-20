// <copyright file="GraphQLTests.cs" company="On Test Automation">
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
    using System.Net;
    using NUnit.Framework;
    using RestAssured.Request.Builders;
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class GraphQLTests : TestBase
    {
        private readonly string simpleQuery = "{ company { name ceo } }";

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for sending
        /// a request to a GraphQL API.
        /// </summary>
        [Test]
        public void GraphQLQueryCanBeSupplied()
        {
            this.CreateStubForNonParameterizedGraphQLQuery();

            GraphQLRequest request = new GraphQLRequestBuilder()
                .WithQuery(this.simpleQuery)
                .Build();

            Given()
            .GraphQL(request)
            .When()
            .Post("http://localhost:9876/simple-graphql")
            .Then()
            .StatusCode(200)
            .Body("$.data.company.name", NHamcrest.Is.EqualTo("SpaceX"));
        }

        /// <summary>
        /// Creates the stub response for the simple GraphQL example.
        /// </summary>
        private void CreateStubForNonParameterizedGraphQLQuery()
        {
            var expectedQuery = new
            {
                query = this.simpleQuery,
            };

            var response = new
            {
                data = new
                {
                    company = new
                    {
                        name = "SpaceX",
                        ceo = "Elon Musk",
                    },
                },
            };

            this.Server.Given(Request.Create().WithPath("/simple-graphql").UsingPost()
                .WithBody(new JsonMatcher(expectedQuery)))
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(response));
        }
    }
}