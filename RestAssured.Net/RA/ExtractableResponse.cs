// <copyright file="ExtractableResponse.cs" company="On Test Automation">
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
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using RestAssuredNet.RA.Exceptions;

namespace RestAssured.Net.RA
{
    /// <summary>
    /// A class representing an <see cref="HttpResponseMessage"/> from which values can be extracted.
    /// </summary>
    public class ExtractableResponse
    {
        private readonly HttpResponseMessage response;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractableResponse"/> class.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> object from which values should be extracted.</param>
        public ExtractableResponse(HttpResponseMessage response)
        {
            this.response = response;
        }

        /// <summary>
        /// Extracts a response body element value from the response based on a JsonPath expression.
        /// </summary>
        /// <param name="jsonPath">The JsonPath expression pointing to the object to extract.</param>
        /// <returns>The element value or values extracted from the response using the JsonPath expression.</returns>
        /// <exception cref="AssertionException">Throws an AssertionException when evaluating the JsonPath did not yield any results.</exception>
        public object Body(string jsonPath)
        {
            string responseBodyAsString = this.response.Content.ReadAsStringAsync().Result;
            JObject responseBodyAsJObject = JObject.Parse(responseBodyAsString);
            IEnumerable<JToken>? resultingElements = responseBodyAsJObject.SelectTokens(jsonPath);

            List<object> elementValues = new List<object>();

            foreach (JToken element in resultingElements)
            {
                elementValues.Add(element.ToObject<object>());
            }

            if (elementValues.Count == 0)
            {
                throw new AssertionException($"JsonPath expression '{jsonPath}' did not yield any results.");
            }

            if (elementValues.Count == 1)
            {
                return elementValues.First();
            }

            return elementValues;
        }

        /// <summary>
        /// Returns the value for the specified header name from the response.
        /// </summary>
        /// <param name="name">The header to return.</param>
        /// <returns>The associated header value.</returns>
        /// <exception cref="ExtractionException">Thrown when the specified header name could not be located in the response.</exception>
        public string Header(string name)
        {
            IEnumerable<string> values;

            if (this.response.Headers.TryGetValues(name, out values))
            {
                return values.First();
            }
            else
            {
                throw new ExtractionException($"Header with name '{name}' could not be found in the response.");
            }
        }
    }
}
