// <copyright file="GraphQLRequest.cs" company="On Test Automation">
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
    /// Class containing properties for creating a GraphQL API request.
    /// </summary>
    public class GraphQLRequest
    {
        /// <summary>
        /// The GraphQL query to be used when sending the request.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// The operation name to use when submitting a parameterized GraphQL request.
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// The variables to be used when submitting a parameterized GraphQL request.
        /// </summary>
        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphQLRequest"/> class.
        /// </summary>
        /// <param name="query">The GraphQL query to use in this request.</param>
        /// <param name="operationName">The operation name to use in this request.</param>
        /// <param name="variables">The variables to use in the GraphQL query.</param>
        public GraphQLRequest(string query, string operationName, Dictionary<string, object> variables)
        {
            this.Query = query;
            this.OperationName = operationName;
            this.Variables = variables;
        }
    }
}
