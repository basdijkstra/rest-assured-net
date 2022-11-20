// <copyright file="GraphQLRequestBuilder.cs" company="On Test Automation">
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
namespace RestAssured.Request.Builders
{
    /// <summary>
    /// A builder class to construct a new instance of the <see cref="GraphQLRequest"/> class.
    /// </summary>
    public class GraphQLRequestBuilder
    {
        private readonly GraphQLRequest graphQLRequest;

        private readonly string query = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphQLRequestBuilder"/> class.
        /// </summary>
        public GraphQLRequestBuilder()
        {
            this.graphQLRequest = new GraphQLRequest(this.query);
        }

        /// <summary>
        /// Sets the query on the <see cref="GraphQLRequest"/> to build.
        /// </summary>
        /// <param name="query">The GraphQL query to use in the request.</param>
        /// <returns>The current <see cref="GraphQLRequestBuilder"/> object.</returns>
        public GraphQLRequestBuilder WithQuery(string query)
        {
            this.graphQLRequest.Query = query;
            return this;
        }

        /// <summary>
        /// Returns the <see cref="GraphQLRequest"/> that was built.
        /// </summary>
        /// <returns>The <see cref="GraphQLRequest"/> object built in this builder class.</returns>
        public GraphQLRequest Build()
        {
            return this.graphQLRequest;
        }
    }
}