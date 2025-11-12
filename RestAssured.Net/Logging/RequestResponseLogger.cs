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

            return verifiableResponse;
        }

        private static void LogRequestHeaders(HttpRequestMessage request, List<string> sensitiveRequestHeadersAndCookies)
        {
            if (request.Content != null)
            {
                Console.WriteLine($"Content-Type: {request.Content.Headers.ContentType}");
                Console.WriteLine($"Content-Length: {request.Content.Headers.ContentLength}");
            }

            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
            {
                if (sensitiveRequestHeadersAndCookies.Contains(header.Key))
                {
                    Console.WriteLine($"{header.Key}: *****");
                }
                else
                {
                    Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }
            }
        }

        private static void LogRequestCookies(CookieCollection cookieCollection, List<string> sensitiveRequestHeadersAndCookies)
        {
            foreach (Cookie cookie in cookieCollection)
            {
                if (sensitiveRequestHeadersAndCookies.Contains(cookie.Name))
                {
                    Console.WriteLine($"Cookie: {cookie.Name}=*****, Domain: {cookie.Domain}, HTTP-only: {cookie.HttpOnly}, Secure: {cookie.Secure}");
                }
                else
                {
                    Console.WriteLine($"Cookie: {cookie.Name}={cookie.Value}, Domain: {cookie.Domain}, HTTP-only: {cookie.HttpOnly}, Secure: {cookie.Secure}");
                }
            }
        }

        private static void LogRequestBody(HttpRequestMessage request)
        {
            if (request.Content == null)
            {
                return;
            }

            string requestBodyAsString = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (requestBodyAsString.Equals(string.Empty))
            {
                return;
            }

            string requestMediaType = request.Content.Headers.ContentType?.MediaType ?? string.Empty;

            if (requestMediaType.Equals(string.Empty) || requestMediaType.Contains("json"))
            {
                object jsonPayload = JsonConvert.DeserializeObject(requestBodyAsString, typeof(object)) ?? "Could not read request payload";
                Console.WriteLine(JsonConvert.SerializeObject(jsonPayload, Formatting.Indented));
            }
            else if (requestMediaType.Contains("xml"))
            {
                XDocument doc = XDocument.Parse(requestBodyAsString);
                Console.WriteLine(doc.ToString());
            }
            else
            {
                Console.WriteLine(requestBodyAsString);
            }
        }

        private static void LogResponseStatusCode(HttpResponseMessage response)
        {
            Console.WriteLine($"HTTP {(int)response.StatusCode} ({response.StatusCode})");
        }

        private static void LogResponseHeaders(HttpResponseMessage response, List<string> sensitiveResponseHeadersAndCookies)
        {
            if (response.Content != null)
            {
                Console.WriteLine($"Content-Type: {response.Content.Headers.ContentType}");
                Console.WriteLine($"Content-Length: {response.Content.Headers.ContentLength}");
            }

            foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
            {
                if (sensitiveResponseHeadersAndCookies.Contains(header.Key))
                {
                    Console.WriteLine($"{header.Key}: *****");
                }
                else
                {
                    Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }
            }
        }

        private static void LogResponseCookies(CookieContainer cookieContainer, List<string> sensitiveResponseHeadersAndCookies)
        {
            var cookies = cookieContainer.GetAllCookies().GetEnumerator();

            while (cookies.MoveNext())
            {
                Cookie cookie = (Cookie)cookies.Current;

                if (sensitiveResponseHeadersAndCookies.Contains(cookie.Name))
                {
                    Console.WriteLine($"Cookie: {cookie.Name}=*****, Domain: {cookie.Domain}, HTTP-only: {cookie.HttpOnly}, Secure: {cookie.Secure}");
                }
                else
                {
                    Console.WriteLine($"Cookie: {cookie.Name}={cookie.Value}, Domain: {cookie.Domain}, HTTP-only: {cookie.HttpOnly}, Secure: {cookie.Secure}");
                }
            }
        }

        private static void LogResponseBody(HttpResponseMessage response)
        {
            if (response.Content == null)
            {
                return;
            }

            string responseBodyAsString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (responseBodyAsString.Equals(string.Empty))
            {
                return;
            }

            string responseMediaType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;

            if (responseMediaType.Equals(string.Empty) || responseMediaType.Contains("json"))
            {
                object jsonPayload = JsonConvert.DeserializeObject(responseBodyAsString, typeof(object)) ?? "Could not read response payload";
                Console.WriteLine(JsonConvert.SerializeObject(jsonPayload, Formatting.Indented));
            }
            else if (responseMediaType.Contains("xml"))
            {
                XDocument doc = XDocument.Parse(responseBodyAsString);
                Console.WriteLine(doc.ToString());
            }
            else
            {
                Console.WriteLine(responseBodyAsString);
            }
        }

        private static void LogResponseTime(TimeSpan elapsedTime)
        {
            Console.WriteLine($"Response time: {elapsedTime.TotalMilliseconds} ms");
        }
    }
}
