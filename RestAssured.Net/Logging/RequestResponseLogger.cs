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
        /// Logs request details to the console.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to be logged to the console.</param>
        /// <param name="cookieCollection">The <see cref="CookieCollection"/> associated with this request.</param>
        public void LogRequest(HttpRequestMessage request, CookieCollection cookieCollection)
        {
            if (this.logConfiguration.RequestLogLevel >= RequestLogLevel.Endpoint)
            {
                this.logger.Log($"{request.Method} {request.RequestUri}");
            }

            if (this.logConfiguration.RequestLogLevel == RequestLogLevel.Headers)
            {
                this.LogRequestHeaders(request, this.logConfiguration.SensitiveRequestHeadersAndCookies);
                this.LogRequestCookies(cookieCollection, this.logConfiguration.SensitiveRequestHeadersAndCookies);
            }

            if (this.logConfiguration.RequestLogLevel == RequestLogLevel.Body)
            {
                this.LogRequestBody(request);
            }

            if (this.logConfiguration.RequestLogLevel == RequestLogLevel.All)
            {
                this.LogRequestHeaders(request, this.logConfiguration.SensitiveRequestHeadersAndCookies);
                this.LogRequestCookies(cookieCollection, this.logConfiguration.SensitiveRequestHeadersAndCookies);
                this.LogRequestBody(request);
            }
        }

        /// <summary>
        /// Logs response details to the console.
        /// </summary>
        /// <param name="verifiableResponse">The <see cref="VerifiableResponse"/> to log to the console.</param>
        /// <returns>The supplied <see cref="VerifiableResponse"/>, with the LogOnVerificationFailure flag set.</returns>
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

        private void LogRequestHeaders(HttpRequestMessage request, List<string> sensitiveRequestHeadersAndCookies)
        {
            this.LogMessageHeaders(request.Headers, request.Content, sensitiveRequestHeadersAndCookies);
        }

        private void LogRequestCookies(CookieCollection cookieCollection, List<string> sensitiveRequestHeadersAndCookies)
        {
            foreach (Cookie cookie in cookieCollection)
            {
                this.LogCookie(cookie, sensitiveRequestHeadersAndCookies);
            }
        }

        private void LogRequestBody(HttpRequestMessage request)
        {
            this.LogHttpContent(request.Content);
        }

        private void LogResponseStatusCode(HttpResponseMessage response)
        {
            this.logger.Log($"HTTP {(int)response.StatusCode} ({response.StatusCode})");
        }

        private void LogResponseHeaders(HttpResponseMessage response, List<string> sensitiveResponseHeadersAndCookies)
        {
            this.LogMessageHeaders(response.Headers, response.Content, sensitiveResponseHeadersAndCookies);
        }

        private void LogResponseCookies(CookieContainer cookieContainer, List<string> sensitiveResponseHeadersAndCookies)
        {
            var cookies = cookieContainer.GetAllCookies().GetEnumerator();

            while (cookies.MoveNext())
            {
                this.LogCookie((Cookie)cookies.Current, sensitiveResponseHeadersAndCookies);
            }
        }

        private void LogResponseBody(HttpResponseMessage response)
        {
            this.LogHttpContent(response.Content);
        }

        private void LogResponseTime(TimeSpan elapsedTime)
        {
            this.logger.Log($"Response time: {elapsedTime.TotalMilliseconds} ms");
        }

        private void LogMessageHeaders(HttpHeaders headers, HttpContent? content, List<string> sensitiveHeaders)
        {
            if (content != null)
            {
                this.logger.Log($"Content-Type: {content.Headers.ContentType}");
                this.logger.Log($"Content-Length: {content.Headers.ContentLength}");
            }

            foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
            {
                string value = sensitiveHeaders.Contains(header.Key) ? "*****" : string.Join(", ", header.Value);
                this.logger.Log($"{header.Key}: {value}");
            }
        }

        private void LogCookie(Cookie cookie, List<string> sensitiveNames)
        {
            string value = sensitiveNames.Contains(cookie.Name) ? "*****" : cookie.Value;
            this.logger.Log($"Cookie: {cookie.Name}={value}, Domain: {cookie.Domain}, HTTP-only: {cookie.HttpOnly}, Secure: {cookie.Secure}");
        }

        private void LogHttpContent(HttpContent? content)
        {
            if (content == null)
            {
                return;
            }

            string bodyAsString = content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!bodyAsString.Equals(string.Empty))
            {
                this.LogFormattedBody(bodyAsString, content.Headers.ContentType?.MediaType ?? string.Empty);
            }
        }

        private void LogFormattedBody(string bodyAsString, string mediaType)
        {
            if (mediaType.Equals(string.Empty) || mediaType.Contains("json"))
            {
                object jsonPayload = JsonConvert.DeserializeObject(bodyAsString, typeof(object)) ?? "Could not read payload";
                this.logger.Log(JsonConvert.SerializeObject(jsonPayload, Formatting.Indented));
            }
            else if (mediaType.Contains("xml"))
            {
                XDocument doc = XDocument.Parse(bodyAsString);
                this.logger.Log(doc.ToString());
            }
            else
            {
                this.logger.Log(bodyAsString);
            }
        }

        private VerifiableResponse LogResponseOnError(VerifiableResponse verifiableResponse)
        {
            if ((int)verifiableResponse.Response.StatusCode >= 400)
            {
                this.LogResponseStatusCode(verifiableResponse.Response);
                this.LogResponseHeaders(verifiableResponse.Response, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                this.LogResponseCookies(verifiableResponse.CookieContainer, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                this.LogResponseBody(verifiableResponse.Response);
                this.LogResponseTime(verifiableResponse.ElapsedTime);
            }

            return verifiableResponse;
        }

        private void LogResponseForLevel(VerifiableResponse verifiableResponse)
        {
            if (this.logConfiguration.ResponseLogLevel > ResponseLogLevel.None)
            {
                this.LogResponseStatusCode(verifiableResponse.Response);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.Headers)
            {
                this.LogResponseHeaders(verifiableResponse.Response, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                this.LogResponseCookies(verifiableResponse.CookieContainer, this.logConfiguration.SensitiveResponseHeadersAndCookies);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.Body)
            {
                this.LogResponseBody(verifiableResponse.Response);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.ResponseTime)
            {
                this.LogResponseTime(verifiableResponse.ElapsedTime);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.All)
            {
                this.LogResponseHeaders(verifiableResponse.Response, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                this.LogResponseCookies(verifiableResponse.CookieContainer, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                this.LogResponseBody(verifiableResponse.Response);
                this.LogResponseTime(verifiableResponse.ElapsedTime);
            }
        }
    }
}
