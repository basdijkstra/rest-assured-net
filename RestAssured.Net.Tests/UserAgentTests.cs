// <copyright file="UserAgentTests.cs" company="On Test Automation">
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
    using System.Net.Http.Headers;
    using NUnit.Framework;
    using RestAssured.Request.Builders;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class UserAgentTests : TestBase
    {
        private RequestSpecification requestSpecification;

        /// <summary>
        /// Creates the <see cref="RequestSpecification"/> instances to be used in the tests in this class.
        /// </summary>
        [SetUp]
        public void CreateRequestSpecifications()
        {
            this.requestSpecification = new RequestSpecBuilder()
                .WithScheme("http")
                .WithHostName("localhost")
                .WithPort(9876)
                .WithUserAgent(new ProductInfoHeaderValue("MyUserAgent", "1.0"))
                .Build();
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for specifying
        /// a user agent for a specific request when sending an HTTP request.
        /// </summary>
        [Test]
        public void CustomUserAgentCanBeSuppliedUsingProductInfoHeaderValueInTest()
        {
            this.CreateStubForUserAgent();

            Given()
                .UserAgent(new ProductInfoHeaderValue("MyUserAgent", "1.0"))
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/user-agent")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for specifying
        /// a user agent for a specific request when sending an HTTP request.
        /// </summary>
        [Test]
        public void CustomUserAgentCanBeSuppliedUsingProductNameAndProductVersionInTest()
        {
            this.CreateStubForUserAgent();

            Given()
                .UserAgent("MyUserAgent", "1.0")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/user-agent")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for specifying
        /// a user agent using a request specification.
        /// </summary>
        [Test]
        public void CustomUserAgentCanBeSuppliedThroughRequestSpecification()
        {
            this.CreateStubForUserAgent();

            Given()
                .Spec(this.requestSpecification!)
                .When()
                .Get("/user-agent")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for the form data example.
        /// </summary>
        private void CreateStubForUserAgent()
        {
            this.Server?.Given(Request.Create().WithPath("/user-agent").UsingGet()
                .WithHeader("User-Agent", "MyUserAgent/1.0"))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}