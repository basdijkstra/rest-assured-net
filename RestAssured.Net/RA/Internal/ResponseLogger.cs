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
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json;
using RestAssuredNet.RA;

namespace RestAssured.Net.RA.Internal
{
    /// <summary>
    /// Contains methods to log response details to the console.
    /// </summary>
    public class ResponseLogger
    {
        private readonly HttpResponseMessage response;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseLogger"/> class.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> object to log details of.</param>
        public ResponseLogger(HttpResponseMessage response)
        {
            this.response = response;
        }

        /// <summary>
        /// Logs the status code and code description to the console.
        /// </summary>
        /// <returns>A <see cref="VerifiableResponse"/> that can be used for further response verification.</returns>
        public VerifiableResponse Status()
        {
            this.LogStatusCode();
            return new VerifiableResponse(this.response);
        }

        /// <summary>
        /// Logs the status code and all response headers to the console.
        /// </summary>
        /// <returns>A <see cref="VerifiableResponse"/> that can be used for further response verification.</returns>
        public VerifiableResponse Headers()
        {
            this.LogStatusCode();
            this.LogHeaders();
            return new VerifiableResponse(this.response);
        }

        /// <summary>
        /// Logs the status code and the response body to the console.
        /// </summary>
        /// <returns>A <see cref="VerifiableResponse"/> that can be used for further response verification.</returns>
        public VerifiableResponse Body()
        {
            this.LogStatusCode();
            this.LogBody();
            return new VerifiableResponse(this.response);
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
            return new VerifiableResponse(this.response);
        }

        private void LogStatusCode()
        {
            Console.WriteLine($"HTTP {(int)this.response.StatusCode} ({this.response.StatusCode})");
        }

        private void LogHeaders()
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in this.response.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
        }

        private void LogBody()
        {
            string responseBodyAsString = this.response.Content.ReadAsStringAsync().Result;

            string responseMediaType = this.response.Content.Headers.ContentType.MediaType ?? string.Empty;

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
