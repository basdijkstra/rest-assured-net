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
    using System.Linq;
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

            // A query string in the endpoint must not become part of the path, because
            // UriBuilder escapes the '?' in a path, so split it off and set it as the query.
            string path = endpoint;
            string query = string.Empty;
            int queryStart = endpoint.IndexOf('?');

            if (queryStart >= 0)
            {
                path = endpoint.Substring(0, queryStart);
                query = endpoint.Substring(queryStart + 1);
            }

            try
            {
                UriBuilder uri = new UriBuilder();
                uri.Scheme = requestSpec.Scheme;
                uri.Host = requestSpec.HostName;
                uri.Port = requestSpec.Port;
                uri.Path = BuildPath(requestSpec.BaseUriPath, requestSpec.BasePath, path);
                uri.Query = query;

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

            // Headers set in the request take precedence over (replace) headers with the same name
            // in the request specification, header names are compared case-insensitively.
            foreach (KeyValuePair<string, object> entry in requestSpec.Headers)
            {
                if (!request.Headers.Contains(entry.Key))
                {
                    request.Headers.Add(entry.Key, entry.Value.ToString());
                }
            }

            if (requestSpec.UserAgent != null && !request.Headers.Contains("User-Agent"))
            {
                request.Headers.UserAgent.Add(requestSpec.UserAgent);
            }

            request.Headers.Authorization ??= requestSpec.AuthenticationHeader;
            return request;
        }

        /// <summary>
        /// Builds the path to the resource to target in the request. Should have no leading / and all double // removed.
        /// A trailing / in the endpoint is kept, because /resource and /resource/ can be different resources.
        /// </summary>
        /// <param name="baseUriPath">The path part of the base URI as supplied in the request specification (can be an empty string).</param>
        /// <param name="basePath">The base path as supplied in the request specification (can be an empty string).</param>
        /// <param name="originalEndpoint">The original endpoint as supplied in the request.</param>
        /// <returns>A correctly formatted path to the resource to target in the request.</returns>
        private static string BuildPath(string baseUriPath, string basePath, string originalEndpoint)
        {
            string trimmedBaseUriPath = baseUriPath.TrimStart('/').TrimEnd('/');
            string trimmedBasePath = basePath.TrimStart('/').TrimEnd('/');
            string trimmedOriginalEndpoint = originalEndpoint.TrimStart('/');

            // The path part of the base URI comes first, followed by the base path (if any).
            string prefix = string.Join("/", new[] { trimmedBaseUriPath, trimmedBasePath }.Where(part => part.Length > 0));

            return $"{prefix}/{trimmedOriginalEndpoint}";
        }
    }
}
