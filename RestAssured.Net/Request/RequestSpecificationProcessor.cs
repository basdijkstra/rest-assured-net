// <copyright file="RequestSpecificationProcessor.cs" company="On Test Automation">
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
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using RestAssured.Request.Builders;
    using RestAssured.Request.Exceptions;

    /// <summary>
    /// Provides utility methods to apply a <see cref="RequestSpecification"/> to an <see cref="HttpRequestMessage"/>.
    /// </summary>
    internal class RequestSpecificationProcessor
    {
        /// <summary>
        /// Builds the Uri using information provided in a request specification.
        /// </summary>
        /// <param name="requestSpec">The request specification containing the information to build the endpoint.</param>
        /// <param name="endpoint">The original endpoint as provided by the user.</param>
        /// <returns>A <see cref="Uri"/> to use in the request.</returns>
        /// <exception cref="RequestCreationException">Thrown whenever the Uri cannot be created with the specified information.</exception>
        internal static Uri BuildUriFromRequestSpec(RequestSpecification requestSpec, string endpoint)
        {
            if (requestSpec is null)
            {
                throw new RequestCreationException("Supplied request specification is null.");
            }

            try
            {
                UriBuilder uri = new UriBuilder();
                uri.Scheme = requestSpec.Scheme;
                uri.Host = requestSpec.HostName;
                uri.Port = requestSpec.Port;
                uri.Path = BuildPath(requestSpec.BasePath, endpoint);

                return uri.Uri;
            }
            catch (UriFormatException)
            {
                throw new RequestCreationException($"Supplied base URI '{requestSpec.Scheme}://{requestSpec.HostName}:{requestSpec.Port}' is invalid.");
            }
        }

        /// <summary>
        /// Applies a <see cref="RequestSpecification"/> to an <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="requestSpec">The <see cref="RequestSpecification"/> to apply.</param>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to apply it to.</param>
        /// <returns>The updated <see cref="HttpRequestMessage"/> object.</returns>
        internal static HttpRequestMessage Apply(RequestSpecification requestSpec, HttpRequestMessage request)
        {
            if (requestSpec == null)
            {
                return request;
            }

            foreach (KeyValuePair<string, object> entry in requestSpec.Headers)
            {
                request.Headers.Add(entry.Key, entry.Value.ToString());
            }

            if (requestSpec.UserAgent != null)
            {
                request.Headers.UserAgent.Add(requestSpec.UserAgent);
            }

            request.Headers.Authorization ??= requestSpec.AuthenticationHeader;
            return request;
        }

        /// <summary>
        /// Builds the path to the resource to target in the request. Should have no leading / and all double // removed.
        /// </summary>
        /// <param name="basePath">The base path as supplied in the request specification (can be an empty string).</param>
        /// <param name="originalEndpoint">The original endpoint as supplied in the request.</param>
        /// <returns>A correctly formatted path to the resource to target in the request.</returns>
        private static string BuildPath(string basePath, string originalEndpoint)
        {
            string trimmedBasePath = basePath.TrimStart('/').TrimEnd('/');
            string trimmedOriginalEndpoint = originalEndpoint.TrimStart('/').TrimEnd('/');

            return $"{trimmedBasePath}/{trimmedOriginalEndpoint}";
        }
    }
}
