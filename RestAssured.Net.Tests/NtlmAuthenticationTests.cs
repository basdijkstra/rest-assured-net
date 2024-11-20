// <copyright file="NtlmAuthenticationTests.cs" company="On Test Automation">
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
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class NtlmAuthenticationTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// NTLM authentication details with default credentials with the request.
        /// </summary>
        [Test]
        public void NtlmUsingDefaultNetworkCredentialsCanBeSpecified()
        {
            this.CreateStubForNtlmAuthenticationVerification();

            Given()
                .NtlmAuth()
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/ntlm-authentication")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// NTLM authentication details with specified credentials with the request.
        /// </summary>
        [Test]
        public void NtlmUsingSpecifiedNetworkCredentialsCanBeSpecified()
        {
            this.CreateStubForNtlmAuthenticationVerification();

            Given()
                .NtlmAuth("username", "password", "domain")
                .When()
                .Get($"{MOCK_SERVER_BASE_URL}/ntlm-authentication")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for the example using NTLM authentication
        /// with default network credentials.
        /// </summary>
        private void CreateStubForNtlmAuthenticationVerification()
        {
            this.Server?.Given(Request.Create().WithPath("/ntlm-authentication").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}