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
        private RequestSpecification? fullRequestSpecification;
        private RequestSpecification? applyDefaultsRequestSpecification;
        private RequestSpecification? incorrectHostNameSpecification;

        /// <summary>
        /// Creates the <see cref="RequestSpecification"/> instances to be used in the tests in this class.
        /// </summary>
        [SetUp]
        public void CreateRequestSpecifications()
        {
            this.fullRequestSpecification = new RequestSpecBuilder()
                .WithScheme("http")
                .WithHostName("localhost")
                .WithBasePath("api")
                .WithPort(9876)
                .Build();

            this.applyDefaultsRequestSpecification = new RequestSpecBuilder()
                .WithPort(9876) // We need to set this because the default is 80
                .Build();

            this.incorrectHostNameSpecification = new RequestSpecBuilder()
                .WithHostName("http://localhost")
                .WithPort(9876)
                .Build();
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a request specification with all values set.
        /// </summary>
        /// <param name="endpoint">The endpoint to use in the HTTP call.</param>
        [TestCase("/request-specification", TestName = "With base path in request specification, works with a leading /")]
        [TestCase("request-specification", TestName = "With base path in request specification, works without a leading /")]
        public void FullRequestSpecificationCanBeUsed(string endpoint)
        {
            this.CreateStubForRequestSpecification();

            Given()
            .Spec(this.fullRequestSpecification)
            .When()
            .Get(endpoint)
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a request specification with default values applied.
        /// </summary>
        /// <param name="endpoint">The endpoint to use in the HTTP call.</param>
        [TestCase("/api/request-specification", TestName = "Works with a leading / in the endpoint")]
        [TestCase("api/request-specification", TestName = "Works without a leading / in the endpoint")]
        public void DefaultValuesAppliedRequestSpecificationCanBeUsed(string endpoint)
        {
            this.CreateStubForRequestSpecification();

            Given()
            .Spec(this.applyDefaultsRequestSpecification)
            .When()
            .Get(endpoint)
            .Then()
            .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax showing that
        /// using a hostname in the request specification including the scheme
        /// throws the expected exception.
        /// </summary>
        [Test]
        public void UsingSchemeInHostNameThrowsTheExpectedException()
        {
            this.CreateStubForRequestSpecification();

            RA.Exceptions.RequestCreationException rce = Assert.Throws<RA.Exceptions.RequestCreationException>(() =>
            {
                Given()
                .Spec(this.incorrectHostNameSpecification)
                .When()
                .Get("/api/request-specification")
                .Then()
                .StatusCode(200);
            });

            Assert.That(rce.Message, Is.EqualTo("Supplied base URI 'http://http://localhost:9876' is invalid."));
        }

        /// <summary>
        /// Creates the stub response for the example setting the accept header as a string.
        /// </summary>
        private void CreateStubForRequestSpecification()
        {
            this.Server.Given(Request.Create().WithPath("/api/request-specification").UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}