// <copyright file="VerifiableResponse.cs" company="On Test Automation">
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHamcrest;
using RestAssured.Net.RA;
using RestAssuredNet.RA.Exceptions;

namespace RestAssuredNet.RA
{
    /// <summary>
    /// A class representing the response of an HTTP call.
    /// </summary>
    public class VerifiableResponse
    {
        private readonly HttpResponseMessage response;

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifiableResponse"/> class.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> returned by the HTTP client.</param>
        public VerifiableResponse(HttpResponseMessage response)
        {
            this.response = response;
        }

        /// <summary>
        /// Syntactic sugar (for now) that indicates the start of the 'Assert' part of a test.
        /// </summary>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Then()
        {
            return this;
        }

        /// <summary>
        /// Syntactic sugar that makes tests read more like natural language.
        /// </summary>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse AssertThat()
        {
            return this;
        }

        /// <summary>
        /// Syntactic sugar that makes tests read more like natural language.
        /// </summary>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse And()
        {
            return this;
        }

        /// <summary>
        /// A method to verify that the actual status code is equal to an expected value.
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="AssertionException">Thrown when the actual status code does not match the expected one.</exception>
        public VerifiableResponse StatusCode(int expectedStatusCode)
        {
            if (!(expectedStatusCode == (int)this.response.StatusCode))
            {
                throw new AssertionException($"Expected status code to be {expectedStatusCode}, but was {(int)this.response.StatusCode}");
            }

            return this;
        }

        /// <summary>
        /// A method to verify that the actual status code is equal to an expected value.
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="AssertionException">Thrown when the actual status code does not match the expected one.</exception>
        public VerifiableResponse StatusCode(HttpStatusCode expectedStatusCode)
        {
            if (!expectedStatusCode.Equals(this.response.StatusCode))
            {
                throw new AssertionException($"Expected status code to be {expectedStatusCode}, but was {this.response.StatusCode}");
            }

            return this;
        }

        /// <summary>
        /// A method to verify that the actual status code is equal to an expected value.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="AssertionException">Thrown when the actual status code does not match the expected one.</exception>
        public VerifiableResponse StatusCode(IMatcher<int> matcher)
        {
            if (!matcher.Matches((int)this.response.StatusCode))
            {
                throw new AssertionException($"Expected response status code to match '{matcher}', but was {(int)this.response.StatusCode}");
            }

            return this;
        }

        /// <summary>
        /// A method to verify that a header exists in the response, with the expected value.
        /// </summary>
        /// <param name="name">The expected response header name.</param>
        /// <param name="expectedValue">The corresponding expected response header value.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="AssertionException">Thrown when the header does not exist, or when the header value does not equal the supplied expected value.</exception>
        public VerifiableResponse Header(string name, string expectedValue)
        {
            IEnumerable<string> values;

            if (this.response.Headers.TryGetValues(name, out values))
            {
                if (!values.First().Equals(expectedValue))
                {
                    throw new AssertionException($"Expected value for response header with name '{name}' to be '{expectedValue}', but was '{values.First()}'.");
                }
            }
            else
            {
                throw new AssertionException($"Expected header with name '{name}' to be in the response, but it could not be found.");
            }

            return this;
        }

        /// <summary>
        /// A method to verify that a header exists in the response, with the expected value.
        /// </summary>
        /// <param name="name">The expected response header name.</param>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="AssertionException">Thrown when the header does not exist, or when the header value does not equal the supplied expected value.</exception>
        public VerifiableResponse Header(string name, IMatcher<string> matcher)
        {
            IEnumerable<string> values;

            if (this.response.Headers.TryGetValues(name, out values))
            {
                if (!matcher.Matches(values.First()))
                {
                    throw new AssertionException($"Expected value for response header with name '{name}' to match '{matcher}', but was '{values.First()}'.");
                }
            }
            else
            {
                throw new AssertionException($"Expected header with name '{name}' to be in the response, but it could not be found.");
            }

            return this;
        }

        /// <summary>
        /// A method to verify that the response Content-Type header has the expected value.
        /// </summary>
        /// <param name="expectedContentType">The expected value for the response Content-Type header.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse ContentType(string expectedContentType)
        {
            MediaTypeHeaderValue? actualContentType = this.response.Content.Headers.ContentType;

            if (actualContentType == null)
            {
                throw new AssertionException("Response Content-Type header could not be found.");
            }

            if (!actualContentType.ToString().Equals(expectedContentType))
            {
                throw new AssertionException($"Expected value for response Content-Type header to be '{expectedContentType}', but was '{actualContentType}'.");
            }

            return this;
        }

