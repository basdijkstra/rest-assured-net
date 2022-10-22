// <copyright file="RequestSpecificationTests.cs" company="On Test Automation">
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
using RestAssured.Net.RA.Builders;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using static RestAssuredNet.RestAssuredNet;

namespace RestAssuredNet.Tests
{
    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class RequestSpecificationTests : TestBase
    {
        private RequestSpecification? requestSpecification;

        /// <summary>
        /// Creates a new <see cref="RequestSpecification"/> to be used in the tests in this class.
        /// </summary>
        [SetUp]
        public void CreateRequestSpecification()
        {
            this.requestSpecification = new RequestSpecBuilder()
                .WithHostName("localhost")
                .WithPort(9876)
                .Build();
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a header with a single value when sending an HTTP request.
        /// </summary>
        [Test]
        public void RequestSpecificationCanBeUsed()
        {
            this.CreateStubForRequestSpecification();

            Given()
            .Spec(this.requestSpecification)
            .When()
            .Get("/request-specification")
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// Creates the stub response for the example setting the accept header as a string.
        /// </summary>
        private void CreateStubForRequestSpecification()
        {
            this.Server.Given(Request.Create().WithPath("/request-specification").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}