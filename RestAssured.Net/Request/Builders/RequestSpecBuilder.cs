// <copyright file="RequestSpecBuilder.cs" company="On Test Automation">
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
namespace RestAssured.Request.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;
    using Newtonsoft.Json;
    using RestAssured.Request.Logging;

    /// <summary>
    /// A builder class to construct a new instance of the <see cref="RequestSpecification"/> class.
    /// </summary>
    public class RequestSpecBuilder
    {
        private readonly RequestSpecification requestSpecification;

        private readonly string scheme = "http";
        private readonly string host = "localhost";
        private readonly int port = -1;  // -1 means the default port for the scheme will be chosen
        private readonly string basePath = string.Empty;
        private readonly TimeSpan? timeout;
        private readonly ProductInfoHeaderValue? userAgent;
        private readonly IWebProxy? proxy;
        private readonly Dictionary<string, object> headers = new Dictionary<string, object>();
        private readonly AuthenticationHeaderValue? authenticationHeader;
        private readonly string? contentTypeHeader = null;
        private readonly Encoding? contentEncoding = null;
        private readonly bool disableSslCertificateValidation = false;
        private readonly RequestLogLevel requestLogLevel = RequestLogLevel.None;
        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
        private readonly List<string> sensitiveRequestHeadersAndCookies = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSpecBuilder"/> class.
        /// </summary>
        public RequestSpecBuilder()
        {
            this.requestSpecification = new RequestSpecification(this.scheme, this.host, this.port, this.basePath, this.timeout, this.userAgent, this.proxy, this.headers, this.authenticationHeader, this.contentTypeHeader, this.contentEncoding, this.disableSslCertificateValidation, this.requestLogLevel, this.jsonSerializerSettings, this.sensitiveRequestHeadersAndCookies);
        }

        /// <summary>
        /// Sets the scheme (http or https) on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="scheme">The scheme to use in the request.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithScheme(string scheme)
        {
            this.requestSpecification.Scheme = scheme;
            return this;
        }

        /// <summary>
        /// Sets the host name on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="host">The host name to use in the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithHostName(string host)
        {
            this.requestSpecification.HostName = host;
            return this;
        }

        /// <summary>
        /// Sets the port on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="port">The port to use in the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithPort(int port)
        {
            this.requestSpecification.Port = port;
            return this;
        }

        /// <summary>
        /// Sets the base path on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="basePath">The base path to use in the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithBasePath(string basePath)
        {
            this.requestSpecification.BasePath = basePath;
            return this;
        }

        /// <summary>
        /// Sets the timeout on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="timeout">The timeout to use for the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithTimeout(TimeSpan timeout)
        {
            this.requestSpecification.Timeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets the user agent on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="userAgent">The user agent to use for the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithUserAgent(ProductInfoHeaderValue userAgent)
        {
            this.requestSpecification.UserAgent = userAgent;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IWebProxy"/> on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="proxy">The <see cref="IWebProxy"/> to use for the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithProxy(IWebProxy proxy)
        {
            this.requestSpecification.Proxy = proxy;
            return this;
        }

        /// <summary>
        /// Sets the user agent on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="productName">The user agent product name to use for the requests.</param>
        /// <param name="productVersion">The user agent product version to use for the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithUserAgent(string productName, string productVersion)
        {
            this.requestSpecification.UserAgent = new ProductInfoHeaderValue(productName, productVersion);
            return this;
        }

        /// <summary>
        /// Adds a header on the <see cref="RequestSpecification"/> to build.
        /// </summary>
        /// <param name="key">The header name to add.</param>
        /// <param name="value">The associated header value.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithHeader(string key, object value)
        {
            this.requestSpecification.Headers[key] = value;
            return this;
        }

        /// <summary>
        /// Adds a basic authentication header to the request.
        /// </summary>
        /// <param name="username">The username to be used for authentication.</param>
        /// <param name="password">The password to be used for authentication.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithBasicAuth(string username, string password)
        {
            string base64EncodedBasicAuthDetails = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            this.requestSpecification.AuthenticationHeader = new AuthenticationHeaderValue("Basic", base64EncodedBasicAuthDetails);
            return this;
        }

        /// <summary>
        /// Adds an OAuth2 authentication header to the request.
        /// </summary>
        /// <param name="token">The OAuth2 token to be used for authentication.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithOAuth2(string token)
        {
            this.requestSpecification.AuthenticationHeader = new AuthenticationHeaderValue("Bearer", token);
            return this;
        }

        /// <summary>
        /// Adds the value for the Content-Type header to the request.
        /// </summary>
        /// <param name="contentType">The value for the Content-Type header.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithContentType(string contentType)
        {
            this.requestSpecification.ContentType = contentType;
            return this;
        }

        /// <summary>
        /// Adds the value for the content encoding to the request.
        /// </summary>
        /// <param name="encoding">The value for the content encoding.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithContentEncoding(Encoding encoding)
        {
            this.requestSpecification.ContentEncoding = encoding;
            return this;
        }

        /// <summary>
        /// Disables SSL certificate validation for the requests that use this specification.
        /// </summary>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithDisabledSslCertificateValidation()
        {
            this.requestSpecification.DisableSslCertificateValidation = true;
            return this;
        }

        /// <summary>
        /// Sets the request log level to the specified <see cref="RequestLogLevel"/> value.
        /// </summary>
        /// <param name="requestLogLevel">The <see cref="RequestLogLevel"/> to apply to the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithRequestLogLevel(RequestLogLevel requestLogLevel)
        {
            this.requestSpecification.RequestLogLevel = requestLogLevel;
            return this;
        }

        /// <summary>
        /// Sets the JSON serializer settings to the specified <see cref="JsonSerializerSettings"/> value.
        /// </summary>
        /// <param name="jsonSerializerSettings">The <see cref="JsonSerializerSettings"/> to use in the requests.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithJsonSerializerSettings(JsonSerializerSettings jsonSerializerSettings)
        {
            this.requestSpecification.JsonSerializerSettings = jsonSerializerSettings;
            return this;
        }

        /// <summary>
        /// Adds a list of request header or cookie names that should be masked when logging to the list.
        /// </summary>
        /// <param name="sensitiveHeaderOrCookieNames">The names of the request headers or cookies to be masked when logging.</param>
        /// <returns>The current <see cref="RequestSpecBuilder"/> object.</returns>
        public RequestSpecBuilder WithMaskingOfHeadersAndCookies(List<string> sensitiveHeaderOrCookieNames)
        {
            this.requestSpecification.SensitiveRequestHeadersAndCookies.AddRange(sensitiveHeaderOrCookieNames);
            return this;
        }

        /// <summary>
        /// Returns the <see cref="RequestSpecification"/> that was built.
        /// </summary>
        /// <returns>The <see cref="RequestSpecification"/> object built in this builder class.</returns>
        public RequestSpecification Build()
        {
            return this.requestSpecification;
        }
    }
}