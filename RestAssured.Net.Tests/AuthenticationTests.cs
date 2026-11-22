// <copyright file="AuthenticationTests.cs" company="On Test Automation">
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
    using NUnit.Framework;
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class AuthenticationTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// Basic authentication details with the request.
        /// </summary>
        [Test]
        public void BasicAuthenticationDetailsCanBeSupplied()
        {
            this.CreateStubForBasicAuthenticationVerification();

            Given()
                .BasicAuth("username", "password")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/basic-auth")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test checking that Basic authentication details containing
        /// non-ASCII characters are encoded as UTF-8.
        /// </summary>
        [Test]
        public void NonAsciiBasicAuthenticationDetailsAreEncodedAsUtf8()
        {
            this.CreateStubForNonAsciiBasicAuthenticationVerification();

            Given()
                .BasicAuth("jürgen", "pässwörd€")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/basic-auth-non-ascii")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// an OAuth2 autentication token with the request.
        /// </summary>
        [Test]
        public void OAuth2AuthenticationTokenCanBeSupplied()
        {
            this.CreateStubForOAuth2TokenAuthenticationVerification();

            Given()
                .OAuth2("this_is_my_token")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/oauth2")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for the example using Basic authentication.
        /// </summary>
        private void CreateStubForBasicAuthenticationVerification()
        {
            this.Server?.Given(Request.Create().WithPath("/basic-auth").UsingGet()
                .WithHeader("Authorization", new ExactMatcher("Basic dXNlcm5hbWU6cGFzc3dvcmQ=")))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the example using Basic authentication with non-ASCII characters.
        /// The expected value is the Base64 encoding of the UTF-8 bytes of 'jürgen:pässwörd€'.
        /// </summary>
        private void CreateStubForNonAsciiBasicAuthenticationVerification()
        {
            this.Server?.Given(Request.Create().WithPath("/basic-auth-non-ascii").UsingGet()
                .WithHeader("Authorization", new ExactMatcher("Basic asO8cmdlbjpww6Rzc3fDtnJk4oKs")))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the example using an OAuth2 authentication token.
        /// </summary>
        private void CreateStubForOAuth2TokenAuthenticationVerification()
        {
            this.Server?.Given(Request.Create().WithPath("/oauth2").UsingGet()
                .WithHeader("Authorization", new ExactMatcher("Bearer this_is_my_token")))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}