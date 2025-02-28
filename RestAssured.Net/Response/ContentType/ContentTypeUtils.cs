// <copyright file="ContentTypeUtils.cs" company="On Test Automation">
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
namespace RestAssured.Response.ContentType
{
    using System;
    using RestAssured.Response.Exceptions;

    /// <summary>
    /// Utility class and methods to handle and process Content-Type headers.
    /// </summary>
    internal class ContentTypeUtils
    {
        /// <summary>
        /// Determines the Content-Type to use when verifying response body values.
        /// </summary>
        /// <param name="responseMediaType">The response Content-Type header value as a string.</param>
        /// <param name="verifyAs">Indicates how to interpret the response.</param>
        /// <returns>The Content-Type to use when verifying response body values as a string.</returns>
        /// <exception cref="ArgumentException">Thrown when the supplied <see cref="VerifyAs"/> value is not supported.</exception>
        internal SupportedContentType DetermineResponseMediaTypeForResponse(string responseMediaType, VerifyAs verifyAs) => verifyAs switch
        {
            VerifyAs.UseResponseContentTypeHeaderValue => this.ParseResponseContentType(responseMediaType),
            VerifyAs.Json => SupportedContentType.Json,
            VerifyAs.Xml => SupportedContentType.Xml,
            VerifyAs.Html => SupportedContentType.Html,
            _ => throw new ArgumentException($"Unsupported argument value: {verifyAs}"),
        };

        /// <summary>
        /// Determines the Content-Type to use when extracting response body values.
        /// </summary>
        /// <param name="responseMediaType">The response Content-Type header value as a string.</param>
        /// <param name="extractAs">Indicates how to interpret the response.</param>
        /// <returns>The Content-Type to use when extracting response body values as a string.</returns>
        /// <exception cref="ArgumentException">Thrown when the supplied <see cref="ExtractAs"/> value is not supported.</exception>
        internal SupportedContentType DetermineResponseMediaTypeForResponse(string responseMediaType, ExtractAs extractAs) => extractAs switch
        {
            ExtractAs.UseResponseContentTypeHeaderValue => this.ParseResponseContentType(responseMediaType),
            ExtractAs.Json => SupportedContentType.Json,
            ExtractAs.Xml => SupportedContentType.Xml,
            ExtractAs.Html => SupportedContentType.Html,
            _ => throw new ArgumentException($"Unsupported argument value: {extractAs}"),
        };

        /// <summary>
        /// Parses a Content-Type header given as a string to a <see cref="SupportedContentType"/>.
        /// </summary>
        /// <param name="contentType">The Content-Type header value as a string.</param>
        /// <returns>The parsed <see cref="SupportedContentType"/> value.</returns>
        /// <exception cref="ExtractionException">Thrown when the specified Content-Type value cannot be parsed.</exception>
        internal SupportedContentType ParseResponseContentType(string contentType)
        {
            if (contentType.Contains("xml"))
            {
                return SupportedContentType.Xml;
            }

            if (contentType.Contains("html"))
            {
                return SupportedContentType.Html;
            }

            if (contentType.Equals(string.Empty) || contentType.Contains("json"))
            {
                return SupportedContentType.Json;
            }

            throw new ExtractionException($"Unable to extract elements from response with Content-Type '{contentType}'");
        }
    }
}
