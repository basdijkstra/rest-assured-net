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
    using RestAssured.Logging;
    using RestAssured.Response.ContentType;
    using RestAssured.Response.Deserialization;
    using RestAssured.Response.Exceptions;
    using RestAssured.Response.Logging;

    /// <summary>
    /// A class representing the response of an HTTP call.
    /// </summary>
    public class VerifiableResponse
    {
        /// <summary>
        /// The wrapped <see cref="HttpResponseMessage"/> contained in this <see cref="VerifiableResponse"/>.
        /// </summary>
        public HttpResponseMessage Response { internal get; init; }

        /// <summary>
        /// The <see cref="CookieContainer"/> associated with the current <see cref="VerifiableResponse"/>.
        /// </summary>
        public CookieContainer CookieContainer { internal get; init; }

        /// <summary>
        /// The <see cref="TimeSpan"/> elapsed between sending a request and receiving this <see cref="VerifiableResponse"/>.
        /// </summary>
        public TimeSpan ElapsedTime { internal get; init; }

        /// <summary>
        /// A boolean flag indicating whether response details should be logged to the console on verification failure.
        /// </summary>
        public bool LogOnVerificationFailure { get; internal set; } = false;

        /// <summary>
        /// The <see cref="IRestAssuredNetLogger"/> to use when logging on verification failure.
        /// </summary>
        internal IRestAssuredNetLogger? Logger { get; set; }

        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();

        /// <summary>
        /// Initializes a new instance of the <see cref="VerifiableResponse"/> class.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> returned by the HTTP client.</param>
        /// <param name="cookieContainer">The <see cref="System.Net.CookieContainer"/> used by the HTTP client.</param>
        /// <param name="elapsedTime">The time elapsed between sending the request and receiving the response.</param>
        public VerifiableResponse(HttpResponseMessage response, CookieContainer cookieContainer, TimeSpan elapsedTime)
        {
            this.Response = response;
            this.CookieContainer = cookieContainer;
            this.ElapsedTime = elapsedTime;
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
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the actual status code does not match the expected one.</exception>
        public VerifiableResponse StatusCode(int expectedStatusCode, ErrorMessage errorMessage = default)
        {
            return this.StatusCode(Is.EqualTo(expectedStatusCode), errorMessage);
        }

        /// <summary>
        /// Verifies that the actual status code is equal to an expected value.
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the actual status code does not match the expected one.</exception>
        public VerifiableResponse StatusCode(HttpStatusCode expectedStatusCode, ErrorMessage errorMessage = default)
        {
            return this.StatusCode(Is.EqualTo(expectedStatusCode), errorMessage);
        }

        /// <summary>
        /// Verifies that the actual status code is equal to an expected value.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the actual status code does not match the expected one.</exception>
        public VerifiableResponse StatusCode(IMatcher<int> matcher, ErrorMessage errorMessage = default)
        {
            this.VerifyWithMatcher(matcher, (int)this.Response.StatusCode, $"Expected response status code to match '{matcher}', but was {(int)this.Response.StatusCode}", errorMessage);
            return this;
        }

        /// <summary>
        /// Verifies that the actual status code is equal to an expected value.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the actual status code does not match the expected one.</exception>
        public VerifiableResponse StatusCode(IMatcher<HttpStatusCode> matcher, ErrorMessage errorMessage = default)
        {
            this.VerifyWithMatcher(matcher, this.Response.StatusCode, $"Expected response status code to match '{matcher}', but was {this.Response.StatusCode}", errorMessage);
            return this;
        }

        /// <summary>
        /// Verifies that a header exists in the response, with the expected value.
        /// </summary>
        /// <param name="name">The expected response header name.</param>
        /// <param name="expectedValue">The corresponding expected response header value.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the header does not exist, or when the header value does not equal the supplied expected value.</exception>
        public VerifiableResponse Header(string name, string expectedValue, ErrorMessage errorMessage = default)
        {
            return this.Header(name, Is.EqualTo(expectedValue), errorMessage);
        }

        /// <summary>
        /// Verifies that a header exists in the response, with the expected value.
        /// </summary>
        /// <param name="name">The expected response header name.</param>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the header does not exist, or when the header value does not equal the supplied expected value.</exception>
        public VerifiableResponse Header(string name, IMatcher<string> matcher, ErrorMessage errorMessage = default)
        {
            if (this.Response.Headers.TryGetValues(name, out IEnumerable<string>? values))
            {
                string firstValue = values.First();

                if (!matcher.Matches(firstValue))
                {
                    this.FailVerification(errorMessage.HasValue
                        ? AssertionMessageBuilder.BuildMessage(errorMessage.Value!, matcher, firstValue)
                        : $"Expected value for response header with name '{name}' to match '{matcher}', but was '{firstValue}'.");
                }
            }
            else
            {
                this.FailVerification(errorMessage.HasValue
                    ? errorMessage.Value!
                    : $"Expected header with name '{name}' to be in the response, but it could not be found.");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the response Content-Type header has the expected value.
        /// </summary>
        /// <param name="expectedContentType">The expected value for the response Content-Type header.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the "Content-Type" header does not exist, or when the header value does not equal the supplied expected value.</exception>
        public VerifiableResponse ContentType(string expectedContentType, ErrorMessage errorMessage = default)
        {
            return this.ContentType(Is.EqualTo(expectedContentType), errorMessage);
        }

        /// <summary>
        /// Verifies that the response Content-Type header value matches a given NHamcrest matcher.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the "Content-Type" header does not exist, or when the header value does not equal the supplied expected value.</exception>
        public VerifiableResponse ContentType(IMatcher<string> matcher, ErrorMessage errorMessage = default)
        {
            MediaTypeHeaderValue? actualContentType = this.Response.Content.Headers.ContentType;

            if (actualContentType == null)
            {
                this.FailVerification(errorMessage.HasValue
                    ? errorMessage.Value!
                    : "Response Content-Type header could not be found.");
            }

            if (!matcher.Matches(actualContentType!.ToString()))
            {
                this.FailVerification(errorMessage.HasValue
                    ? AssertionMessageBuilder.BuildMessage(errorMessage.Value!, matcher, actualContentType.ToString())
                    : $"Expected value for response Content-Type header to match '{matcher}', but was '{actualContentType}'.");
            }

            return this;
        }

        /// <summary>
        /// Verifies that the value of the cookie with the specified name matches a given NHamcrest matcher.
        /// </summary>
        /// <param name="name">The name of the cookie to verify.</param>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Cookie(string name, IMatcher<string> matcher, ErrorMessage errorMessage = default)
        {
            var cookies = this.CookieContainer.GetAllCookies().GetEnumerator();

            while (cookies.MoveNext())
            {
                Cookie cookie = (Cookie)cookies.Current;
                if (cookie.Name.Equals(name))
                {
                    if (!matcher.Matches(cookie.Value))
                    {
                        this.FailVerification(errorMessage.HasValue
                            ? AssertionMessageBuilder.BuildMessage(errorMessage.Value!, matcher, cookie.Value)
                            : $"Expected value for cookie with name '{name}' to match '{matcher}', but was '{cookie.Value}'.");
                    }

                    return this;
                }
            }

            this.FailVerification(errorMessage.HasValue
                ? errorMessage.Value!
                : $"Cookie with name '{name}' could not be found in the response.");

            return this;
        }

        /// <summary>
        /// Verifies that the response body is equal to the specified expected body.
        /// </summary>
        /// <param name="expectedResponseBody">The expected response body.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the actual response body does not match the expected one.</exception>
        public VerifiableResponse Body(string expectedResponseBody, ErrorMessage errorMessage = default)
        {
            return this.VerifyResponseBody(
                actual => !actual.Equals(expectedResponseBody),
                actual => $"Actual response body did not match expected response body.\nExpected: '{expectedResponseBody}'\nActual: '{actual}'",
                expectedResponseBody,
                errorMessage);
        }

        /// <summary>
        /// Verifies that the response body matches the specified NHamcrest matcher.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when the actual response body does not match the expected one.</exception>
        public VerifiableResponse Body(IMatcher<string> matcher, ErrorMessage errorMessage = default)
        {
            return this.VerifyResponseBody(
                actual => !matcher.Matches(actual),
                actual => $"Actual response body expected to match '{matcher}' but didn't.\nActual: '{actual}'",
                matcher,
                errorMessage);
        }

        /// <summary>
        /// Verifies that the response body matches the specified NHamcrest matcher.
        /// </summary>
        /// <typeparam name="T">The type of value that the matcher operates on.</typeparam>
        /// <param name="path">The JsonPath or XPath expression to evaluate.</param>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <param name="verifyAs">Indicates how to interpret the response.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Body<T>(string path, IMatcher<T> matcher, VerifyAs verifyAs = VerifyAs.UseResponseContentTypeHeaderValue, ErrorMessage errorMessage = default)
        {
            return this.DispatchBodyVerification(
                path,
                verifyAs,
                (np, rb) => this.VerifyJsonBody(np, matcher, rb, errorMessage),
                (np, rb) => this.VerifyMarkupBody(np, matcher, rb, errorMessage));
        }

        /// <summary>
        /// Verifies that the response body matches the specified NHamcrest matcher.
        /// </summary>
        /// <typeparam name="T">The type of value that the matcher operates on.</typeparam>
        /// <param name="path">The JsonPath expression to evaluate.</param>
        /// <param name="matcher">The NHamcrest matcher to evaluate.</param>
        /// <param name="verifyAs">Indicates how to interpret the response.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse Body<T>(string path, IMatcher<IEnumerable<T>> matcher, VerifyAs verifyAs = VerifyAs.UseResponseContentTypeHeaderValue, ErrorMessage errorMessage = default)
        {
            return this.DispatchBodyVerification(
                path,
                verifyAs,
                (np, rb) => this.VerifyJsonElements(np, matcher, rb, errorMessage),
                (np, rb) => this.VerifyMarkupElements(np, matcher, rb, errorMessage));
        }

        /// <summary>
        /// Verifies that the JSON response body matches the supplied JSON schema.
        /// </summary>
        /// <param name="jsonSchema">The JSON schema to verify the response against.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        /// <exception cref="ResponseVerificationException">Thrown when can't parse supplied JSON schema.</exception>
        public VerifiableResponse MatchesJsonSchema(string jsonSchema)
        {
            if (File.Exists(Path.GetFullPath(jsonSchema)))
            {
                jsonSchema = File.ReadAllText(Path.GetFullPath(jsonSchema));
            }

            try
            {
                JsonSchema parsedSchema = JsonSchema.FromJsonAsync(jsonSchema).GetAwaiter().GetResult();
                return this.MatchesJsonSchema(parsedSchema);
            }
            catch (JsonException je)
            {
                this.FailVerification($"Could not parse supplied JSON schema. Error: {je.Message}");
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
            this.RequireContentType(SupportedContentType.Json);

            string responseBodyAsString = this.ReadBodyAsString();

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
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemas;

            this.ReadAndValidateXml(settings, "Response body did not match XML schema supplied");
            return this;
        }

        /// <summary>
        /// Verifies that the XML response body matches the inline DTD.
        /// </summary>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse MatchesInlineDtd()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;

            this.ReadAndValidateXml(settings, "Response body did not match inline DTD");
            return this;
        }

        /// <summary>
        /// Verifies that the response time matches the specified NHamcrest matcher.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to match against the response time.</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse ResponseTime(IMatcher<TimeSpan> matcher, ErrorMessage errorMessage = default)
        {
            this.VerifyWithMatcher(matcher, this.ElapsedTime, $"Expected response time to match '{matcher}' but was '{this.ElapsedTime}'", errorMessage);
            return this;
        }

        /// <summary>
        /// Verifies that the response body length (in bytes) matches the specified NHamcrest matcher.
        /// </summary>
        /// <param name="matcher">The NHamcrest matcher to match against the response body length (in bytes).</param>
        /// <param name="errorMessage">A custom error message to be used when the verification fails.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        public VerifiableResponse ResponseBodyLength(IMatcher<int> matcher, ErrorMessage errorMessage = default)
        {
            string responseContentAsString = this.ReadBodyAsString();

            if (!matcher.Matches(responseContentAsString.Length))
            {
                this.FailVerification(errorMessage.HasValue
                    ? AssertionMessageBuilder.BuildMessage(errorMessage.Value!, matcher, responseContentAsString.Length)
                    : $"Expected response body length to match '{matcher}' but was '{responseContentAsString.Length}'");
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
        [Obsolete("This method is obsolete and will be removed in RestAssured.Net version 5.0.0. Please use DeserializeTo<T>() instead.")]
        public object? DeserializeTo(Type type, DeserializeAs deserializeAs = DeserializeAs.UseResponseContentTypeHeaderValue)
        {
            return Deserializer.DeserializeResponseInto(this.Response, type, deserializeAs, this.jsonSerializerSettings);
        }

        /// <summary>
        /// Deserializes the response content into the specified type and returns it.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response body into.</typeparam>
        /// <param name="deserializeAs">Indicates how to interpret the response content when deserializing.</param>
        /// <returns>The deserialized response object.</returns>
        public T? DeserializeTo<T>(DeserializeAs deserializeAs = DeserializeAs.UseResponseContentTypeHeaderValue)
            where T : class
        {
            return Deserializer.DeserializeResponseInto<T>(this.Response, deserializeAs, this.jsonSerializerSettings);
        }

        /// <summary>
        /// Logs response details to the standard output.
        /// </summary>
        /// <param name="responseLogLevel">The required log level.</param>
        /// <param name="sensitiveHeaderOrCookieNames">The names of the response headers or cookies to be masked when logging.</param>
        /// <returns>The current <see cref="VerifiableResponse"/> object.</returns>
        [Obsolete("Use Log(LogConfiguration logConfiguration) in ExecutableRequest instead. This method will be removed in RestAssured.Net 5.0.0")]
        public VerifiableResponse Log(Logging.ResponseLogLevel responseLogLevel, List<string>? sensitiveHeaderOrCookieNames = null)
        {
            if (responseLogLevel == Logging.ResponseLogLevel.OnVerificationFailure)
            {
                this.LogOnVerificationFailure = true;
                return this;
            }

            ResponseLogger.Log(this.Response, this.CookieContainer, responseLogLevel, sensitiveHeaderOrCookieNames ?? new List<string>(), this.ElapsedTime);
            return this;
        }

        /// <summary>
        /// Gives access to various methods to extract values from this response object.
        /// </summary>
        /// <returns>An <see cref="ExtractableResponse"/> object from which values can then be extracted.</returns>
        public ExtractableResponse Extract()
        {
            return new ExtractableResponse(this.Response, this.CookieContainer, this.ElapsedTime);
        }

        private void RequireContentType(SupportedContentType required)
        {
            string mediaType = this.Response.Content.Headers.ContentType?.MediaType ?? string.Empty;
            string requiredFragment = required.ToString().ToLower();

            if (!mediaType.Contains(requiredFragment))
            {
                this.FailVerification($"Expected response Content-Type header to contain '{requiredFragment}', but was '{mediaType}'");
            }
        }

        private void ReadAndValidateXml(XmlReaderSettings settings, string failureMessage)
        {
            this.RequireContentType(SupportedContentType.Xml);

            string responseXmlAsString = this.ReadBodyAsString();
            XmlReader reader = XmlReader.Create(new StringReader(responseXmlAsString), settings);

            try
            {
                while (reader.Read())
                {
                }
            }
            catch (XmlSchemaException xse)
            {
                this.FailVerification($"{failureMessage}. Error: '{xse.Message}'");
            }
        }

        private void VerifyJsonBody<T>(NodePath nodePath, IMatcher<T> matcher, ResolvedBody resolved, ErrorMessage errorMessage = default)
        {
            JToken? resultingElement = JToken.Parse(resolved.Content).SelectToken(nodePath.Expression);

            if (resultingElement == null)
            {
                this.FailVerification(errorMessage.HasValue
                    ? $"{errorMessage.Value!}: JsonPath expression '{nodePath.Expression}' did not yield any results."
                    : $"JsonPath expression '{nodePath.Expression}' did not yield any results.");
            }

            T valueToMatch = resultingElement!.GetType().Equals(typeof(JArray))
                ? (T)resultingElement!.ToObject<ICollection<T>>() !
                : resultingElement!.ToObject<T>() !;

            if (!matcher.Matches(valueToMatch))
            {
                this.FailVerification(errorMessage.HasValue
                    ? AssertionMessageBuilder.BuildMessage(errorMessage.Value!, matcher, resultingElement!)
                    : $"Expected element selected by '{nodePath.Expression}' to match '{matcher}' but was '{resultingElement}'");
            }
        }

        private void VerifyJsonElements<T>(NodePath nodePath, IMatcher<IEnumerable<T>> matcher, ResolvedBody resolved, ErrorMessage errorMessage = default)
        {
            List<T> elementValues = new List<T>();

            IEnumerable<JToken> resultingElements = JToken.Parse(resolved.Content).SelectTokens(nodePath.Expression);

            foreach (JToken element in resultingElements)
            {
                elementValues.Add(element.ToObject<T>() !);
            }

            if (!matcher.Matches(elementValues))
            {
                this.FailVerification(errorMessage.HasValue
                    ? AssertionMessageBuilder.BuildMessage(errorMessage.Value!, matcher, string.Join(", ", elementValues))
                    : $"Expected elements selected by '{nodePath.Expression}' to match '{matcher}', but was [{string.Join(", ", elementValues)}]");
            }
        }

        private void VerifyMarkupBody<T>(NodePath nodePath, IMatcher<T> matcher, ResolvedBody resolved, ErrorMessage errorMessage = default)
        {
            string innerText = this.SelectSingleNodeInnerText(nodePath, resolved);

            // Try and cast the element value to an object of the type used in the matcher
            try
            {
                if (!matcher.Matches((T)Convert.ChangeType(innerText, typeof(T))))
                {
                    this.FailVerification(errorMessage.HasValue
                        ? AssertionMessageBuilder.BuildMessage(errorMessage.Value!, matcher, innerText)
                        : $"Expected element selected by '{nodePath.Expression}' to match '{matcher}' but was '{innerText}'");
                }
            }
            catch (FormatException)
            {
                this.FailVerification(errorMessage.HasValue
                    ? $"{errorMessage.Value!}: Response element value {innerText} cannot be converted to value of type '{typeof(T)}'"
                    : $"Response element value {innerText} cannot be converted to value of type '{typeof(T)}'");
            }
        }

        private void VerifyMarkupElements<T>(NodePath nodePath, IMatcher<IEnumerable<T>> matcher, ResolvedBody resolved, ErrorMessage errorMessage = default)
        {
            List<T> elementValues = new List<T>();

            // Try and cast the element values to an object of the type used in the matcher
            foreach (string innerText in this.SelectNodeInnerTexts(nodePath, resolved))
            {
                try
                {
                    elementValues.Add((T)Convert.ChangeType(innerText, typeof(T)));
                }
                catch (FormatException)
                {
                    this.FailVerification(errorMessage.HasValue
                        ? $"{errorMessage.Value!}: Response element value {innerText} cannot be converted to object of type {typeof(T)}"
                        : $"Response element value {innerText} cannot be converted to object of type {typeof(T)}");
                }
            }

            if (!matcher.Matches(elementValues))
            {
                this.FailVerification(errorMessage.HasValue
                    ? AssertionMessageBuilder.BuildMessage(errorMessage.Value!, matcher, string.Join(", ", elementValues))
                    : $"Expected elements selected by '{nodePath.Expression}' to match '{matcher}', but was [{string.Join(", ", elementValues)}]");
            }
        }

        private string SelectSingleNodeInnerText(NodePath nodePath, ResolvedBody resolved)
        {
            if (resolved.ContentType == SupportedContentType.Xml)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resolved.Content);
                XmlNode? node = xmlDoc.SelectSingleNode(nodePath.Expression);
                if (node == null)
                {
                    this.FailVerification($"XPath expression '{nodePath.Expression}' did not yield any results.");
                }

                return node!.InnerText;
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(resolved.Content);
            HtmlNode? htmlNode = htmlDoc.DocumentNode.SelectSingleNode(nodePath.Expression);
            if (htmlNode == null)
            {
                this.FailVerification($"XPath expression '{nodePath.Expression}' did not yield any results.");
            }

            return htmlNode!.InnerText;
        }

        private IEnumerable<string> SelectNodeInnerTexts(NodePath nodePath, ResolvedBody resolved)
        {
            if (resolved.ContentType == SupportedContentType.Xml)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resolved.Content);
                return xmlDoc.SelectNodes(nodePath.Expression)!.Cast<XmlNode>().Select(n => n.InnerText).ToList();
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(resolved.Content);
            return htmlDoc.DocumentNode.SelectNodes(nodePath.Expression)!.Cast<HtmlNode>().Select(n => n.InnerText).ToList();
        }

        private ResolvedBody ResolveBodyAndContentType(VerifyAs verifyAs)
        {
            string body = this.ReadBodyAsString();
            string mediaType = this.Response.Content.Headers.ContentType?.MediaType ?? string.Empty;

            try
            {
                return new ResolvedBody(body, new ContentTypeUtils().DetermineResponseMediaTypeForResponse(mediaType, verifyAs));
            }
            catch (ExtractionException ee)
            {
                this.FailVerification(ee.Message);
                return default;
            }
        }

        private readonly record struct ResolvedBody(string Content, SupportedContentType ContentType);

        private readonly record struct NodePath(string Expression);

        private VerifiableResponse VerifyResponseBody(Func<string, bool> failCondition, Func<string, string> buildDefaultMessage, object expectedValue, ErrorMessage errorMessage)
        {
            string actual = this.ReadBodyAsString();

            if (failCondition(actual))
            {
                this.FailVerification(errorMessage.HasValue
                    ? AssertionMessageBuilder.BuildMessage(errorMessage.Value!, expectedValue, actual)
                    : buildDefaultMessage(actual));
            }

            return this;
        }

        private string ReadBodyAsString()
        {
            return this.Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        private void VerifyWithMatcher<T>(IMatcher<T> matcher, T actualValue, string defaultMessage, ErrorMessage errorMessage)
        {
            if (!matcher.Matches(actualValue))
            {
                this.FailVerification(errorMessage.HasValue
                    ? AssertionMessageBuilder.BuildMessage(errorMessage.Value!, matcher, actualValue!)
                    : defaultMessage);
            }
        }

        private VerifiableResponse DispatchBodyVerification(string path, VerifyAs verifyAs, Action<NodePath, ResolvedBody> jsonVerify, Action<NodePath, ResolvedBody> markupVerify)
        {
            ResolvedBody resolved = this.ResolveBodyAndContentType(verifyAs);
            NodePath nodePath = new NodePath(path);

            if (resolved.ContentType.Equals(SupportedContentType.Json))
            {
                jsonVerify(nodePath, resolved);
            }
            else
            {
                markupVerify(nodePath, resolved);
            }

            return this;
        }

        private void FailVerification(string exceptionMessage)
        {
            if (this.LogOnVerificationFailure)
            {
                var logConfiguration = new LogConfiguration
                {
                    ResponseLogLevel = RestAssured.Logging.ResponseLogLevel.All,
                };

                var logger = new RequestResponseLogger(logConfiguration, this.Logger);
                logger.LogResponse(this);
            }

            throw new ResponseVerificationException(exceptionMessage);
        }
    }
}
