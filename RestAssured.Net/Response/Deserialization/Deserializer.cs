// <copyright file="Deserializer.cs" company="On Test Automation">
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
namespace RestAssured.Response.Deserialization
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using RestAssured.Response.Exceptions;

    /// <summary>
    /// Class providing utility methods for response body deserialization.
    /// </summary>
    internal class Deserializer
    {
        /// <summary>
        /// Deserializes an <see cref="HttpResponseMessage"/> body into the specified type.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> containing the body to be deserialized.</param>
        /// <param name="type">The type to deserialize the response body into.</param>
        /// <param name="deserializeAs">Indicates how to interpret the response content when deserializing.</param>
        /// <param name="jsonSerializerSettings">The <see cref="JsonSerializerSettings"/> to use when deserializing to JSON.</param>
        /// <returns>The deserialized response body object.</returns>
        /// <exception cref="DeserializationException">Thrown when deserialization of the response body fails.</exception>
        internal static object DeserializeResponseInto(HttpResponseMessage response, Type type, DeserializeAs deserializeAs, JsonSerializerSettings jsonSerializerSettings)
        {
            string responseBodyAsString = response.Content.ReadAsStringAsync().Result;

            if (responseBodyAsString == null || responseBodyAsString.Equals(string.Empty))
            {
                throw new DeserializationException("Response content is null or empty.");
            }

            string? responseMediaType = string.Empty;

            switch (deserializeAs)
            {
                case DeserializeAs.UseResponseContentTypeHeaderValue:
                {
                        responseMediaType = response.Content.Headers.ContentType?.MediaType;
                        break;
                }

                case DeserializeAs.Json:
                {
                        responseMediaType = "application/json";
                        break;
                }

                case DeserializeAs.Xml:
                {
                        responseMediaType = "application/xml";
                        break;
                }
            }

            if (responseMediaType == null || responseMediaType.Contains("json"))
            {
                return JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result, type, jsonSerializerSettings) ?? string.Empty;
            }
            else if (responseMediaType.Contains("xml"))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                using (TextReader reader = new StringReader(response.Content.ReadAsStringAsync().Result))
                {
                    return xmlSerializer.Deserialize(reader) !;
                }
            }
            else
            {
                throw new DeserializationException($"Unable to deserialize response with Content-Type '{responseMediaType}'");
            }
        }
    }
}
