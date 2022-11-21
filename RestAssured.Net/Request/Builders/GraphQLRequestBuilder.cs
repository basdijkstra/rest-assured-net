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
    using System.Collections.Generic;

    /// <summary>
    /// A builder class to construct a new instance of the <see cref="GraphQLRequest"/> class.
    /// </summary>
    public class GraphQLRequestBuilder
    {
        private readonly GraphQLRequest graphQLRequest;

        private readonly string query = string.Empty;
        private readonly string operationName = string.Empty;
        private readonly Dictionary<string, object> variables = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphQLRequestBuilder"/> class.
        /// </summary>
        public GraphQLRequestBuilder()
        {
            this.graphQLRequest = new GraphQLRequest(this.query, this.operationName, this.variables);
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
        /// Sets the operation name on the <see cref="GraphQLRequest"/> to build.
        /// </summary>
        /// <param name="operationName">The operation name to use in the request.</param>
        /// <returns>The current <see cref="GraphQLRequestBuilder"/> object.</returns>
        public GraphQLRequestBuilder WithOperationName(string operationName)
        {
            this.graphQLRequest.OperationName = operationName;
            return this;
        }

        /// <summary>
        /// Sets the variables to use in the <see cref="GraphQLRequest"/> query.
        /// </summary>
        /// <param name="variables">The variables to use in the query sent in the request.</param>
        /// <returns>The current <see cref="GraphQLRequestBuilder"/> object.</returns>
        public GraphQLRequestBuilder WithVariables(Dictionary<string, object> variables)
        {
            this.graphQLRequest.Variables = variables;
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