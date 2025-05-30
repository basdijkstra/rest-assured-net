﻿// <copyright file="ExtractableResponse.cs" company="On Test Automation">
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
    using System.Xml;
    using HtmlAgilityPack;
    using Newtonsoft.Json.Linq;
    using RestAssured.Response.ContentType;
    using RestAssured.Response.Exceptions;

    /// <summary>
    /// A class representing an <see cref="HttpResponseMessage"/> from which values can be extracted.
    /// </summary>
    public class ExtractableResponse
    {
        private readonly HttpResponseMessage response;
        private readonly CookieContainer cookieContainer;
        private readonly TimeSpan elapsedTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractableResponse"/> class.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> object from which values should be extracted.</param>
        /// <param name="cookieContainer">The <see cref="CookieContainer"/> used by the HTTP client.</param>
        /// <param name="elapsedTime">The time elapsed between sending a request and receiving a response. Not to be used for performance tests.</param>
        public ExtractableResponse(HttpResponseMessage response, CookieContainer cookieContainer, TimeSpan elapsedTime)
        {
            this.response = response;
            this.cookieContainer = cookieContainer;
            this.elapsedTime = elapsedTime;
        }

        /// <summary>
        /// Extracts the entire response body as a string.
        /// </summary>
        /// <returns>The response body as a string.</returns>
        [Obsolete("Please use BodyAsString(), BodyAsByteArray() or BodyAsStream() instead. This method is obsolete and will be removed in RestAssured.Net 5.0.0")]
        public string Body()
        {
            return this.response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Extracts the entire response body as a string.
        /// </summary>
        /// <returns>The response body as a string.</returns>
        public string BodyAsString()
        {
            return this.response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Extracts the entire response body as a byte array.
        /// </summary>
        /// <returns>The response body as a byte array.</returns>
        public byte[] BodyAsByteArray()
        {
            return this.response.Content.ReadAsByteArrayAsync().Result;
        }

        /// <summary>
        /// Extracts the entire response body as a <see cref="Stream"/>.
        /// </summary>
        /// <returns>The response body as a <see cref="Stream"/>.</returns>
        public Stream BodyAsStream()
        {
            return this.response.Content.ReadAsStreamAsync().Result;
        }

        /// <summary>
        /// Extracts a response body element value from the response based on a JsonPath expression.
        /// </summary>
        /// <param name="path">The JsonPath or XPath expression pointing to the object to extract.</param>
        /// <param name="extractAs">Indicates how to interpret the response.</param>
        /// <param name="returnAs">Indicates how to return singular extracted values.</param>
        /// <returns>The element value or values extracted from the response using the JsonPath expression.</returns>
        /// <exception cref="ExtractionException">Thrown when evaluating the JsonPath did not yield any results.</exception>
        public object Body(string path, ExtractAs extractAs = ExtractAs.UseResponseContentTypeHeaderValue, ReturnAs returnAs = ReturnAs.Singular)
        {
            string responseBodyAsString = this.response.Content.ReadAsStringAsync().Result;

            string responseMediaType = this.response.Content.Headers.ContentType?.MediaType ?? string.Empty;

            var contentType = new ContentTypeUtils().DetermineResponseMediaTypeForResponse(responseMediaType, extractAs);

            if (contentType.Equals(SupportedContentType.Xml))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseBodyAsString);
                XmlNodeList? xmlElements = xmlDoc.SelectNodes(path);

                if (xmlElements == null || xmlElements.Count == 0)
                {
                    throw new ExtractionException($"XPath expression '{path}' did not yield any results.");
                }

                if (xmlElements.Count == 1 && returnAs.Equals(ReturnAs.Singular))
                {
                    return xmlElements.Item(0) !.InnerText;
                }

                List<string> xmlElementValues = new List<string>();
                foreach (XmlNode xmlElement in xmlElements)
                {
                    xmlElementValues.Add(xmlElement.InnerText);
                }

                return xmlElementValues;
            }

            if (contentType.Equals(SupportedContentType.Html))
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(responseBodyAsString);
                HtmlNodeCollection? htmlElements = htmlDoc.DocumentNode.SelectNodes(path);

                if (htmlElements == null || htmlElements.Count == 0)
                {
                    throw new ExtractionException($"XPath expression '{path}' did not yield any results.");
                }

                if (htmlElements.Count == 1 && returnAs.Equals(ReturnAs.Singular))
                {
                    return htmlElements.First().InnerText;
                }

                List<string> htmlElementValues = new List<string>();
                foreach (HtmlNode htmlElement in htmlElements)
                {
                    htmlElementValues.Add(htmlElement.InnerText);
                }

                return htmlElementValues;
            }

            IEnumerable<JToken>? resultingElements = JToken.Parse(responseBodyAsString).SelectTokens(path);

            List<object?> jsonElementValues = resultingElements
                .Select(element => element.ToObject<object>())
                .ToList();

            if (!jsonElementValues.Any())
            {
                throw new ExtractionException($"JsonPath expression '{path}' did not yield any results.");
            }

            if (jsonElementValues.Count == 1 && returnAs.Equals(ReturnAs.Singular))
            {
                return jsonElementValues.First() !;
            }

            return jsonElementValues;
        }

        /// <summary>
        /// Returns the value for the specified header name from the response.
        /// </summary>
        /// <param name="name">The header to return.</param>
        /// <returns>The associated header value.</returns>
        /// <exception cref="ExtractionException">Thrown when the specified header name could not be located in the response.</exception>
        public string Header(string name)
        {
            if (this.response.Headers.TryGetValues(name, out IEnumerable<string>? values))
            {
                return values.First();
            }
            else
            {
                throw new ExtractionException($"Header with name '{name}' could not be found in the response.");
            }
        }

        /// <summary>
        /// Returns the value of the cookie with the specified name.
        /// </summary>
        /// <param name="name">The name of the cookie to return.</param>
        /// <returns>The value of the cookie.</returns>
        public string Cookie(string name)
        {
            var cookies = this.cookieContainer.GetAllCookies().GetEnumerator();

            while (cookies.MoveNext())
            {
                Cookie cookie = (Cookie)cookies.Current;
                if (cookie.Name.Equals(name))
                {
                    return cookie.Value;
                }
            }

            throw new ExtractionException($"Cookie with name '{name}' could not be found in the response.");
        }

        /// <summary>
        /// Returns the entire HttpResponseMessage.
        /// </summary>
        /// <returns>The current <see cref="HttpResponseMessage"/> response object.</returns>
        public HttpResponseMessage Response()
        {
            return this.response;
        }

        /// <summary>
        /// Returns the time elapsed between sending the request and receiving the response. Not to be used for performance testing purposes.
        /// </summary>
        /// <returns>The time elapsed between sending the request and receiving the response.</returns>
        public TimeSpan ResponseTime()
        {
            return this.elapsedTime;
        }
    }
}
