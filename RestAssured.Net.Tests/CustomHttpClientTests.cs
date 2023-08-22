// <copyright file="CustomHttpClientTests.cs" company="On Test Automation">
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
    using DemoService;
    using Microsoft.AspNetCore.Mvc.Testing;
    using NUnit.Framework;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class CustomHttpClientTests
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// a response status code when performing an HTTP GET.
        /// </summary>
        [Test]
        public void HttpGetCanBeUsed()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            var httpClient = webAppFactory.CreateDefaultClient();

            Given(httpClient)
                .When()
                .Get("http://localhost:5131/weatherforecast")
                .Then()
                .StatusCode(HttpStatusCode.OK);
        }
    }
}