// <copyright file="RequestHeaderUsageExamples.cs" company="On Test Automation">
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
using System.Collections.Generic;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class RequestHeaderUsageExamples
    {
        private WireMockServer server;

        /// <summary>
        /// Starts the WireMock server before every test.
        /// </summary>
        [SetUp]
        public void StartServer()
        {
            this.server = WireMockServer.Start(9876);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a header with a single value when sending an HTTP request.
        /// </summary>
        [Test]
        public void HeaderWithASingleValueCanBeSupplied()
        {
            this.CreateStubForSingleHeaderValue();

            Given()
            .Header("my_header", "my_header_value")
            .When()
            .Get("http://localhost:9876/single-header-value")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a header with multiple values when sending an HTTP request.
        /// </summary>
        [Test]
        public void HeaderWithMultipleValuesCanbeSupplied()
        {
            this.CreateStubForMultipleHeaderValues();

            Given()
            .Header("my_header", new List<string>() { "my_header_value_1", "my_header_value_2" })
            .When()
            .Get("http://localhost:9876/multiple-header-values")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// Stops the WireMock server after every test.
        /// </summary>
        [TearDown]
        public void StopServer()
        {
            this.server.Stop();
        }

        /// <summary>
        /// Creates the stub response for the single header value example.
        /// </summary>
        private void CreateStubForSingleHeaderValue()
        {
            this.server.Given(Request.Create().WithPath("/single-header-value").UsingGet()
                .WithHeader("my_header", "my_header_value"))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the multiple header values example.
        /// </summary>
        private void CreateStubForMultipleHeaderValues()
        {
            this.server.Given(Request.Create().WithPath("/multiple-header-values").UsingGet()
                .WithHeader("my_header", "my_header_value_1, my_header_value_2"))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}