// <copyright file="AuthorizationTests.cs" company="On Test Automation">
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
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssured.Net.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class AuthorizationTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// Basic authorization details with the request.
        /// </summary>
        [Test]
        public void BasicAuthorizationDetailsCanBeSupplied()
        {
            this.CreateStubForBasicAuthorizationVerification();

            Given()
            .BasicAuth("username", "password")
            .When()
            .Get("http://localhost:9876/basic-auth")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// an OAuth2 authorization token with the request.
        /// </summary>
        [Test]
        public void OAuth2TokenCanBeSupplied()
        {
            this.CreateStubForOAuth2TokenAuthorizationVerification();

            Given()
            .OAuth2("this_is_my_token")
            .When()
            .Get("http://localhost:9876/oauth2")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for the example using Basic authorization.
        /// </summary>
        private void CreateStubForBasicAuthorizationVerification()
        {
            this.Server.Given(Request.Create().WithPath("/basic-auth").UsingGet()
                .WithHeader("Authorization", new ExactMatcher("Basic dXNlcm5hbWU6cGFzc3dvcmQ=")))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the example using an OAuth2 token.
        /// </summary>
        private void CreateStubForOAuth2TokenAuthorizationVerification()
        {
            this.Server.Given(Request.Create().WithPath("/oauth2").UsingGet()
                .WithHeader("Authorization", new ExactMatcher("Bearer this_is_my_token")))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}