        /// <summary>
        /// A method to verify that the response Content-Type header value matches a given NHamcrest matcher.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse ContentType(IMatcher<string> matcher)
        {
            MediaTypeHeaderValue? actualContentType = this.response.Content.Headers.ContentType;

            if (actualContentType == null)
            {
                throw new AssertionException("Response Content-Type header could not be found.");
            }

            if (!matcher.Matches(actualContentType.ToString()))
            {
                throw new AssertionException($"Expected value for response Content-Type header to match '{matcher}', but was '{actualContentType}'.");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response body is equal to the specified expected body.
        /// </summary>
        /// <param name="expectedResponseBody">The expected response body.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Body(string expectedResponseBody)
        {
            string actualResponseBody = this.response.Content.ReadAsStringAsync().Result;

            if (!actualResponseBody.Equals(expectedResponseBody))
            {
                throw new AssertionException($"Actual response body did not match expected response body.\nExpected: {expectedResponseBody}\nActual: {actualResponseBody}");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response body matches the specified NHamcrest matcher.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Body(IMatcher<string> matcher)
        {
            string actualResponseBody = this.response.Content.ReadAsStringAsync().Result;

            if (!matcher.Matches(actualResponseBody))
            {
                throw new AssertionException($"Actual response body expected to match '{matcher}' but didn't.\nActual: {actualResponseBody}");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response body matches the specified NHamcrest matcher.
        /// </summary>
        /// <typeparam name="T">The type of object that the matcher operates on.</typeparam>
        /// <param name="jsonPath">The JsonPath expression to evaluate.</param>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Body<T>(string jsonPath, IMatcher<T> matcher)
        {
            string responseBodyAsString = this.response.Content.ReadAsStringAsync().Result;
            JObject responseBodyAsJObject = JObject.Parse(responseBodyAsString);
            JToken? resultingElement = responseBodyAsJObject.SelectToken(jsonPath);

            if (resultingElement == null)
            {
                throw new AssertionException($"JsonPath expression '{jsonPath}' did not yield any results.");
            }

            if (!matcher.Matches(resultingElement.ToObject<T>()))
            {
                throw new AssertionException($"Expected element selected by '{jsonPath}' to match '{matcher}' but was {resultingElement}");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response body matches the specified NHamcrest matcher.
        /// </summary>
        /// <typeparam name="T">The type of object that the matcher operates on.</typeparam>
        /// <param name="jsonPath">The JsonPath expression to evaluate.</param>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Body<T>(string jsonPath, IMatcher<IEnumerable<T>> matcher)
        {
            string responseBodyAsString = this.response.Content.ReadAsStringAsync().Result;
            JObject responseBodyAsJObject = JObject.Parse(responseBodyAsString);
            IEnumerable<JToken>? resultingElements = responseBodyAsJObject.SelectTokens(jsonPath);

            List<T> elementValues = new List<T>();

            foreach (JToken element in resultingElements)
            {
                elementValues.Add(element.ToObject<T>());
            }

            if (!matcher.Matches(elementValues))
            {
                throw new AssertionException($"Expected elements selected by '{jsonPath}' to match '{matcher}', but was [{string.Join(", ", elementValues)}]");
            }

            return this;
        }

        /// <summary>
        /// Deserializes the response content into the specified type and returns it.
        /// </summary>
        /// <param name="type">The object type to deserialize into.</param>
        /// <returns>The deserialized response object.</returns>
        public object As(Type type)
        {
            string responseBodyAsString = this.response.Content.ReadAsStringAsync().Result;

            if (responseBodyAsString == null)
            {
                throw new JsonSerializationException("Response content null or empty.");
            }

            return JsonConvert.DeserializeObject(this.response.Content.ReadAsStringAsync().Result, type);
        }

        /// <summary>
        /// Gives access to various methods to extract values from this response object.
        /// </summary>
        /// <returns>An <see cref="ExtractableResponse"/> object from which values can then be extracted.</returns>
        public ExtractableResponse Extract()
        {
            return new ExtractableResponse(this.response);
        }
    }
}
