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
using System;
using System.Net.Http.Headers;

namespace RestAssured.Net.RA.Builders
{
    /// <summary>
    /// Class containing shared properties for requests.
    /// </summary>
    public class RequestSpecification
    {
        /// <summary>
        /// The scheme (http, https, ...) to be used when constructing the request.
        /// </summary>
        public string? Scheme { get; set; }

        /// <summary>
        /// The base URI to be used when constructing the request.
        /// </summary>
        public string? HostName { get; set; }

        /// <summary>
        /// The port number to be used when constructing the request.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The base path to be used when constructing the request.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// The timeout to be used when sending the request.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// The user agent to be used when sending the request.
        /// </summary>
        public ProductInfoHeaderValue? UserAgent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSpecification"/> class.
        /// </summary>
        /// <param name="scheme">The scheme (http, https, ....) to use in this request.</param>
        /// <param name="host">The host name to use in this request.</param>
        /// <param name="port">The port number to use in this request.</param>
        /// <param name="basePath">The base path to use in this request.</param>
        /// <param name="timeout">The timeout to use for this request.</param>
        /// <param name="userAgent">The user agent to use for this request.</param>
        public RequestSpecification(string scheme, string host, int port, string basePath, TimeSpan? timeout, ProductInfoHeaderValue? userAgent)
        {
            this.Scheme = scheme;
            this.HostName = host;
            this.Port = port;
            this.BasePath = basePath;
            this.Timeout = timeout;
            this.UserAgent = userAgent;
        }
    }
}
