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
        private LogConfiguration logConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestResponseLogger"/> class.
        /// </summary>
        /// <param name="logConfiguration">The <see cref="LogConfiguration"/> to use when logging request and response details.</param>
        public RequestResponseLogger(LogConfiguration logConfiguration)
        {
            this.logConfiguration = logConfiguration;
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
                Console.WriteLine($"{request.Method} {request.RequestUri}");
            }

            if (this.logConfiguration.RequestLogLevel == RequestLogLevel.Headers)
            {
                LogRequestHeaders(request, this.logConfiguration.SensitiveRequestHeadersAndCookies);
                LogRequestCookies(cookieCollection, this.logConfiguration.SensitiveRequestHeadersAndCookies);
            }

            if (this.logConfiguration.RequestLogLevel == RequestLogLevel.Body)
            {
                LogRequestBody(request);
            }

            if (this.logConfiguration.RequestLogLevel == RequestLogLevel.All)
            {
                LogRequestHeaders(request, this.logConfiguration.SensitiveRequestHeadersAndCookies);
                LogRequestCookies(cookieCollection, this.logConfiguration.SensitiveRequestHeadersAndCookies);
                LogRequestBody(request);
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

        private static void LogRequestHeaders(HttpRequestMessage request, List<string> sensitiveRequestHeadersAndCookies)
        {
            LogMessageHeaders(request.Headers, request.Content, sensitiveRequestHeadersAndCookies);
        }

        private static void LogRequestCookies(CookieCollection cookieCollection, List<string> sensitiveRequestHeadersAndCookies)
        {
            foreach (Cookie cookie in cookieCollection)
            {
                LogCookie(cookie, sensitiveRequestHeadersAndCookies);
            }
        }

        private static void LogRequestBody(HttpRequestMessage request)
        {
            LogHttpContent(request.Content);
        }

        private static void LogResponseStatusCode(HttpResponseMessage response)
        {
            Console.WriteLine($"HTTP {(int)response.StatusCode} ({response.StatusCode})");
        }

        private static void LogResponseHeaders(HttpResponseMessage response, List<string> sensitiveResponseHeadersAndCookies)
        {
            LogMessageHeaders(response.Headers, response.Content, sensitiveResponseHeadersAndCookies);
        }

        private static void LogResponseCookies(CookieContainer cookieContainer, List<string> sensitiveResponseHeadersAndCookies)
        {
            var cookies = cookieContainer.GetAllCookies().GetEnumerator();

            while (cookies.MoveNext())
            {
                LogCookie((Cookie)cookies.Current, sensitiveResponseHeadersAndCookies);
            }
        }

        private static void LogResponseBody(HttpResponseMessage response)
        {
            LogHttpContent(response.Content);
        }

        private static void LogResponseTime(TimeSpan elapsedTime)
        {
            Console.WriteLine($"Response time: {elapsedTime.TotalMilliseconds} ms");
        }

        private static void LogMessageHeaders(HttpHeaders headers, HttpContent? content, List<string> sensitiveHeaders)
        {
            if (content != null)
            {
                Console.WriteLine($"Content-Type: {content.Headers.ContentType}");
                Console.WriteLine($"Content-Length: {content.Headers.ContentLength}");
            }

            foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
            {
                string value = sensitiveHeaders.Contains(header.Key) ? "*****" : string.Join(", ", header.Value);
                Console.WriteLine($"{header.Key}: {value}");
            }
        }

        private static void LogCookie(Cookie cookie, List<string> sensitiveNames)
        {
            string value = sensitiveNames.Contains(cookie.Name) ? "*****" : cookie.Value;
            Console.WriteLine($"Cookie: {cookie.Name}={value}, Domain: {cookie.Domain}, HTTP-only: {cookie.HttpOnly}, Secure: {cookie.Secure}");
        }

        private static void LogHttpContent(HttpContent? content)
        {
            if (content == null)
            {
                return;
            }

            string bodyAsString = content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!bodyAsString.Equals(string.Empty))
            {
                LogFormattedBody(bodyAsString, content.Headers.ContentType?.MediaType ?? string.Empty);
            }
        }

        private static void LogFormattedBody(string bodyAsString, string mediaType)
        {
            if (mediaType.Equals(string.Empty) || mediaType.Contains("json"))
            {
                object jsonPayload = JsonConvert.DeserializeObject(bodyAsString, typeof(object)) ?? "Could not read payload";
                Console.WriteLine(JsonConvert.SerializeObject(jsonPayload, Formatting.Indented));
            }
            else if (mediaType.Contains("xml"))
            {
                XDocument doc = XDocument.Parse(bodyAsString);
                Console.WriteLine(doc.ToString());
            }
            else
            {
                Console.WriteLine(bodyAsString);
            }
        }

        private VerifiableResponse LogResponseOnError(VerifiableResponse verifiableResponse)
        {
            if ((int)verifiableResponse.Response.StatusCode >= 400)
            {
                LogResponseStatusCode(verifiableResponse.Response);
                LogResponseHeaders(verifiableResponse.Response, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                LogResponseCookies(verifiableResponse.CookieContainer, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                LogResponseBody(verifiableResponse.Response);
                LogResponseTime(verifiableResponse.ElapsedTime);
            }

            return verifiableResponse;
        }

        private void LogResponseForLevel(VerifiableResponse verifiableResponse)
        {
            if (this.logConfiguration.ResponseLogLevel > ResponseLogLevel.None)
            {
                LogResponseStatusCode(verifiableResponse.Response);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.Headers)
            {
                LogResponseHeaders(verifiableResponse.Response, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                LogResponseCookies(verifiableResponse.CookieContainer, this.logConfiguration.SensitiveResponseHeadersAndCookies);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.Body)
            {
                LogResponseBody(verifiableResponse.Response);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.ResponseTime)
            {
                LogResponseTime(verifiableResponse.ElapsedTime);
            }

            if (this.logConfiguration.ResponseLogLevel == ResponseLogLevel.All)
            {
                LogResponseHeaders(verifiableResponse.Response, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                LogResponseCookies(verifiableResponse.CookieContainer, this.logConfiguration.SensitiveResponseHeadersAndCookies);
                LogResponseBody(verifiableResponse.Response);
                LogResponseTime(verifiableResponse.ElapsedTime);
            }
        }
    }
}
