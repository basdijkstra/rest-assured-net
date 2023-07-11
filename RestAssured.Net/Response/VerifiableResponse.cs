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
namespace RestAssured.Response
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Xml;
    using System.Xml.Schema;
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NHamcrest;
    using NJsonSchema;
    using NJsonSchema.Validation;
    using RestAssured.Response.Deserialization;
    using RestAssured.Response.Exceptions;
    using RestAssured.Response.Logging;

    /// <summary>
    /// A class representing the response of an HTTP call.
    /// </summary>
    public class VerifiableResponse
    {
        private readonly HttpResponseMessage response;
        private readonly CookieContainer cookieContainer;
        private readonly TimeSpan elapsedTime;

        private bool logOnVerificationFailure = false;
        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
        private List<string> sensitiveResponseHeadersAndCookies = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifiableResponse"/> class.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> returned by the HTTP client.</param>
        /// <param name="cookieContainer">The <see cref="CookieContainer"/> used by the HTTP client.</param>
        /// <param name="elapsedTime">The time elapsed between sending the request and receiving the response.</param>
        public VerifiableResponse(HttpResponseMessage response, CookieContainer cookieContainer, TimeSpan elapsedTime)
        {
            this.response = response;
            this.cookieContainer = cookieContainer;
            this.elapsedTime = elapsedTime;
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
        /// Verifies that the actual status code is equal to an expected value.
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the actual status code does not match the expected one.</exception>
        public VerifiableResponse StatusCode(int expectedStatusCode)
        {
            if (expectedStatusCode != (int)this.response.StatusCode)
            {
                this.FailVerification($"Expected status code to be {expectedStatusCode}, but was {(int)this.response.StatusCode}");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the actual status code is equal to an expected value.
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the actual status code does not match the expected one.</exception>
        public VerifiableResponse StatusCode(HttpStatusCode expectedStatusCode)
        {
            if (!expectedStatusCode.Equals(this.response.StatusCode))
            {
                this.FailVerification($"Expected status code to be {expectedStatusCode}, but was {this.response.StatusCode}");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the actual status code is equal to an expected value.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the actual status code does not match the expected one.</exception>
        public VerifiableResponse StatusCode(IMatcher<int> matcher)
        {
            if (!matcher.Matches((int)this.response.StatusCode))
            {
                this.FailVerification($"Expected response status code to match '{matcher}', but was {(int)this.response.StatusCode}");
            }

            return this;
        }

        /// <summary>
        /// Verifies that a header exists in the response, with the expected value.
        /// </summary>
        /// <param name="name">The expected response header name.</param>
        /// <param name="expectedValue">The corresponding expected response header value.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the header does not exist, or when the header value does not equal the supplied expected value.</exception>
        public VerifiableResponse Header(string name, string expectedValue)
        {
            if (!this.response.Headers.TryGetValues(name, out IEnumerable<string>? values))
            {
                this.FailVerification($"Expected header with name '{name}' to be in the response, but it could not be found.");
            }

            string firstValue = values!.First();

            if (!firstValue.Equals(expectedValue))
            {
                this.FailVerification($"Expected value for response header with name '{name}' to be '{expectedValue}', but was '{firstValue}'.");
            }

            return this;
        }

        /// <summary>
        /// Verifies that a header exists in the response, with the expected value.
        /// </summary>
        /// <param name="name">The expected response header name.</param>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the header does not exist, or when the header value does not equal the supplied expected value.</exception>
        public VerifiableResponse Header(string name, IMatcher<string> matcher)
        {
            if (this.response.Headers.TryGetValues(name, out IEnumerable<string>? values))
            {
                string firstValue = values.First();

                if (!matcher.Matches(firstValue))
                {
                    this.FailVerification($"Expected value for response header with name '{name}' to match '{matcher}', but was '{firstValue}'.");
                }
            }
            else
            {
                this.FailVerification($"Expected header with name '{name}' to be in the response, but it could not be found.");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response Content-Type header has the expected value.
        /// </summary>
        /// <param name="expectedContentType">The expected value for the response Content-Type header.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the "Content-Type" header does not exist, or when the header value does not equal the supplied expected value.</exception>
        public VerifiableResponse ContentType(string expectedContentType)
        {
            MediaTypeHeaderValue? actualContentType = this.response.Content.Headers.ContentType;

            if (actualContentType == null)
            {
                this.FailVerification("Response Content-Type header could not be found.");
            }

            if (!actualContentType!.ToString().Equals(expectedContentType))
            {
                this.FailVerification($"Expected value for response Content-Type header to be '{expectedContentType}', but was '{actualContentType}'.");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response Content-Type header value matches a given NHamcrest matcher.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the "Content-Type" header does not exist, or when the header value does not equal the supplied expected value.</exception>
        public VerifiableResponse ContentType(IMatcher<string> matcher)
        {
            MediaTypeHeaderValue? actualContentType = this.response.Content.Headers.ContentType;

            if (actualContentType == null)
            {
                this.FailVerification("Response Content-Type header could not be found.");
            }

            if (!matcher.Matches(actualContentType!.ToString()))
            {
                this.FailVerification($"Expected value for response Content-Type header to match '{matcher}', but was '{actualContentType}'.");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the value of the cookie with the specified name matches a given NHamcrest matcher.
        /// </summary>
        /// <param name="name">The name of the cookie to verify.</param>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Cookie(string name, IMatcher<string> matcher)
        {
            var cookies = this.cookieContainer.GetAllCookies().GetEnumerator();

            while (cookies.MoveNext())
            {
                Cookie cookie = (Cookie)cookies.Current;
                if (cookie.Name.Equals(name))
                {
                    if (!matcher.Matches(cookie.Value))
                    {
                        this.FailVerification($"Expected value for cookie with name '{name}' to match '{matcher}', but was '{cookie.Value}'.");
                    }

                    return this;
                }
            }

            this.FailVerification($"Cookie with name '{name}' could not be found in the response.");

            return this;
        }

        /// <summary>
        /// Verifies that the response body is equal to the specified expected body.
        /// </summary>
        /// <param name="expectedResponseBody">The expected response body.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the actual response body does not match the expected one.</exception>
        public VerifiableResponse Body(string expectedResponseBody)
        {
            string actualResponseBody = this.response.Content.ReadAsStringAsync().Result;

            if (!actualResponseBody.Equals(expectedResponseBody))
            {
                this.FailVerification($"Actual response body did not match expected response body.\nExpected: '{expectedResponseBody}'\nActual: '{actualResponseBody}'");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response body matches the specified NHamcrest matcher.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the actual response body does not match the expected one.</exception>
        public VerifiableResponse Body(IMatcher<string> matcher)
        {
            string actualResponseBody = this.response.Content.ReadAsStringAsync().Result;

            if (!matcher.Matches(actualResponseBody))
            {
                this.FailVerification($"Actual response body expected to match '{matcher}' but didn't.\nActual: '{actualResponseBody}'");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response body matches the specified NHamcrest matcher.
        /// </summary>
        /// <typeparam name="T">The type of value that the matcher operates on.</typeparam>
        /// <param name="path">The JsonPath or XPath expression to evaluate.</param>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <param name="verifyAs">Indicates how to interpret the response.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Body<T>(string path, IMatcher<T> matcher, VerifyAs verifyAs = VerifyAs.UseResponseContentTypeHeaderValue)
        {
            string responseBodyAsString = this.response.Content.ReadAsStringAsync().Result;

            string? responseMediaType = string.Empty;

            switch (verifyAs)
            {
                case VerifyAs.UseResponseContentTypeHeaderValue:
                    {
                        responseMediaType = this.response.Content.Headers.ContentType?.MediaType;
                        break;
                    }

                case VerifyAs.Json:
                    {
                        responseMediaType = "application/json";
                        break;
                    }

                case VerifyAs.Xml:
                    {
                        responseMediaType = "application/xml";
                        break;
                    }

                case VerifyAs.Html:
                    {
                        responseMediaType = "text/html";
                        break;
                    }
            }

            if (responseMediaType!.Equals(string.Empty) || responseMediaType.Contains("json"))
            {
                JObject responseBodyAsJObject = JObject.Parse(responseBodyAsString);
                JToken? resultingElement = responseBodyAsJObject.SelectToken(path);

                if (resultingElement == null)
                {
                    this.FailVerification($"JsonPath expression '{path}' did not yield any results.");
                }

                if (!matcher.Matches(resultingElement!.ToObject<T>() !))
                {
                    this.FailVerification($"Expected element selected by '{path}' to match '{matcher}' but was '{resultingElement}'");
                }
            }
            else if (responseMediaType.Contains("xml"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseBodyAsString);
                XmlNode? xmlElement = xmlDoc.SelectSingleNode(path);

                if (xmlElement == null)
                {
                    this.FailVerification($"XPath expression '{path}' did not yield any results.");
                }

                // Try and cast the element value to an object of the type used in the matcher
                try
                {
                    T objectFromElementValue = (T)Convert.ChangeType(xmlElement!.InnerText, typeof(T));
                    if (!matcher.Matches((T)Convert.ChangeType(xmlElement.InnerText, typeof(T))))
                    {
                        this.FailVerification($"Expected element selected by '{path}' to match '{matcher}' but was '{xmlElement.InnerText}'");
                    }
                }
                catch (FormatException)
                {
                    this.FailVerification($"Response element value {xmlElement!.InnerText} cannot be converted to value of type '{typeof(T)}'");
                }
            }
            else if (responseMediaType.Contains("html"))
            {
                HtmlDocument responseBodyAsHtml = new HtmlDocument();
                responseBodyAsHtml.LoadHtml(responseBodyAsString);
                HtmlNode? htmlElement = responseBodyAsHtml.DocumentNode.SelectSingleNode(path);

                if (htmlElement == null)
                {
                    this.FailVerification($"XPath expression '{path}' did not yield any results.");
                }

                // Try and cast the element value to an object of the type used in the matcher
                try
                {
                    T objectFromElementValue = (T)Convert.ChangeType(htmlElement!.InnerText, typeof(T));
                    if (!matcher.Matches((T)Convert.ChangeType(htmlElement.InnerText, typeof(T))))
                    {
                        this.FailVerification($"Expected element selected by '{path}' to match '{matcher}' but was '{htmlElement.InnerText}'");
                    }
                }
                catch (FormatException)
                {
                    this.FailVerification($"Response element value {htmlElement!.InnerText} cannot be converted to value of type '{typeof(T)}'");
                }
            }
            else
            {
                this.FailVerification($"Unable to extract elements from response with Content-Type '{responseMediaType}'");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response body matches the specified NHamcrest matcher.
        /// </summary>
        /// <typeparam name="T">The type of value that the matcher operates on.</typeparam>
        /// <param name="path">The JsonPath expression to evaluate.</param>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <param name="verifyAs">Indicates how to interpret the response.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Body<T>(string path, IMatcher<IEnumerable<T>> matcher, VerifyAs verifyAs = VerifyAs.UseResponseContentTypeHeaderValue)
        {
            List<T> elementValues = new List<T>();

            string responseBodyAsString = this.response.Content.ReadAsStringAsync().Result;

            string? responseMediaType = string.Empty;

            switch (verifyAs)
            {
                case VerifyAs.UseResponseContentTypeHeaderValue:
                    {
                        responseMediaType = this.response.Content.Headers.ContentType?.MediaType;
                        break;
                    }

                case VerifyAs.Json:
                    {
                        responseMediaType = "application/json";
                        break;
                    }

                case VerifyAs.Xml:
                    {
                        responseMediaType = "application/xml";
                        break;
                    }

                case VerifyAs.Html:
                    {
                        responseMediaType = "text/html";
                        break;
                    }
            }

            if (responseMediaType!.Equals(string.Empty) || responseMediaType.Contains("json"))
            {
                JObject responseBodyAsJObject = JObject.Parse(responseBodyAsString);
                IEnumerable<JToken>? resultingElements = responseBodyAsJObject.SelectTokens(path);

                foreach (JToken element in resultingElements)
                {
                    elementValues.Add(element.ToObject<T>() !);
                }

                if (!matcher.Matches(elementValues))
                {
                    this.FailVerification($"Expected elements selected by '{path}' to match '{matcher}', but was [{string.Join(", ", elementValues)}]");
                }
            }
            else if (responseMediaType.Contains("xml"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseBodyAsString);
                XmlNodeList? xmlElements = xmlDoc.SelectNodes(path);

                // Try and cast the element values to an object of the type used in the matcher
                foreach (XmlNode xmlElement in xmlElements!)
                {
                    try
                    {
                        T objectFromElementValue = (T)Convert.ChangeType(xmlElement.InnerText, typeof(T));
                        elementValues.Add(objectFromElementValue);
                    }
                    catch (FormatException)
                    {
                        this.FailVerification($"Response element value {xmlElement.InnerText} cannot be converted to object of type {typeof(T)}");
                    }
                }

                if (!matcher.Matches(elementValues))
                {
                    this.FailVerification($"Expected elements selected by '{path}' to match '{matcher}', but was [{string.Join(", ", elementValues)}]");
                }
            }
            else if (responseMediaType.Contains("html"))
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(responseBodyAsString);
                HtmlNodeCollection? htmlElements = htmlDoc.DocumentNode.SelectNodes(path);

                // Try and cast the element values to an object of the type used in the matcher
                foreach (HtmlNode htmlElement in htmlElements!)
                {
                    try
                    {
                        T objectFromElementValue = (T)Convert.ChangeType(htmlElement.InnerText, typeof(T));
                        elementValues.Add(objectFromElementValue);
                    }
                    catch (FormatException)
                    {
                        this.FailVerification($"Response element value {htmlElement.InnerText} cannot be converted to object of type {typeof(T)}");
                    }
                }

                if (!matcher.Matches(elementValues))
                {
                    this.FailVerification($"Expected elements selected by '{path}' to match '{matcher}', but was [{string.Join(", ", elementValues)}]");
                }
            }
            else
            {
                this.FailVerification($"Unable to extract elements from response with Content-Type '{responseMediaType}'");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the JSON response body matches the supplied JSON schema.
        /// </summary>
        /// <param name="jsonSchema">The JSON schema to verify the response against.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when can't parse supplied JSON schema.</exception>
        public VerifiableResponse MatchesJsonSchema(string jsonSchema)
        {
            try
            {
                JsonSchema parsedSchema = JsonSchema.FromJsonAsync(jsonSchema).Result;
                return this.MatchesJsonSchema(parsedSchema);
            }
            catch (AggregateException ae)
            {
                foreach (Exception ex in ae.InnerExceptions)
                {
                    this.FailVerification($"Could not parse supplied JSON schema. Error: {ex.Message}");
                }
            }

            return this;
        }

        /// <summary>
        /// Verifies that the JSON response body matches the supplied JSON schema.
        /// </summary>
        /// <param name="jsonSchema">The JSON schema to verify the response against.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when "Content-Type" doesn't contain "json" or when body doesn't match JSON schema supplied.</exception>
        public VerifiableResponse MatchesJsonSchema(JsonSchema jsonSchema)
        {
            string responseMediaType = this.response.Content.Headers.ContentType?.MediaType ?? string.Empty;

            if (!responseMediaType.Contains("json"))
            {
                this.FailVerification($"Expected response Content-Type header to contain 'json', but was '{responseMediaType}'");
            }

            string responseBodyAsString = this.response.Content.ReadAsStringAsync().Result;

            ICollection<ValidationError> schemaValidationErrors = jsonSchema.Validate(responseBodyAsString);

            if (schemaValidationErrors.Count > 0)
            {
                this.FailVerification($"Response body did not match JSON schema supplied. Error: '{schemaValidationErrors.First()}'");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the XML response body matches the supplied XSD.
        /// </summary>
        /// <param name="xsd">The XSD to verify the response against.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse MatchesXsd(string xsd)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();

            try
            {
                schemas.Add(string.Empty, XmlReader.Create(new StringReader(xsd)));
            }
            catch (XmlSchemaException xse)
            {
                this.FailVerification($"Could not parse supplied XML schema. Error: {xse.Message}");
            }

            return this.MatchesXsd(schemas);
        }

        /// <summary>
        /// Verifies that the XML response body matches the supplied XSD.
        /// </summary>
        /// <param name="schemas">The <see cref="XmlSchemaSet"/> to verify the response against.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse MatchesXsd(XmlSchemaSet schemas)
        {
            string responseMediaType = this.response.Content.Headers.ContentType?.MediaType ?? string.Empty;

            if (!responseMediaType.Contains("xml"))
            {
                this.FailVerification($"Expected response Content-Type header to contain 'xml', but was '{responseMediaType}'");
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemas;

            string responseXmlAsString = this.response.Content.ReadAsStringAsync().Result;
            XmlReader reader = XmlReader.Create(new StringReader(responseXmlAsString), settings);

            try
            {
                while (reader.Read())
                {
                }
            }
            catch (XmlSchemaValidationException xsve)
            {
                this.FailVerification($"Response body did not match XML schema supplied. Error: '{xsve.Message}'");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the XML response body matches the inline DTD.
        /// </summary>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse MatchesInlineDtd()
        {
            string responseMediaType = this.response.Content.Headers.ContentType?.MediaType ?? string.Empty;

            if (!responseMediaType.Contains("xml"))
            {
                this.FailVerification($"Expected response Content-Type header to contain 'xml', but was '{responseMediaType}'");
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;

            string responseXmlAsString = this.response.Content.ReadAsStringAsync().Result;
            XmlReader reader = XmlReader.Create(new StringReader(responseXmlAsString), settings);

            try
            {
                while (reader.Read())
                {
                }
            }
            catch (XmlSchemaException xse)
            {
                this.FailVerification($"Response body did not match inline DTD. Error: '{xse.Message}'");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response time matches the specified NHamcrest matcher.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to match against the response time.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse ResponseTime(IMatcher<TimeSpan> matcher)
        {
            if (!matcher.Matches(this.elapsedTime))
            {
                this.FailVerification($"Expected response time to match '{matcher}' but was '{this.elapsedTime}'");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response body length (in bytes) matches the specified NHamcrest matcher.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to match against the response body length (in bytes).</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse ResponseBodyLength(IMatcher<int> matcher)
        {
            string responseContentAsString = this.response.Content.ReadAsStringAsync().Result;

            if (!matcher.Matches(responseContentAsString.Length))
            {
                this.FailVerification($"Expected response body length to match '{matcher}' but was '{responseContentAsString.Length}'");
            }

            return this;
        }

        /// <summary>
        /// Sets the <see cref="JsonSerializerSettings"/> to use when deserializing the response payload to JSON.
        /// </summary>
        /// <param name="jsonSerializerSettings">The <see cref="JsonSerializerSettings"/> to apply when deserializing.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse UsingJsonSerializerSettings(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings;
            return this;
        }

        /// <summary>
        /// Deserializes the response content into the specified type and returns it.
        /// </summary>
        /// <param name="type">The object type to deserialize into.</param>
        /// <param name="deserializeAs">Indicates how to interpret the response content when deserializing.</param>
        /// <returns>The deserialized response object.</returns>
        public object DeserializeTo(Type type, DeserializeAs deserializeAs = DeserializeAs.UseResponseContentTypeHeaderValue)
        {
            return Deserializer.DeserializeResponseInto(this.response, type, deserializeAs, this.jsonSerializerSettings);
        }

        /// <summary>
        /// Logs response details to the standard output.
        /// </summary>
        /// <param name="responseLogLevel">The required log level.</param>
        /// <param name="sensitiveHeaderOrCookieNames">The names of the response headers or cookies to be masked when logging.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Log(ResponseLogLevel responseLogLevel, List<string>? sensitiveHeaderOrCookieNames = null)
        {
            if (responseLogLevel == ResponseLogLevel.OnVerificationFailure)
            {
                this.logOnVerificationFailure = true;
                return this;
            }

            ResponseLogger.Log(this.response, this.cookieContainer, responseLogLevel, sensitiveHeaderOrCookieNames ?? new List<string>(), this.elapsedTime);
            return this;
        }

        /// <summary>
        /// Gives access to various methods to extract values from this response object.
        /// </summary>
        /// <returns>An <see cref="ExtractableResponse"/> object from which values can then be extracted.</returns>
        public ExtractableResponse Extract()
        {
            return new ExtractableResponse(this.response, this.cookieContainer, this.elapsedTime);
        }

        private void FailVerification(string exceptionMessage)
        {
            if (this.logOnVerificationFailure)
            {
                ResponseLogger.Log(this.response, this.cookieContainer, ResponseLogLevel.All, this.sensitiveResponseHeadersAndCookies, this.elapsedTime);
            }

            throw new ResponseVerificationException(exceptionMessage);
        }
    }
}
