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
using System;
using System.Net.Http;
using RestAssured.Net.RA.Builders;
using RestAssuredNet.RA.Exceptions;

namespace RestAssured.Net.RA.Internal
{
    /// <summary>
    /// Provides utility methods to apply a <see cref="RequestSpecification"/> to an <see cref="HttpRequestMessage"/>.
    /// </summary>
    public class RequestSpecificationProcessor
    {
        /// <summary>
        /// Applies a <see cref="RequestSpecification"/> to an <see cref=HttpRequestMessage"/>.
        /// </summary>
        /// <param name="requestSpec">The <see cref="RequestSpecification"/> to apply.</param>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to apply it to.</param>
        /// <param name="endpoint">The endpoint as originally supplied by the user.</param>
        /// <returns>The updated <see cref="HttpRequestMessage"/> object.</returns>
        public static HttpRequestMessage Apply(RequestSpecification requestSpec, HttpRequestMessage request, string endpoint)
        {
            try
            {
                Uri uri = new Uri(endpoint);

                // If the endpoint supplied is a valid URI, ignore the request specification
                request.RequestUri = uri;
                return request;
            }
            catch (UriFormatException)
            {
                try
                {
                    UriBuilder uri = new UriBuilder();
                    uri.Scheme = requestSpec.Scheme;
                    uri.Host = requestSpec.HostName;
                    uri.Port = requestSpec.Port;
                    uri.Path = BuildPath(requestSpec.BasePath, endpoint);
                    request.RequestUri = uri.Uri;
                }
                catch (UriFormatException)
                {
                    throw new RequestCreationException($"Supplied base URI '{requestSpec.Scheme}://{requestSpec.HostName}:{requestSpec.Port}' is invalid.");
                }
            }

            if (requestSpec.UserAgent != null)
            {
                request.Headers.UserAgent.Add(requestSpec.UserAgent);
            }

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
