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
    using System.Collections.Generic;
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

        private readonly string parameterizedQuery = @"query getRocketData($id: ID!)
            {
                rocket(id: $id) {
                    name
                    country
                }
            }";

        private readonly string companyName = Faker.Company.Name();
        private readonly string ceoName = Faker.Name.FullName();
        private readonly string countryName = Faker.Country.Name();
        private readonly string rocketName = Faker.Lorem.Words(3).ToString();

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for sending
        /// a simple (non-parameterized) request to a GraphQL API.
        /// </summary>
        [Test]
        public void SimpleGraphQLQueryCanBeSupplied()
        {
            this.CreateStubForSimpleGraphQLQuery();

            GraphQLRequest request = new GraphQLRequestBuilder()
                .WithQuery(this.simpleQuery)
                .Build();

            Given()
                .GraphQL(request)
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/simple-graphql")
                .Then()
                .StatusCode(200)
                .Body("$.data.company.name", NHamcrest.Is.EqualTo(this.companyName));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for sending
        /// a parameterized request to a GraphQL API.
        /// </summary>
        [Test]
        public void ParameterizedGraphQLQueryCanBeSupplied()
        {
            this.CreateStubForGraphQLQueryWithVariables();

            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "id", this.rocketName },
            };

            GraphQLRequest request = new GraphQLRequestBuilder()
                .WithQuery(this.parameterizedQuery)
                .WithOperationName("getRocketData")
                .WithVariables(variables)
                .Build();

            Given()
                .GraphQL(request)
                .ContentType("application/graphql+json")
                .When()
                .Post($"{MOCK_SERVER_BASE_URL}/graphql-with-variables")
                .Then()
                .StatusCode(200)
                .Body("$.data.rocket.country", NHamcrest.Is.EqualTo(this.countryName));
        }

        /// <summary>
        /// Creates the stub response for the simple GraphQL example.
        /// </summary>
        private void CreateStubForSimpleGraphQLQuery()
        {
            var expectedQuery = new
            {
                query = this.simpleQuery,
                operationName = string.Empty,
                variables = new { },
            };

            var response = new
            {
                data = new
                {
                    company = new
                    {
                        name = this.companyName,
                        ceo = this.ceoName,
                    },
                },
            };

            this.Server?.Given(Request.Create().WithPath("/simple-graphql").UsingPost()
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithBody(new JsonMatcher(expectedQuery)))
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(response));
        }

        /// <summary>
        /// Creates the stub response for the simple GraphQL example.
        /// </summary>
        private void CreateStubForGraphQLQueryWithVariables()
        {
            var expectedQuery = new
            {
                query = this.parameterizedQuery,
                operationName = "getRocketData",
                variables = new
                {
                    id = this.rocketName,
                },
            };

            var response = new
            {
                data = new
                {
                    rocket = new
                    {
                        name = this.rocketName,
                        country = this.countryName,
                    },
                },
            };

            this.Server?.Given(Request.Create().WithPath("/graphql-with-variables").UsingPost()
                .WithHeader("Content-Type", "application/graphql+json; charset=utf-8")
                .WithBody(new JsonMatcher(expectedQuery)))
                .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(response));
        }
    }
}