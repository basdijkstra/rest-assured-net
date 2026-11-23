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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
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
                .WithBaseUri(MOCK_SERVER_BASE_URL)
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
                .WithPort(MOCK_SERVER_PORT) // We need to set this because the default is 80 / 443
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
                .WithBaseUri(MOCK_SERVER_BASE_URL)
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
        /// A test checking that Basic authentication details containing non-ASCII characters
        /// in a request specification are encoded as UTF-8.
        /// </summary>
        [Test]
        public void NonAsciiBasicAuthenticationDetailsInRequestSpecificationAreEncodedAsUtf8()
        {
            // The expected value is the Base64 encoding of the UTF-8 bytes of 'jürgen:pässwörd€'.
            this.Server?.Given(Request.Create().WithPath("/request-specification-with-non-ascii-basic-auth").UsingGet()
                .WithHeader("Authorization", new ExactMatcher("Basic asO8cmdlbjpww6Rzc3fDtnJk4oKs")))
                .RespondWith(Response.Create()
                .WithStatusCode(200));

            var basicAuthSpecification = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithBasicAuth("jürgen", "pässwörd€")
                .Build();

            Given()
                .Spec(basicAuthSpecification)
                .When()
                .Get("/request-specification-with-non-ascii-basic-auth")
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
                .WithBaseUri(MOCK_SERVER_BASE_URL)
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
        /// A test checking that a query string in a relative endpoint is sent as a query string
        /// when a request specification with a base path is used.
        /// </summary>
        [Test]
        public void RelativeEndpointWithQueryStringCanBeUsedWithBasePath()
        {
            this.CreateStubForQueryStringEndpoint("/api/query-string-endpoint");

            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithBasePath("api")
                .Build();

            Given()
                .Spec(requestSpecification)
                .When()
                .Get("/query-string-endpoint?id=1")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test checking that a query string in a relative endpoint is combined with query parameters
        /// set in the request specification and in the request.
        /// </summary>
        [Test]
        public void QueryStringInEndpointIsCombinedWithQueryParams()
        {
            this.Server?.Given(Request.Create().WithPath("/query-string-endpoint")
                .WithParam("id", "1")
                .WithParam("a", "1")
                .WithParam("b", "2")
                .UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));

            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithQueryParam("a", "1")
                .Build();

            Given()
                .Spec(requestSpecification)
                .QueryParam("b", "2")
                .When()
                .Get("/query-string-endpoint?id=1")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test checking that an already encoded query string in a relative endpoint is not encoded again.
        /// </summary>
        [Test]
        public void EncodedQueryStringInEndpointIsNotEncodedAgain()
        {
            this.Server?.Given(Request.Create().WithPath("/query-string-endpoint")
                .WithParam("id", "1")
                .WithParam("name", "John Doe")
                .UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));

            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .Build();

            Given()
                .Spec(requestSpecification)
                .When()
                .Get("/query-string-endpoint?id=1&name=John%20Doe")
                .Then()
                .StatusCode(200);
        }

        /// <summary>
        /// A test checking that a Content-Type set in the request takes precedence
        /// over a Content-Type set in the request specification.
        /// </summary>
        [Test]
        public void RequestContentTypeOverridesSpecContentType()
        {
            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithContentType("application/json")
                .Build();

            Given()
                .Spec(requestSpecification)
                .ContentType("application/xml")
                .Body("<a/>")
                .When()
                .Post("/precedence");

            Assert.That(this.GetLastRequestHeaderValues("Content-Type"), Is.EqualTo(new[] { "application/xml; charset=utf-8" }));
        }

        /// <summary>
        /// A test checking that a content encoding set in the request takes precedence
        /// over a content encoding set in the request specification.
        /// </summary>
        [Test]
        public void RequestContentEncodingOverridesSpecContentEncoding()
        {
            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithContentEncoding(Encoding.ASCII)
                .Build();

            Given()
                .Spec(requestSpecification)
                .ContentEncoding(Encoding.UTF8)
                .Body("ü")
                .When()
                .Post("/precedence");

            Assert.That(this.GetLastRequestHeaderValues("Content-Type"), Is.EqualTo(new[] { "application/json; charset=utf-8" }));
            Assert.That(this.Server.LogEntries.Last().RequestMessage.Body, Is.EqualTo("ü"));
        }

        /// <summary>
        /// A test checking that JSON serializer settings set in the request take precedence
        /// over JSON serializer settings set in the request specification.
        /// </summary>
        [Test]
        public void RequestJsonSerializerSettingsOverrideSpecJsonSerializerSettings()
        {
            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithJsonSerializerSettings(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include })
                .Build();

            Given()
                .Spec(requestSpecification)
                .JsonSerializerSettings(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                .Body(new { a = 1, b = (string)null })
                .When()
                .Post("/precedence");

            Assert.That(this.Server.LogEntries.Last().RequestMessage.Body, Is.EqualTo("{\"a\":1}"));
        }

        /// <summary>
        /// A test checking that JSON serializer settings set in the request are used
        /// when the request specification does not set any JSON serializer settings.
        /// </summary>
        [Test]
        public void RequestJsonSerializerSettingsAreUsedWhenSpecHasNone()
        {
            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .Build();

            Given()
                .Spec(requestSpecification)
                .JsonSerializerSettings(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                .Body(new { a = 1, b = (string)null })
                .When()
                .Post("/precedence");

            Assert.That(this.Server.LogEntries.Last().RequestMessage.Body, Is.EqualTo("{\"a\":1}"));
        }

        /// <summary>
        /// A test checking that a header set in the request replaces a header with the same name
        /// (compared case-insensitively) set in the request specification.
        /// </summary>
        [Test]
        public void RequestHeaderOverridesSpecHeaderWithSameName()
        {
            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithHeader("x-tenant", "a")
                .Build();

            Given()
                .Spec(requestSpecification)
                .Header("X-Tenant", "b")
                .When()
                .Get("/precedence");

            Assert.That(this.GetLastRequestHeaderValues("X-Tenant"), Is.EqualTo(new[] { "b" }));
        }

        /// <summary>
        /// A test checking that a user agent set in the request replaces
        /// the user agent set in the request specification.
        /// </summary>
        [Test]
        public void RequestUserAgentOverridesSpecUserAgent()
        {
            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri(MOCK_SERVER_BASE_URL)
                .WithUserAgent("SpecAgent", "1.0")
                .Build();

            Given()
                .Spec(requestSpecification)
                .UserAgent("RequestAgent", "2.0")
                .When()
                .Get("/precedence");

            Assert.That(this.GetLastRequestHeaderValues("User-Agent"), Is.EqualTo(new[] { "RequestAgent/2.0" }));
        }

        /// <summary>
        /// A test checking that the path part of the base URI is included in the request.
        /// </summary>
        /// <param name="baseUriPath">The path part of the base URI.</param>
        /// <param name="endpoint">The endpoint to use in the request.</param>
        [TestCase("/api", "/echo", TestName = "Path in base URI is included in request")]
        [TestCase("/api/", "/echo", TestName = "Path with trailing slash in base URI is included in request")]
        [TestCase("/api", "echo", TestName = "Path in base URI is included in request for endpoint without leading slash")]
        public void PathInBaseUriIsIncludedInRequest(string baseUriPath, string endpoint)
        {
            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri($"{MOCK_SERVER_BASE_URL}{baseUriPath}")
                .Build();

            Given()
                .Spec(requestSpecification)
                .When()
                .Get(endpoint);

            Assert.That(this.Server.LogEntries.Last().RequestMessage.Path, Is.EqualTo("/api/echo"));
        }

        /// <summary>
        /// A test checking that the path part of the base URI is combined with the base path,
        /// regardless of the order in which they are set.
        /// </summary>
        /// <param name="baseUriFirst">True to set the base URI before the base path, false to set it after.</param>
        [TestCase(true, TestName = "Path in base URI is combined with base path set after it")]
        [TestCase(false, TestName = "Path in base URI is combined with base path set before it")]
        public void PathInBaseUriIsCombinedWithBasePath(bool baseUriFirst)
        {
            var builder = new RequestSpecBuilder();

            if (baseUriFirst)
            {
                builder = builder.WithBaseUri($"{MOCK_SERVER_BASE_URL}/v1").WithBasePath("api");
            }
            else
            {
                builder = builder.WithBasePath("api").WithBaseUri($"{MOCK_SERVER_BASE_URL}/v1");
            }

            Given()
                .Spec(builder.Build())
                .When()
                .Get("/echo");

            Assert.That(this.Server.LogEntries.Last().RequestMessage.Path, Is.EqualTo("/v1/api/echo"));
        }

        /// <summary>
        /// A test checking that encoded characters in the path part of the base URI are not encoded a second time.
        /// </summary>
        [Test]
        public void EncodedCharactersInBaseUriPathAreNotEncodedTwice()
        {
            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri($"{MOCK_SERVER_BASE_URL}/my%20api")
                .Build();

            Given()
                .Spec(requestSpecification)
                .When()
                .Get("/echo");

            Assert.That(this.Server.LogEntries.Last().RequestMessage.Path, Is.EqualTo("/my api/echo"));
        }

        /// <summary>
        /// A test checking that the BaseUri property of the request specification reflects the configured base URI.
        /// </summary>
        [Test]
        public void BaseUriPropertyReflectsConfiguredBaseUri()
        {
            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri($"{MOCK_SERVER_BASE_URL}/api")
                .Build();

            Assert.That(requestSpecification.BaseUri, Is.EqualTo($"http://localhost:{MOCK_SERVER_PORT}/api"));
        }

        /// <summary>
        /// A test checking that the BaseUri property of the request specification reflects a port set after the base URI.
        /// </summary>
        [Test]
        public void BaseUriPropertyReflectsPortSetAfterBaseUri()
        {
            var requestSpecification = new RequestSpecBuilder()
                .WithBaseUri("http://localhost")
                .WithPort(1234)
                .Build();

            Assert.That(requestSpecification.BaseUri, Is.EqualTo("http://localhost:1234"));
        }

        /// <summary>
        /// Returns the values of a header in the last request received by the mock server.
        /// </summary>
        /// <param name="name">The name of the header (case-insensitive).</param>
        /// <returns>The values of the header, or an empty list if the header was not sent.</returns>
        private IEnumerable<string> GetLastRequestHeaderValues(string name)
        {
            var headers = this.Server.LogEntries.Last().RequestMessage.Headers;

            return headers
                .Where(header => string.Equals(header.Key, name, StringComparison.OrdinalIgnoreCase))
                .SelectMany(header => header.Value)
                .ToList();
        }

        /// <summary>
        /// Creates the stub response for an endpoint that requires the query parameter id=1.
        /// </summary>
        /// <param name="path">The path to create the stub for.</param>
        private void CreateStubForQueryStringEndpoint(string path)
        {
            this.Server?.Given(Request.Create().WithPath(path)
                .WithParam("id", "1")
                .UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(200));
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