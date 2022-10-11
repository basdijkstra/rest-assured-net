// <copyright file="QueryParameterTests.cs" company="On Test Automation">
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
using System.Collections.Generic;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage, focusing on query parameters.
    /// </summary>
    [TestFixture]
    public class QueryParameterTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for adding
        /// a single query parameter.
        /// </summary>
        [Test]
        public void SingleQueryParameterCanBeSpecified()
        {
            this.CreateStubForSingleQueryParameter();

            Given()
            .QueryParam("name", "john")
            .When()
            .Get("http://localhost:9876/single-query-param")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for adding
        /// multiple query parameters in separate calls.
        /// </summary>
        [Test]
        public void MultipleQueryParametersCanBeSpecifiedInSeparateCalls()
        {
            this.CreateStubForMultipleQueryParameters();

            Given()
            .QueryParam("name", "john")
            .QueryParam("id", 12345)
            .When()
            .Get("http://localhost:9876/multiple-query-params")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test to verify that when a query parameter is specified more than once,
        /// the last value specified will be used.
        /// </summary>
        [Test]
        public void SpecifyingTheSameSingleQueryParameterMoreThanOnceIsNotAProblem()
        {
            this.CreateStubForSingleQueryParameter();

            Given()
            .QueryParam("name", "susan")
            .QueryParam("name", "john")
            .When()
            .Get("http://localhost:9876/single-query-param")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test to verify that when a query parameter is specified more than once,
        /// the last value specified will be used.
        /// </summary>
        [Test]
        public void MultipleQueryParametersCanBeSpecifiedUsingADictionary()
        {
            Dictionary<string, object> queryParams = new Dictionary<string, object>();
            queryParams.Add("name", "john");
            queryParams.Add("id", 12345);

            this.CreateStubForMultipleQueryParameters();

            Given()
            .QueryParams(queryParams)
            .When()
            .Get("http://localhost:9876/multiple-query-params")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test to verify that when a query parameter is specified more than once,
        /// the last value specified will be used.
        /// </summary>
        [Test]
        public void SpecifyingTheSameQueryParametersMoreThanOnceUsingADictionaryIsNotAProblem()
        {
            Dictionary<string, object> queryParams = new Dictionary<string, object>();
            queryParams.Add("name", "susan"); // This parameter value will be overwritten
            queryParams.Add("id", 12345);

            this.CreateStubForMultipleQueryParameters();

            Given()
            .QueryParams(queryParams)
            .QueryParam("name", "john") // This parameter value will be used
            .When()
            .Get("http://localhost:9876/multiple-query-params")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for the single query parameter example.
        /// </summary>
        private void CreateStubForSingleQueryParameter()
        {
            this.Server.Given(Request.Create()
                .WithPath("/single-query-param")
                .WithParam("name", "john")
                .UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the multiple query parameter example.
        /// </summary>
        private void CreateStubForMultipleQueryParameters()
        {
            this.Server.Given(Request.Create()
                .WithPath("/multiple-query-params")
                .WithParam("name", "john")
                .WithParam("id", "12345")
                .UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}