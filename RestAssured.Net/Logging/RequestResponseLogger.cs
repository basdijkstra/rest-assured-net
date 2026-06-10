// <copyright file="RequestResponseLogger.cs" company="On Test Automation">
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

namespace RestAssured.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Xml.Linq;
    using Newtonsoft.Json;
    using RestAssured.Response;

    /// <summary>
    /// Provides methods for logging request and response details.
    /// </summary>
    internal class RequestResponseLogger
    {
        private readonly LogConfiguration logConfiguration;
        private readonly IRestAssuredNetLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestResponseLogger"/> class.
        /// </summary>
        /// <param name="logConfiguration">The <see cref="LogConfiguration"/> to use when logging request and response details.</param>
        /// <param name="logger">The <see cref="IRestAssuredNetLogger"/> to use when writing log output.</param>
        public RequestResponseLogger(LogConfiguration logConfiguration, IRestAssuredNetLogger? logger = null)
        {
            this.logConfiguration = logConfiguration;
            this.logger = logger ?? new RestAssuredNetConsoleLogger();
        }

        /// <summary>
        /// Logs request details using the injected <see cref="IRestAssuredNetLogger"/>.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> whose details are to be logged.</param>
        /// <param name="cookieCollection">The <see cref="CookieCollection"/> containing cookies associated with the request.</param>
        public void LogRequest(HttpRequestMessage request, CookieCollection cookieCollection)
        {
            var entries = new List<string>();

            if (this.logConfiguration.RequestLogLevel >= RequestLogLevel.Endpoint)
            {
                entries.Add($"{request.Method} {request.RequestUri}");
            }

            if (this.logConfiguration.RequestLogLevel == RequestLogLevel.Headers)
            {
                entries = this.LogRequestHeaders(entries, request, this.logConfiguration.SensitiveRequestHeadersAndCookies);
                entries = this.LogRequestCookies(entries, cookieCollection, this.logConfiguration.SensitiveRequestHeadersAndCookies);
            }

            if (this.logConfiguration.RequestLogLevel == RequestLogLevel.Body)
            {
                entries = this.LogRequestBody(entries, request);
            }

            if (this.logConfiguration.RequestLogLevel == RequestLogLevel.All)
            {
                entries = this.LogRequestHeaders(entries, request, this.logConfiguration.SensitiveRequestHeadersAndCookies);
                entries = this.LogRequestCookies(entries, cookieCollection, this.logConfiguration.SensitiveRequestHeadersAndCookies);
                entries = this.LogRequestBody(entries, request);
            }

            this.logger.Log(string.Join(Environment.NewLine, entries));
        }

        /// <summary>
        /// Logs response details using the injected <see cref="IRestAssuredNetLogger"/>.
        /// </summary>
        /// <param name="verifiableResponse">The <see cref="VerifiableResponse"/> whose details are to be logged.</param>
        /// <returns>The supplied <see cref="VerifiableResponse"/>, with the LogOnVerificationFailure flag set when applicable.</returns>
        public VerifiableResponse LogResponse(VerifiableResponse verifiableResponse)
        {
            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.OnVerificationFailure)
            {
                verifiableResponse.LogOnVerificationFailure = true;
                return verifiableResponse;
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.OnError)
            {
                return this.LogResponseOnError(verifiableResponse);
            }

            this.LogResponseForLevel(verifiableResponse);
            return verifiableResponse;
        }

        private List<string> LogRequestHeaders(List<string> entries, HttpRequestMessage request, List<string> sensitiveRequestHeadersAndCookies)
        {
            return this.LogMessageHeaders(entries, request.Headers, request.Content, sensitiveRequestHeadersAndCookies);
        }

        private List<string> LogRequestCookies(List<string> entries, CookieCollection cookieCollection, List<string> sensitiveRequestHeadersAndCookies)
        {
            foreach (Cookie cookie in cookieCollection)
            {
                entries = this.LogCookie(entries, cookie, sensitiveRequestHeadersAndCookies);
            }

            return entries;
        }

        private List<string> LogRequestBody(List<string> entries, HttpRequestMessage request)
        {
            return this.LogHttpContent(entries, request.Content);
        }

        private List<string> LogResponseStatusCode(List<string> entries, HttpResponseMessage response)
        {
            entries.Add($"HTTP {(int)response.StatusCode} ({response.StatusCode})");
            return entries;
        }

        private List<string> LogResponseHeaders(List<string> entries, HttpResponseMessage response, List<string> sensitiveResponseHeadersAndCookies)
        {
            return this.LogMessageHeaders(entries, response.Headers, response.Content, sensitiveResponseHeadersAndCookies);
        }

        private List<string> LogResponseCookies(List<string> entries, CookieContainer cookieContainer, List<string> sensitiveResponseHeadersAndCookies)
        {
            var cookies = cookieContainer.GetAllCookies().GetEnumerator();

            while (cookies.MoveNext())
            {
                entries = this.LogCookie(entries, (Cookie)cookies.Current, sensitiveResponseHeadersAndCookies);
            }

            return entries;
        }

        private List<string> LogResponseBody(List<string> entries, HttpResponseMessage response)
        {
            return this.LogHttpContent(entries, response.Content);
        }

        private List<string> LogResponseTime(List<string> entries, TimeSpan elapsedTime)
        {
            entries.Add($"Response time: {elapsedTime.TotalMilliseconds} ms");
            return entries;
        }

        private List<string> LogMessageHeaders(List<string> entries, HttpHeaders headers, HttpContent? content, List<string> sensitiveHeaders)
        {
            if (content != null)
            {
                entries.Add($"Content-Type: {content.Headers.ContentType}");
                entries.Add($"Content-Length: {content.Headers.ContentLength}");
            }

            foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
            {
                string value = sensitiveHeaders.Contains(header.Key) ? "*****" : string.Join(", ", header.Value);
                entries.Add($"{header.Key}: {value}");
            }

            return entries;
        }

        private List<string> LogCookie(List<string> entries, Cookie cookie, List<string> sensitiveNames)
        {
            string value = sensitiveNames.Contains(cookie.Name) ? "*****" : cookie.Value;
            entries.Add($"Cookie: {cookie.Name}={value}, Domain: {cookie.Domain}, HTTP-only: {cookie.HttpOnly}, Secure: {cookie.Secure}");

            return entries;
        }

        private List<string> LogHttpContent(List<string> entries, HttpContent? content)
        {
            if (content == null)
            {
                return entries;
            }

            string bodyAsString = content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!bodyAsString.Equals(string.Empty))
            {
                entries = this.LogFormattedBody(entries, bodyAsString, content.Headers.ContentType?.MediaType ?? string.Empty);
            }

            return entries;
        }

        private List<string> LogFormattedBody(List<string> entries, string bodyAsString, string mediaType)
        {
            if (mediaType.Equals(string.Empty) || mediaType.Contains("json"))
            {
                object jsonPayload = JsonConvert.DeserializeObject(bodyAsString, typeof(object)) ?? "Could not read payload";
                entries.Add(JsonConvert.SerializeObject(jsonPayload, Formatting.Indented));
            }
            else if (mediaType.Contains("xml"))
            {
                XDocument doc = XDocument.Parse(bodyAsString);
                entries.Add(doc.ToString());
            }
            else
            {
                entries.Add(bodyAsString);
            }

            return entries;
        }

        private VerifiableResponse LogResponseOnError(VerifiableResponse verifiableResponse)
        {
            var entries = new List<string>();

            if ((int)verifiableResponse.Response.StatusCode >= 400)
            {
                entries = this.LogResponseStatusCode(entries, verifiableResponse.Response);
                entries = this.LogResponseHeaders(entries, verifiableResponse.Response, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                entries = this.LogResponseCookies(entries, verifiableResponse.CookieContainer, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                entries = this.LogResponseBody(entries, verifiableResponse.Response);
                entries = this.LogResponseTime(entries, verifiableResponse.ElapsedTime);
            }

            this.logger.Log(string.Join(Environment.NewLine, entries));

            return verifiableResponse;
        }

        private void LogResponseForLevel(VerifiableResponse verifiableResponse)
        {
            var entries = new List<string>();

            if (this.logConfiguration.ResponseLogLevel > ResponseLogLevel.None)
            {
                entries = this.LogResponseStatusCode(entries, verifiableResponse.Response);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.Headers)
            {
                entries = this.LogResponseHeaders(entries, verifiableResponse.Response, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                entries = this.LogResponseCookies(entries, verifiableResponse.CookieContainer, this.logConfiguration.SensitiveResponseHeadersAndCookies);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.Body)
            {
                entries = this.LogResponseBody(entries, verifiableResponse.Response);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.ResponseTime)
            {
                entries = this.LogResponseTime(entries, verifiableResponse.ElapsedTime);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.All)
            {
                entries = this.LogResponseHeaders(entries, verifiableResponse.Response, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                entries = this.LogResponseCookies(entries, verifiableResponse.CookieContainer, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                entries = this.LogResponseBody(entries, verifiableResponse.Response);
                entries = this.LogResponseTime(entries, verifiableResponse.ElapsedTime);
            }

            this.logger.Log(string.Join(Environment.NewLine, entries));
        }
    }
}
