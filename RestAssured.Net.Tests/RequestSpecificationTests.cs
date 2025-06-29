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
namespace RestAssured.Tests
{
    using System.Text;
    using NUnit.Framework;
    using RestAssured.Logging;
    using RestAssured.Request.Builders;
    using RestAssured.Request.Exceptions;
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class RequestSpecificationTests : TestBase
    {
        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a request specification with all values set.
        /// </summary>
        /// <param name="endpoint">The endpoint to use in the HTTP call.</param>
        [TestCase("/request-specification", TestName = "With base path in request specification, works with a leading /")]
        [TestCase("request-specification", TestName = "With base path in request specification, works without a leading /")]
        public void FullRequestSpecificationCanBeUsed(string endpoint)
        {
            var logConfig = new LogConfiguration
            {
                RequestLogLevel = Logging.RequestLogLevel.All,
            };

            var fullRequestSpecification = new RequestSpecBuilder()
                .WithBaseUri("http://localhost:9876")
                .WithBasePath("api")
                .WithQueryParam("param_name", "param_value")
                .WithQueryParam("another_param_name", "another_param_value")
                .WithLogConfiguration(logConfig)
                .Build();

            this.CreateStubForRequestSpecification();

            Given()
                .Spec(fullRequestSpecification)
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
        [TestCase("/api/request-specification-no-query-params", TestName = "Works with a leading / in the endpoint")]
        [TestCase("api/request-specification-no-query-params", TestName = "Works without a leading / in the endpoint")]
        public void DefaultValuesAppliedRequestSpecificationCanBeUsed(string endpoint)
        {
            var applyDefaultsRequestSpecification = new RequestSpecBuilder()
                .WithPort(9876) // We need to set this because the default is 80 / 443
                .Build();

            this.CreateStubForRequestSpecificationWithoutQueryParams();

            Given()
                .Spec(applyDefaultsRequestSpecification)
                .When()
                .Get(endpoint)
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a request specification with headers added.
        /// </summary>
        [Test]
        public void RequestSpecificationWithHeadersCanBeUsed()
        {
            var headersSpecification = new RequestSpecBuilder()
                .WithBaseUri("http://localhost:9876")
                .WithBasicAuth("username", "password")
                .WithContentType("application/xml")
                .WithContentEncoding(Encoding.ASCII)
                .WithHeader("custom_header", "custom_header_value")
                .Build();

            this.CreateStubForRequestSpecificationWithHeaders();

            Given()
                .Spec(headersSpecification)
                .Header("another_header", "another_header_value")
                .When()
                .Get("/request-specification-with-headers")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// a request specification with OAuth2 details added.
        /// </summary>
        [Test]
        public void RequestSpecificationWithOAuth2CanBeUsed()
        {
            var oauthSpecification = new RequestSpecBuilder()
                .WithBaseUri("http://localhost:9876")
                .WithOAuth2("this_is_my_token")
                .Build();

            this.CreateStubForRequestSpecificationWithOAuth2();

            Given()
                .Spec(oauthSpecification)
                .When()
                .Get("/request-specification-with-oauth2")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax showing that
        /// using a hostname in the request specification including the scheme
        /// throws the expected exception.
        /// </summary>
        [Test]
        public void AnotherInvalidBaseUriThrowsTheExpectedException()
        {
            this.CreateStubForRequestSpecification();

            var rce = Assert.Throws<RequestCreationException>(() =>
            {
                var incorrectHostNameSpecification = new RequestSpecBuilder()
                .WithBaseUri("http:/localhost")
                .WithPort(9876)
                .Build();
            });

            Assert.That(rce?.Message, Is.EqualTo("Supplied value 'http:/localhost' is not a valid URI"));
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax showing that
        /// using an incorrect value for the base URI throws the expected exception.
        /// </summary>
        [Test]
        public void InvalidBaseUriThrowsTheExpectedException()
        {
            var rce = Assert.Throws<RequestCreationException>(() =>
            {
                var invalidBaseUriSpecification = new RequestSpecBuilder()
                    .WithBaseUri("not-valid")
                    .Build();
            });

            Assert.That(rce?.Message, Is.EqualTo("Supplied value 'not-valid' is not a valid URI"));
        }

        /// <summary>
        /// Creates the stub response for the request specification tests.
        /// </summary>
        private void CreateStubForRequestSpecification()
        {
            this.Server?.Given(Request.Create().WithPath("/api/request-specification")
                .WithParam("param_name", "param_value")
                .WithParam("another_param_name", "another_param_value")
                .UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the request specification tests, without query parameters.
        /// </summary>
        private void CreateStubForRequestSpecificationWithoutQueryParams()
        {
            this.Server?.Given(Request.Create().WithPath("/api/request-specification-no-query-params")
                .UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the request specification with headers tests.
        /// </summary>
        private void CreateStubForRequestSpecificationWithHeaders()
        {
            this.Server?.Given(Request.Create().WithPath("/request-specification-with-headers").UsingGet()
                .WithHeader("Content-Type", new ExactMatcher("application/xml; charset=us-ascii"))
                .WithHeader("Authorization", new ExactMatcher("Basic dXNlcm5hbWU6cGFzc3dvcmQ="))
                .WithHeader("custom_header", "custom_header_value")
                .WithHeader("another_header", "another_header_value"))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }

        /// <summary>
        /// Creates the stub response for the request specification with an OAuth2 token tests.
        /// </summary>
        private void CreateStubForRequestSpecificationWithOAuth2()
        {
            this.Server?.Given(Request.Create().WithPath("/request-specification-with-oauth2").UsingGet()
                .WithHeader("Authorization", new ExactMatcher("Bearer this_is_my_token")))
                .RespondWith(Response.Create()
                .WithStatusCode(200));
        }
    }
}