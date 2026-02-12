// <copyright file="RequestContext.cs" company="On Test Automation">
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
namespace RestAssured.Request
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using Newtonsoft.Json;
    using RestAssured.Logging;
    using RestAssured.Request.Builders;

    /// <summary>
    /// Bundles all the state required to execute an HTTP request.
    /// </summary>
    internal class RequestContext
    {
        /// <summary>
        /// The <see cref="HttpClient"/> to use when sending the request, if any.
        /// </summary>
        internal HttpClient? HttpClient { get; set; }

        /// <summary>
        /// The <see cref="HttpRequestMessage"/> being built.
        /// </summary>
        internal HttpRequestMessage Request { get; set; } = new HttpRequestMessage();

        /// <summary>
        /// The <see cref="CookieCollection"/> to include in the request.
        /// </summary>
        internal CookieCollection CookieCollection { get; set; } = new CookieCollection();

        /// <summary>
        /// The optional <see cref="RequestSpecification"/> to apply.
        /// </summary>
        internal RequestSpecification? RequestSpecification { get; set; }

        /// <summary>
        /// Path parameters to substitute into the endpoint template.
        /// </summary>
        internal Dictionary<string, string> PathParams { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Query parameters to append to the request URI.
        /// </summary>
        internal IEnumerable<KeyValuePair<string, string>> QueryParams { get; set; } = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// The request body object.
        /// </summary>
        internal object RequestBody { get; set; } = string.Empty;

        /// <summary>
        /// The Content-Type header value.
        /// </summary>
        internal string ContentTypeHeader { get; set; } = "application/json";

        /// <summary>
        /// The content character encoding.
        /// </summary>
        internal Encoding ContentEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Whether to strip the charset from the Content-Type header.
        /// </summary>
        internal bool StripCharset { get; set; }

        /// <summary>
        /// The JSON serializer settings to use when serializing request bodies.
        /// </summary>
        internal JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings();

        /// <summary>
        /// Optional form data to send as x-www-form-urlencoded content.
        /// </summary>
        internal IEnumerable<KeyValuePair<string, string>>? FormData { get; set; }

        /// <summary>
        /// Optional multipart form data content.
        /// </summary>
        internal MultipartFormDataContent? MultipartFormDataContent { get; set; }

        /// <summary>
        /// Whether to disable SSL certificate validation for this request.
        /// </summary>
        internal bool DisableSslCertificateValidation { get; set; }

        /// <summary>
        /// The proxy to use for the request, if any.
        /// </summary>
        internal IWebProxy? Proxy { get; set; }

        /// <summary>
        /// The network credential for NTLM authentication, if any.
        /// </summary>
        internal NetworkCredential? NetworkCredential { get; set; }

        /// <summary>
        /// The custom timeout for the request, if any.
        /// </summary>
        internal TimeSpan? Timeout { get; set; }

        /// <summary>
        /// The <see cref="HttpCompletionOption"/> to use when sending the request.
        /// </summary>
        internal HttpCompletionOption HttpCompletionOption { get; set; } = HttpCompletionOption.ResponseContentRead;

        /// <summary>
        /// The request logging level.
        /// </summary>
        [Obsolete("Use LogConfiguration instead. Will be removed in RestAssured.Net 5.0.0")]
        internal Logging.RequestLogLevel RequestLoggingLevel { get; set; }

        /// <summary>
        /// The response logging level.
        /// </summary>
        [Obsolete("Use LogConfiguration instead. Will be removed in RestAssured.Net 5.0.0")]
        internal Response.Logging.ResponseLogLevel ResponseLoggingLevel { get; set; }

        /// <summary>
        /// Names of sensitive request headers and cookies to mask when logging.
        /// </summary>
        [Obsolete("Use LogConfiguration instead. Will be removed in RestAssured.Net 5.0.0")]
        internal List<string> SensitiveRequestHeadersAndCookies { get; set; } = new List<string>();

        /// <summary>
        /// The log configuration settings for this request.
        /// </summary>
        internal LogConfiguration? LogConfiguration { get; set; }
    }
}
