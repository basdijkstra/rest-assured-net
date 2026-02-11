// <copyright file="RequestBodyFactory.cs" company="On Test Automation">
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
namespace RestAssured.Request
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Xml.Serialization;
    using Microsoft.AspNetCore.StaticFiles;
    using Newtonsoft.Json;
    using RestAssured.Request.Exceptions;

    /// <summary>
    /// Holds the serialization-related settings for creating a request body.
    /// </summary>
    /// <param name="contentType">The resolved content type for the request.</param>
    /// <param name="contentEncoding">The resolved content encoding for the request.</param>
    /// <param name="stripCharset">Flag indicating whether the charset should be stripped from the content type.</param>
    /// <param name="jsonSerializerSettings">The resolved JSON serializer settings for the request.</param>
    internal record RequestBodySettings(
        string contentType,
        Encoding contentEncoding,
        bool stripCharset,
        JsonSerializerSettings jsonSerializerSettings);

    /// <summary>
    /// Provides methods for creating and serializing HTTP request bodies.
    /// </summary>
    internal static class RequestBodyFactory
    {
        /// <summary>
        /// Creates the body payload for the request.
        /// </summary>
        /// <param name="multipartFormDataContent">The multipart form data content, if any.</param>
        /// <param name="formData">The form data, if any.</param>
        /// <param name="requestBody">The request body object.</param>
        /// <param name="settings">The serialization settings for the request body.</param>
        /// <returns>The request body as an object of type <see cref="HttpContent"/>.</returns>
        internal static HttpContent Create(
            MultipartFormDataContent? multipartFormDataContent,
            IEnumerable<KeyValuePair<string, string>>? formData,
            object requestBody,
            RequestBodySettings settings)
        {
            if (multipartFormDataContent != null)
            {
                return multipartFormDataContent;
            }

            if (formData != null)
            {
                return new FormUrlEncodedContent(formData);
            }

            string requestBodyAsString = Serialize(requestBody, settings.contentType, settings.jsonSerializerSettings);

            var stringContent = new StringContent(requestBodyAsString, settings.contentEncoding, settings.contentType);

            if (settings.stripCharset)
            {
                stringContent.Headers.ContentType!.CharSet = null;
            }

            return stringContent;
        }

        /// <summary>
        /// Serializes the request body to the appropriate format based on the content type.
        /// </summary>
        /// <param name="body">The request body object.</param>
        /// <param name="contentType">The request Content-Type header value.</param>
        /// <param name="jsonSerializerSettings">The JSON serializer settings to use when serializing to JSON.</param>
        /// <returns>Either the body itself (if the body is a string), or a serialized version of the body.</returns>
        internal static string Serialize(object body, string contentType, JsonSerializerSettings jsonSerializerSettings)
        {
            if (body is string)
            {
                return (string)body;
            }

            if (contentType.Contains("json"))
            {
                return JsonConvert.SerializeObject(body, jsonSerializerSettings);
            }

            if (contentType.Contains("xml"))
            {
                using (StringWriter sw = new StringWriter())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(body.GetType());
                    xmlSerializer.Serialize(sw, body);
                    return sw.ToString();
                }
            }

            throw new RequestCreationException(
                $"Could not determine how to serialize request based on specified content type '{contentType}'");
        }

        /// <summary>
        /// Determines the content type for a file based on its extension.
        /// </summary>
        /// <param name="fileName">The file to determine the content type for.</param>
        /// <returns>The <see cref="MediaTypeHeaderValue"/> for the file.</returns>
        internal static MediaTypeHeaderValue GetContentTypeForFile(FileInfo fileName)
        {
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();

            string contentType;

            if (!provider.TryGetContentType(fileName.FullName, out contentType!))
            {
                contentType = "application/octet-stream";
            }

            return MediaTypeHeaderValue.Parse(contentType);
        }
    }
}
