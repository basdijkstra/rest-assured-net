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
    using System.Net.Http;
    using System.Xml.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Contains methods to log response details to the console.
    /// </summary>
    public class ResponseLogger
    {
        private readonly HttpResponseMessage response;
        private readonly TimeSpan elapsedTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseLogger"/> class.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> object to log details of.</param>
        /// <param name="elapsedTime">The time elapsed for between sending a request and receiving a response.</param>
        public ResponseLogger(HttpResponseMessage response, TimeSpan elapsedTime)
        {
            this.response = response;
            this.elapsedTime = elapsedTime;
        }

        /// <summary>
        /// Logs the status code and code description to the console.
        /// </summary>
        /// <returns>A <see cref="VerifiableResponse"/> that can be used for further response verification.</returns>
        public VerifiableResponse Status()
        {
            this.LogStatusCode();
            return new VerifiableResponse(this.response, this.elapsedTime);
        }

        /// <summary>
        /// Logs the status code and all response headers to the console.
        /// </summary>
        /// <returns>A <see cref="VerifiableResponse"/> that can be used for further response verification.</returns>
        public VerifiableResponse Headers()
        {
            this.LogStatusCode();
            this.LogHeaders();
            return new VerifiableResponse(this.response, this.elapsedTime);
        }

        /// <summary>
        /// Logs the status code and the response body to the console.
        /// </summary>
        /// <returns>A <see cref="VerifiableResponse"/> that can be used for further response verification.</returns>
        public VerifiableResponse Body()
        {
            this.LogStatusCode();
            this.LogBody();
            return new VerifiableResponse(this.response, this.elapsedTime);
        }

        /// <summary>
        /// Logs the status code and the response body to the console.
        /// </summary>
        /// <returns>A <see cref="VerifiableResponse"/> that can be used for further response verification.</returns>
        public VerifiableResponse Time()
        {
            this.LogStatusCode();
            this.LogTime();
            return new VerifiableResponse(this.response, this.elapsedTime);
        }

        /// <summary>
        /// Logs the status code, all response headers and the response body to the console.
        /// </summary>
        /// <returns>A <see cref="VerifiableResponse"/> that can be used for further response verification.</returns>
        public VerifiableResponse All()
        {
            this.LogStatusCode();
            this.LogHeaders();
            this.LogBody();
            this.LogTime();
            return new VerifiableResponse(this.response, this.elapsedTime);
        }

        /// <summary>
        /// Logs the response to the console with the requested <see cref="ResponseLogLevel"/>.
        /// </summary>
        /// <param name="response">The response to be logged to the console.</param>
        /// <param name="responseLogLevel">The <see cref="ResponseLogLevel"/> to use.</param>
        /// <param name="elapsedTime">The time elapsed between sending a request and returning a response.</param>
        internal static void Log(HttpResponseMessage response, ResponseLogLevel responseLogLevel, TimeSpan elapsedTime)
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
                    LogHeaders(response);
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
                LogHeaders(response);
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
                LogHeaders(response);
                LogBody(response);
                LogTime(elapsedTime);
            }
        }

        private static void LogStatusCode(HttpResponseMessage response)
        {
            Console.WriteLine($"HTTP {(int)response.StatusCode} ({response.StatusCode})");
        }

        private static void LogHeaders(HttpResponseMessage response)
        {
            if (response.Content != null)
            {
                // TODO: Add other headers to log
                Console.WriteLine($"Content-Type: {response.Content.Headers.ContentType}");
                Console.WriteLine($"Content-Length: {response.Content.Headers.ContentLength}");
            }

            foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
        }

        private static void LogBody(HttpResponseMessage response)
        {
            string responseBodyAsString = response.Content.ReadAsStringAsync().Result;

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

        private void LogStatusCode()
        {
            Console.WriteLine($"HTTP {(int)this.response.StatusCode} ({this.response.StatusCode})");
        }

        private void LogHeaders()
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in this.response.Content.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            foreach (KeyValuePair<string, IEnumerable<string>> header in this.response.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
        }

        private void LogTime()
        {
            Console.WriteLine($"Response time: {this.elapsedTime.TotalMilliseconds} ms");
        }

        private void LogBody()
        {
            string responseBodyAsString = this.response.Content.ReadAsStringAsync().Result;

            string responseMediaType = this.response.Content.Headers.ContentType?.MediaType ?? string.Empty;

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
    }
}
