// <copyright file="RequestLogger.cs" company="On Test Automation">
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

namespace RestAssured.Request.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Contains methods to log request details to the console.
    /// </summary>
    public class RequestLogger
    {
        /// <summary>
        /// Logs request details to the console at the set <see cref="RequestLogLevel"/>.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to be logged to the console.</param>
        /// <param name="requestLogLevel">The <see cref="RequestLogLevel"/> to use when logging request details.</param>
        /// <param name="cookieCollection">The <see cref="CookieCollection"/> associated with this request.</param>
        /// <param name="sensitiveRequestHeadersAndCookies">A list of sensitive request headers and cookies to be masked when logging.</param>
        internal static void LogToConsole(HttpRequestMessage request, RequestLogLevel requestLogLevel, CookieCollection cookieCollection, List<string> sensitiveRequestHeadersAndCookies)
        {
            if (requestLogLevel >= RequestLogLevel.Endpoint)
            {
                Console.WriteLine($"{request.Method} {request.RequestUri}");
            }

            if (requestLogLevel == RequestLogLevel.Headers)
            {
                LogHeaders(request, sensitiveRequestHeadersAndCookies);
                LogCookies(cookieCollection, sensitiveRequestHeadersAndCookies);
            }

            if (requestLogLevel == RequestLogLevel.Body)
            {
                LogBody(request);
            }

            if (requestLogLevel == RequestLogLevel.All)
            {
                LogHeaders(request, sensitiveRequestHeadersAndCookies);
                LogCookies(cookieCollection, sensitiveRequestHeadersAndCookies);
                LogBody(request);
            }
        }

        private static void LogHeaders(HttpRequestMessage request, List<string> sensitiveRequestHeadersAndCookies)
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

        private static void LogCookies(CookieCollection cookieCollection, List<string> sensitiveRequestHeadersAndCookies)
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

        private static void LogBody(HttpRequestMessage request)
        {
            if (request.Content == null)
            {
                return;
            }

            string requestBodyAsString = request.Content.ReadAsStringAsync().Result;

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
    }
}
