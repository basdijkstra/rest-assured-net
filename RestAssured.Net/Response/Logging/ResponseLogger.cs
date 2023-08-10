// <copyright file="ResponseLogger.cs" company="On Test Automation">
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

namespace RestAssured.Response.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Xml.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Contains methods to log response details to the console.
    /// </summary>
    public class ResponseLogger
    {
        /// <summary>
        /// Logs the response to the console with the requested <see cref="ResponseLogLevel"/>.
        /// </summary>
        /// <param name="response">The response to be logged to the console.</param>
        /// <param name="cookieContainer">The <see cref="CookieContainer"/> containing the cookies associated with the response.</param>
        /// <param name="responseLogLevel">The <see cref="ResponseLogLevel"/> to use.</param>
        /// <param name="sensitiveResponseHeadersAndCookies">A list of sensitive response headers and cookies to be masked when logging.</param>
        /// <param name="elapsedTime">The time elapsed between sending a request and returning a response.</param>
        internal static void Log(HttpResponseMessage response, CookieContainer cookieContainer, ResponseLogLevel responseLogLevel, List<string> sensitiveResponseHeadersAndCookies, TimeSpan elapsedTime)
        {
            if (responseLogLevel == ResponseLogLevel.OnVerificationFailure)
            {
                return;
            }

            if (responseLogLevel == ResponseLogLevel.OnError)
            {
                if ((int)response.StatusCode >= 400)
                {
                    LogStatusCode(response);
                    LogHeaders(response, sensitiveResponseHeadersAndCookies);
                    LogCookies(cookieContainer, sensitiveResponseHeadersAndCookies);
                    LogBody(response);
                    LogTime(elapsedTime);
                }

                return;
            }

            if (responseLogLevel > ResponseLogLevel.None)
            {
                LogStatusCode(response);
            }

            if (responseLogLevel == ResponseLogLevel.Headers)
            {
                LogHeaders(response, sensitiveResponseHeadersAndCookies);
                LogCookies(cookieContainer, sensitiveResponseHeadersAndCookies);
            }

            if (responseLogLevel == ResponseLogLevel.Body)
            {
                LogBody(response);
            }

            if (responseLogLevel == ResponseLogLevel.ResponseTime)
            {
                LogTime(elapsedTime);
            }

            if (responseLogLevel == ResponseLogLevel.All)
            {
                LogHeaders(response, sensitiveResponseHeadersAndCookies);
                LogCookies(cookieContainer, sensitiveResponseHeadersAndCookies);
                LogBody(response);
                LogTime(elapsedTime);
            }
        }

        private static void LogStatusCode(HttpResponseMessage response)
        {
            Console.WriteLine($"HTTP {(int)response.StatusCode} ({response.StatusCode})");
        }

        private static void LogHeaders(HttpResponseMessage response, List<string> sensitiveResponseHeadersAndCookies)
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

        private static void LogCookies(CookieContainer cookieContainer, List<string> sensitiveResponseHeadersAndCookies)
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

        private static void LogBody(HttpResponseMessage response)
        {
            if (response.Content == null)
            {
                return;
            }

            string responseBodyAsString = response.Content.ReadAsStringAsync().Result;

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

        private static void LogTime(TimeSpan elapsedTime)
        {
            Console.WriteLine($"Response time: {elapsedTime.TotalMilliseconds} ms");
        }
    }
}
