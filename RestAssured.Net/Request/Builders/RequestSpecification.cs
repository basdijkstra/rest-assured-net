// <copyright file="RequestSpecification.cs" company="On Test Automation">
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
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Newtonsoft.Json;
    using RestAssured.Logging;

    /// <summary>
    /// Class containing shared properties for requests.
    /// </summary>
    public class RequestSpecification
    {
        /// <summary>
        /// The base URI to be used when constructing the request.
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// The port number to be used when constructing the request.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The base path to be used when constructing the request.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Query parameters to be appended to the endpoint when constructing the request.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> QueryParams { get; set; }

        /// <summary>
        /// The timeout to be used when sending the request.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// The user agent to be used when sending the request.
        /// </summary>
        public ProductInfoHeaderValue? UserAgent { get; set; }

        /// <summary>
        /// The proxy to be used when sending the request.
        /// </summary>
        public IWebProxy? Proxy { get; set; }

        /// <summary>
        /// The headers to be added when sending the request.
        /// </summary>
        public Dictionary<string, object> Headers { get; set; }

        /// <summary>
        /// Authentication details to be added when sending the request.
        /// </summary>
        public AuthenticationHeaderValue AuthenticationHeader { get; set; }

        /// <summary>
        /// The value for the Content-Type header when sending the request.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The value for the content encoding when sending the request.
        /// </summary>
        public Encoding ContentEncoding { get; set; }

        /// <summary>
        /// When set to true, disables SSL certificate validation on the requests that use this specification.
        /// </summary>
        public bool DisableSslCertificateValidation { get; set; }

        /// <summary>
        /// The configuration to be used when logging request and response details.
        /// </summary>
        public LogConfiguration LogConfiguration { get; set; }

        /// <summary>
        /// Can be used to provide custom serialization settings when working with JSON request payloads.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// Can be used to indicate whether the <see cref="HttpClient"/> should wait for the request body to be sent.
        /// </summary>
        public HttpCompletionOption HttpCompletionOption { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSpecification"/> class.
        /// </summary>
        /// <param name="baseUri">The base URI to use in this request.</param>
        /// <param name="port">The port number to use in this request.</param>
        /// <param name="basePath">The base path to use in this request.</param>
        /// <param name="queryParams">Query parameters to use in this request.</param>
        /// <param name="timeout">The timeout to use for this request.</param>
        /// <param name="userAgent">The user agent to use for this request.</param>
        /// <param name="proxy">The proxy to use for this request.</param>
        /// <param name="headers">The headers to add to this request.</param>
        /// <param name="authenticationHeader">The authentication header to add to this request.</param>
        /// <param name="contentType">The Content-Type header value to set for this request.</param>
        /// <param name="contentEncoding">The content encoding to use in this request.</param>
        /// <param name="disableSslCertificateValidation">Flag indicating whether or not to disable SSL certificate validation.</param>
        /// <param name="logConfiguration">The logging configuration to use when logging request and response details.</param>
        /// <param name="jsonSerializerSettings">The JSON serializer settings to use in this request.</param>
        /// <param name="httpCompletionOption">Indicates whether the HttpClient should wait for the request body to be sent.</param>
        public RequestSpecification(string baseUri, int port, string basePath, IEnumerable<KeyValuePair<string, string>> queryParams, TimeSpan? timeout, ProductInfoHeaderValue? userAgent, IWebProxy proxy, Dictionary<string, object> headers, AuthenticationHeaderValue authenticationHeader, string contentType, Encoding contentEncoding, bool disableSslCertificateValidation, LogConfiguration logConfiguration, JsonSerializerSettings jsonSerializerSettings, HttpCompletionOption httpCompletionOption)
        {
            this.BaseUri = baseUri;
            this.Port = port;
            this.BasePath = basePath;
            this.QueryParams = queryParams;
            this.Timeout = timeout;
            this.UserAgent = userAgent;
            this.Proxy = proxy;
            this.Headers = headers;
            this.AuthenticationHeader = authenticationHeader;
            this.ContentType = contentType;
            this.ContentEncoding = contentEncoding;
            this.DisableSslCertificateValidation = disableSslCertificateValidation;
            this.LogConfiguration = logConfiguration;
            this.JsonSerializerSettings = jsonSerializerSettings;
            this.HttpCompletionOption = httpCompletionOption;
        }
    }
}
